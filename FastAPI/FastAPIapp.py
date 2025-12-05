from fastapi import FastAPI
from pydantic import BaseModel
import sqlite3
import requests
import json
import re
import os
from langchain_ollama import OllamaLLM
from langchain.chains import create_sql_query_chain
from langchain_community.utilities import SQLDatabase
from langchain.chains import LLMChain
from langchain.prompts import PromptTemplate
import logging
from langchain.callbacks import StdOutCallbackHandler

# Initialize FastAPI app
app = FastAPI()

# Initialize logging
logging.basicConfig(level=logging.INFO, format="%(message)s")
handler = StdOutCallbackHandler()
logger = logging.getLogger("FastAPIapp")

# Defines the request schema 
class QueryRequest(BaseModel):
    prompt: str

# Path to your SQLite database
# Make sure to change this path to your actual database path
#DB_PATH = r"C:\Users\P\OneDrive\Documents\AiChatAppProject\SQLiteDB\thrombosis_prediction\thrombosis_prediction.sqlite"

# Get the base directory where this FastAPI script is located
BASE_DIR = os.path.dirname(os.path.abspath(__file__))

# SQLite path relative to project
DB_PATH = os.path.join(BASE_DIR, "..", "SQLiteDB", "thrombosis_prediction", "thrombosis_prediction.sqlite")

# GUI path (if needed elsewhere)
GUI_PATH = os.path.join(BASE_DIR, "..", "GUI", "FivosChatbotGUI", "bin", "Debug", "FivosChatbotGUI.exe")


# Creates the SQL lite connection
connection = sqlite3.connect(DB_PATH, check_same_thread=False)

# LangChain database wrapper
#db = SQLDatabase.from_uri(f"sqlite:///{DB_PATH}")

@app.get("/ping")
def ping():
    print("Pinged!")
    return {"status": "ok"}

# Initialize LangChain model
llm = OllamaLLM(model="tinyllama")


def get_schema():
    cur = connection.cursor()
    cur.execute("SELECT name FROM sqlite_master WHERE type='table';")
    tables = cur.fetchall()
    
    schema = {}
    for (table,) in tables:
        cur.execute(f"PRAGMA table_info({table});")
        columns = cur.fetchall()
        schema[table] = [f"{col[1]} ({col[2]})" for col in columns]
    cur.close()
    return json.dumps(schema, indent=2)


def extract_sql_query(ai_text: str) -> str:

    # 🧹 Clean common formatting junk
    ai_text = (
        ai_text.replace("```sql", "")
        .replace("```", "")
        .replace("SQL:", "")
        .replace("SQL", "")
        .replace("Here’s your SQL:", "")
        .replace("Query:", "")
        .replace("Here's your SQL:", "")
        .strip()
    )

    # 🧠 Try to match SELECT first
    matches = re.findall(r"\bSELECT\b[\s\S]*?;", ai_text, flags=re.IGNORECASE)

    # 🧩 If SELECT not found, try other SQL commands
    if not matches:
        matches = re.findall(r"(CREATE|INSERT|UPDATE|DELETE)[\s\S]*?;", ai_text, flags=re.IGNORECASE)

    # 🪄 Final fallback for unusual formatting
    if not matches:
        matches = re.findall(r"SELECT.*?;", ai_text, flags=re.IGNORECASE | re.DOTALL)

    # ✅ Return first clean match or fallback to entire text
    sql_query = matches[0].strip() if matches else ai_text.strip()

    print(f"✅ Generated SQL:\n{sql_query}\n")
    return sql_query


