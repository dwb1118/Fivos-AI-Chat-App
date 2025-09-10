from fastapi import FastAPI
from pydantic import BaseModel
import sqlite3
import requests
import json

app = FastAPI()

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

@app.post("/ask")
def ask(request: QueryRequest):
    prompt = request.prompt
    schema = get_schema()

    ollama_payload = {
        "model": "llama3",
        "prompt": f"You are an expert SQL assistant.\n"
                  f"Database schema:\n{schema}\n"
                  f"Write a valid SQLite query to answer this: {prompt}\n"
                  f"Only return SQL, nothing else.",
        "stream": False
    }
    response = requests.post("http://localhost:11434/api/generate", json=ollama_payload)
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

    # Executes the generated SQL query on PostgreSQL
    
    
# To run the app, use the command: uvicorn FastAPIapp:app --reload
# Make sure to replace "your_username" and "your_password" with your actual PostgreSQL credentials.
# After running this, FastAPI will be available at http://127.0.0.1:8000    