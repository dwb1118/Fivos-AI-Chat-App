# Fivos-AI-Chat-App

GUI:<br>
How to run this code:<br>
1. Ensure you have .NET Framework installed (version 4.8 or compatible).<br>
2. Create a new Windows Forms App project in Visual Studio.<br>
3. Ensure you have the Newtonsoft.Json package installed via NuGet.<br>
4. Tools > NuGet Package Manager > Manage NuGet Packages for solution > Install-Package Newtonsoft.Json<br>
5. Replace the auto-generated code in Form1.cs with the code above.<br>
6. Build and run the project.<br>

How it works:<br>
1. The application creates a basic GUI with a chat display, an input box, and a send button.<br>
2. When the user types a message and clicks "Send", the message is displayed in the chat box.<br>
3. WinForms sends the request to the FastAPI/ask endpoint.<br>
4. The FastAPI sends the request to the Ollama (AI model) and gets a SQL query as a response.<br>
5. It runs the SQL query on PostgreSQL and gets the results.<br>
6. The results are sent back to the FastAPI and then to the WinForms app and displayed in the chat box.<br>
<br>

FastAPI:<br>
1. How to run the app:<br>
2. Ensure you have FastAPI, Uvicorn and Pydantic installed:<br>
3. In the console: python -m pip install fastapi pydantic requests<br>
4. In the console: python -m pip install "uvicorn[standard]"<br>
5. In the console: python -m pip install fastapi uvicorn requests <br>
6. Ensure you have SQLite database named "ChatAppDB.db" in the same directory.<br>
7. Ensure you have Ollama running locally with the TinyLlama model:<br>
8. In the console: ollama serve<br>
9. To run the app, use the command: uvicorn FastAPIapp:app --reload<br>
9. After running this, FastAPI will be available at http://127.0.0.1:8000   <br>
