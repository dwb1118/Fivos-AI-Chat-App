# 🧠 Fivos AI Chat App

An AI-powered SQL assistant that translates natural language questions into SQL queries using **LangChain**, **Ollama**, and **TinyLlama** — featuring a **C# WinForms GUI** frontend and a **FastAPI** backend.

---

## ⚙️ Project Structure
```
Fivos-AI-Chat-App/
│
├── FastAPI/                     # Python backend
│   ├── FastAPIapp.py
│   ├── venv/                    # Virtual environment (auto-used by script)
│   └── requirements.txt
│
├── GUI/                         # WinForms frontend
│   └── FivosChatbotGUI/
│       └── bin/Debug/FivosChatbotGUI.exe
│
├── SQLiteDB/
│   └── thrombosis_prediction/thrombosis_prediction.sqlite
│
└── Launch_FivosAI.bat           # One-click launcher for teammates
```

---

## 🚀 One-Click Launch (Recommended)

### Step 1️⃣ — Install Ollama and Model
1. [Download Ollama](https://ollama.ai/download) and install it.  
2. (Only once) Open a terminal and pull the model:
   ```bash
   ollama pull tinyllama
   ```

---

### Step 2️⃣ — Launch the Entire App
Just double-click:  
👉 **`Launch_FivosAI.bat`**

This will automatically:
- Start **Ollama** (if it isn’t already running)
- Pull the **TinyLlama** model (if not present)
- Run the **FastAPI backend**
- Launch the **C# GUI**

✅ **Everything runs automatically** — no path edits, no manual activation, no separate terminals needed.  
The script detects its own folder and runs all components relative to it.

---

## 💻 Developer Setup (Manual Run)

If you prefer to run components manually or modify code:

### Backend (FastAPI)
```bash
cd FastAPI
python -m venv venv
venv\Scripts\activate
pip install -r requirements.txt
uvicorn FastAPIapp:app --reload
```

FastAPI will start at  
👉 **http://127.0.0.1:8000**

---

### Frontend (GUI)
1. Open `GUI/FivosChatbotGUI/FivosChatbotGUI.sln` in Visual Studio.  
2. Ensure **.NET Framework 4.8** is installed.  
3. Add **Newtonsoft.Json** from NuGet if not already installed.  
4. Build and run the solution, or launch directly via:
   ```
   GUI/FivosChatbotGUI/bin/Debug/FivosChatbotGUI.exe
   ```

---

## 🧩 How It Works

1. The **user** types a natural language question in the GUI.  
2. The **GUI** sends it to the **FastAPI** backend.  
3. The backend uses **TinyLlama** (via Ollama) to translate the request into SQL.  
4. The query runs on the **SQLite** database.  
5. The backend sends the SQL and summarized results back to the GUI.

---

## 🧪 Example Interaction

**User:**  
> Show all patients from Patient table

**Generated SQL:**  
```sql
SELECT * FROM Patient;
```

**Summary:**  
> Here are all the patients from the patient table.

---

## ⚠️ Troubleshooting

| Issue | Fix |
|-------|-----|
| `❌ Could not connect to FastAPI backend` | Make sure `Launch_FivosAI.bat` ran successfully. |
| `Ollama request timed out` | Reopen Ollama (`ollama serve`) or restart your PC. |
| GUI doesn’t open | Check that .NET Framework 4.8 is installed. |
| Database not found | Ensure the `SQLiteDB` folder is in the same directory as the `.bat` file. |

---

## 🧰 Tech Stack

- **Backend:** Python, FastAPI, LangChain, TinyLlama, Ollama  
- **Frontend:** C# (.NET WinForms), Newtonsoft.Json  
- **Database:** SQLite  
- **Automation:** Windows Batch Script (`Launch_FivosAI.bat`)

---

### 🧩 Tip for Teammates
If you clone the repo anywhere on your desktop, you can just:
> 🖱️ Double-click `Launch_FivosAI.bat` and everything will start.

No configuration needed.
