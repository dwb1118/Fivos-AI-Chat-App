from fastapi import FastAPI
from pydantic import BaseModel
import requests

app = FastAPI()

# Requests schema
class QueryRequest(BaseModel):
    prompt: str

@app.post("/ask")
def ask(request: QueryRequest):
    try:
        # Sends the prompt directly to Ollama
        ollama_payload = {
            "model": "tinyllama",   # change to the model you installed (tinyllama, gemma, qwen, etc.)
            "prompt": request.prompt,
            "stream": False
        }

        response = requests.post("http://localhost:11434/api/generate", json=ollama_payload)
        response.raise_for_status()  # throws if error

        # Parses Ollama response
        ollama_response = response.json().get("response", "").strip()

        return {"prompt": request.prompt, "response": ollama_response}

    except Exception as e:
        return {"error": str(e)}


# How to run the app:    
# Ensure you have FastAPI, Uvicorn and Pydantic installed:
# In the console: python -m pip install fastapi pydantic requests
# In the console: python -m pip install "uvicorn[standard]"
# In the console: python -m pip install fastapi uvicorn requests 
# Ensure you have SQLite database named "ChatAppDB.db" in the same directory.
# Ensure you have Ollama running locally with the TinyLlama model:
# In the console: ollama serve
# To run the app, use the command: uvicorn FastAPIapp:app --reload
# After running this, FastAPI will be available at http://127.0.0.1:8000   