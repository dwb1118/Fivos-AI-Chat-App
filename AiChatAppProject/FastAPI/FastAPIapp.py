from fastapi import FastAPI
from pydantic import BaseModel
import sqlite3
import requests
import json

app = FastAPI()

@app.get("/ping")
def ping():
    print("pinged")
    return {"status": "ok"}

# Defines the request schema 
class QueryRequest(BaseModel):
    prompt: str

# Creates the SQL lite connection
connection = sqlite3.connect("ChatAppDB.db", check_same_thread=False)

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

'''
@app.post("/ask")
def ask(request: QueryRequest):
    prompt = request.prompt
    schema = get_schema()
    print("SQL Data Retrieved")

    ollama_payload = {
        "model": "qwen3:4b",
        "prompt": f"You are an expert SQL assistant.\n"
                  f"Database schema:\n{schema}\n"
                  f"Write a valid SQLite query to answer this: {prompt}\n"
                  f"Only return SQL, nothing else.",
        "stream": True
    }
    print("➡️ Sending request to qwen3:4b with prompt:", prompt)
    response = requests.post("http://localhost:11434/api/generate", json=ollama_payload)
    print("⬅️ Received response from tinyllama")
    sql_query = response.json().get("response", "").strip()

    try:
        cur = connection.cursor()
        cur.execute(sql_query)
        rows = cur.fetchall()
        cur.close()
        results = [list(row) for row in rows]
        return {"sql": sql_query, "result": results}
    except Exception as e:
        return {"error": str(e), "sql": sql_query}
'''
    # Executes the generated SQL query on PostgreSQL


@app.post("/ask")
def ask(request: QueryRequest):
    prompt = request.prompt
    schema = get_schema()
    print("SQL Data Retrieved")

    ollama_payload = {
        "model": "qwen3:4b",
        "prompt": f"You are an expert SQL assistant.\n"
                  f"Database schema:\n{schema}\n"
                  f"Write a valid SQLite query to answer this: {prompt}\n"
                  f"Only return SQL, nothing else.",
        "stream": False  # Turn off streaming for easier debugging
    }

    print("➡️ Sending request to qwen3:4b with prompt:", prompt)

    try:
        response = requests.post("http://localhost:11434/api/generate", json=ollama_payload)
        response.raise_for_status()  # Raise an error if response not 200
        print("⬅️ Received response from qwen3:4b")

        # Extract model response
        ai_text = response.json().get("response", "").strip()
        print("🧠 Model output:", ai_text)

        # Just return AI response (skip SQL execution)
        return {"ai_response": ai_text}

    except Exception as e:
        print("❌ Error communicating with Ollama:", str(e))
        return {"error": str(e)}

    
    
# To run the app, use the command: uvicorn FastAPIapp:app --reload
# Make sure to replace "your_username" and "your_password" with your actual PostgreSQL credentials.
# After running this, FastAPI will be available at http://127.0.0.1:8000    