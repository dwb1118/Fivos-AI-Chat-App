::[Bat To Exe Converter]
::
::YAwzoRdxOk+EWAjk
::fBw5plQjdCyDJGyX8VAjFAtXSRSHPWy4B4k45+vu4u+Jtl4hdesccI7P+6SeEPIc4EDnYaoM31lSmd8tHAtobB2hawwglUhLoGuWG8ibvEHoSUfp
::YAwzuBVtJxjWCl3EqQJgSA==
::ZR4luwNxJguZRRnk
::Yhs/ulQjdF+5
::cxAkpRVqdFKZSjk=
::cBs/ulQjdF+5
::ZR41oxFsdFKZSDk=
::eBoioBt6dFKZSDk=
::cRo6pxp7LAbNWATEpCI=
::egkzugNsPRvcWATEpCI=
::dAsiuh18IRvcCxnZtBJQ
::cRYluBh/LU+EWAnk
::YxY4rhs+aU+JeA==
::cxY6rQJ7JhzQF1fEqQJQ
::ZQ05rAF9IBncCkqN+0xwdVs0
::ZQ05rAF9IAHYFVzEqQJQ
::eg0/rx1wNQPfEVWB+kM9LVsJDGQ=
::fBEirQZwNQPfEVWB+kM9LVsJDGQ=
::cRolqwZ3JBvQF1fEqQJQ
::dhA7uBVwLU+EWDk=
::YQ03rBFzNR3SWATElA==
::dhAmsQZ3MwfNWATElA==
::ZQ0/vhVqMQ3MEVWAtB9wSA==
::Zg8zqx1/OA3MEVWAtB9wSA==
::dhA7pRFwIByZRRnk
::Zh4grVQjdCyDJGyX8VAjFAtXSRSHPWy4B4k45+vu4u+Jtl4hdesccI7P+6SeEPIc4EDnYaoM31lSmd8tHAtobB2hawwglUhLoGuWFu6fuw71a1iZqE4oHgU=
::YB416Ek+ZG8=
::
::
::978f952a14a936cc963da21a135fa983
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