@app.post("/ask")
def ask(request: QueryRequest):
    prompt = request.prompt
    schema = get_schema()

    print("\n🧠 [1] Generating SQL query using LangChain & TinyLlama...\n")

    with open("train_gold.sql", "r", encoding="utf-8") as file: golden_set = file.read()
    with open("thrombosis_mini_dev_sqlite.json", "r", encoding="utf-8") as f: golden_data = json.load(f)

    golden_set_text = ""

    for item in golden_data:
        q = item["question"]
        sql = item["SQL"]
        golden_set_text += f"Q: {q}\nSQL: {sql}\n\n"


    ollama_payload = {
        "model": "tinyllama",
        "prompt": (
            "You are an expert SQLite query generator. "
            "Use ONLY the tables and columns provided below. "
            "Do NOT create new tables or guess column names.\n\n"
            "=== DATABASE SCHEMA ===\n"
            f"{schema}\n"
            "========================\n\n"
            "=== GOLDEN SET EXAMPLES ===\n"
            "You may use the golden set below as reference examples for query style and structure.\n"
            f"{golden_set_text}"
            "=== ADDITIONAL SQL EXAMPLES ===\n"
            f"{golden_set}\n"                    # <-- moved inside section
            "============================\n\n"
            "User Question:\n"
            f"{prompt}\n\n"
            "Rules:\n"
            "1. Use only valid SQLite syntax.\n"
            "2. Return exactly ONE SQL query, ending with a semicolon.\n"
            "3. Do NOT include explanations, markdown, or commentary.\n"
            "4. Table names are case-sensitive — use exactly as shown.\n\n"
            "Output:\n"
            "Only the SQL query, nothing else."
        ),
        "stream": False
    }



    try:
        response = requests.post(
            "http://localhost:11434/api/generate",
            json=ollama_payload,
            timeout=180  # ⏱️ 180-second timeout for Ollama
        )
        response.raise_for_status()

        ai_text = response.json().get("response", "").strip()
        print("🧠 Raw model output:\n", ai_text)

        # ✅ Use extract_sql_query function to extract SQL
        sql_query = extract_sql_query(ai_text)
        print("🧠 Cleaned SQL Query:", sql_query)

        # ✅ Execute cleaned query
        try:
            print("⚙️ [2] Executing query on SQLite database...")
            cur = connection.cursor()
            cur.execute(sql_query)
            rows = cur.fetchall()
            cur.close()
            results = [list(row) for row in rows]
            print(f"✅ Retrieved {len(results)} rows.\n")

        except Exception as e:
            return {"error": str(e), "sql": sql_query}

        # ✅ [3] Generate natural-language summary using LangChain
        print("💬 [3] Generating natural-language summary using LangChain...\n")
        llm = OllamaLLM(model="tinyllama")  # small, more natural-text focused model
        summary_prompt = PromptTemplate(
            input_variables=["query", "results"],
            template=(
                "You are a helpful assistant. Given the SQL query:\n{query}\n"
                "and its results:\n{results}\n"
                "summarize the findings in clear, natural English."
            ),
        )

        summary_chain = LLMChain(llm=llm, prompt=summary_prompt)
        summary = summary_chain.run({"query": sql_query, "results": results})
        print(f"🧾 [4] Summary generated:\n{summary}\n")

        # ✅ [4] Return all data (SQL, Results, and Summary)
        response_data = {
            "sql": sql_query,
            "result": results,
            "summary": summary
        }

        print("📤 Sending to GUI:", response_data)
        return response_data

    except requests.Timeout:
        return {"error": "Ollama request timed out after 60 seconds."}
    except Exception as e:
        return {"error": str(e)}



# How to run the app:    
# Ensure you have FastAPI, Uvicorn, Langchain, Requests and Pydantic installed:
# In the console: python -m pip install langchain langchain-community fastapi uvicorn requests pydantic
# Install the Ollama Langchain integration: pip install -U langchain-ollama
# In the console: python -m pip install "uvicorn[standard]"
# Ensure you have a SQLite database that matches the name in the code (e.g., ChatAppDB.db or modify the code to match your database name).
# Ensure you have Ollama running locally with the correct model:
# In a terminal: ollama serve
# In another terminal (ensures you're using the right model): ollama pull (model name)
# To run the app, use the command in another terminal (make sure to cd to your folder): uvicorn FastAPIapp:app --reload
# After running this, FastAPI will be available at http://127.0.0.1:8000    