@echo off
title Fivos AI Chatbot Launcher
color 0A

REM Automatically get the directory where this .bat file is located
set "BASE_DIR=%~dp0"
REM Remove trailing backslash if exists
if "%BASE_DIR:~-1%"=="\" set "BASE_DIR=%BASE_DIR:~0,-1%"

echo ==============================================
echo     Starting Fivos AI Chatbot Environment
echo ==============================================
echo.

REM === Step 1: Start Ollama (AI Model Server) if not running ===
echo [1/4] Launching Ollama Server...
powershell -Command "if (-not (Get-Process ollama -ErrorAction SilentlyContinue)) { Start-Process cmd -ArgumentList '/k ollama serve' }"
timeout /t 5 >nul

REM === Step 2: Pull TinyLlama model (if not already pulled) ===
echo [Optional] Pulling TinyLlama model...
ollama pull tinyllama
timeout /t 3 >nul

REM === Step 3: Activate virtual environment and start FastAPI backend ===
echo [2/4] Starting FastAPI backend in virtual environment...
cd /d "%BASE_DIR%\FastAPI"

REM Activate venv and run FastAPI in a new window
start "FastAPI Backend" cmd /k "call venv\Scripts\activate && uvicorn FastAPIapp:app --reload"
timeout /t 5 >nul

REM === Step 4: Launch GUI ===
echo [3/4] Launching Chat Application...
start "" "%BASE_DIR%\GUI\FivosChatbotGUI\bin\Debug\FivosChatbotGUI.exe"

echo.
echo ==============================================
echo  [4/4] All systems started successfully!
echo ==============================================
timeout /t 4 >nul
exit
