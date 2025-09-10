# Fivos-AI-Chat-App

GUI:
How to run this code:
Ensure you have .NET Framework installed (version 4.8 or compatible).
Create a new Windows Forms App project in Visual Studio.
Ensure you have the Newtonsoft.Json package installed via NuGet.
Tools > NuGet Package Manager > Manage NuGet Packages for solution > Install-Package Newtonsoft.Json
Replace the auto-generated code in Form1.cs with the code above.
Build and run the project.

How it works:
The application creates a basic GUI with a chat display, an input box, and a send button.
When the user types a message and clicks "Send", the message is displayed in the chat box.
WinForms sends the request to the FastAPI/ask endpoint
The FastAPI sends the request to the Ollama (AI model) and gets a SQL query as a response.
It runs the SQL query on PostgreSQL and gets the results.
The results are sent back to the FastAPI and then to the WinForms app and displayed in the chat box.


FastAPI:
To run the app, use the command: uvicorn FastAPIapp:app --reload
Make sure to replace "your_username" and "your_password" with your actual PostgreSQL credentials.
After running this, FastAPI will be available at http://127.0.0.1:8000 
