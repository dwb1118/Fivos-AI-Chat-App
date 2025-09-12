using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FivosChatbotGUI
{
    public partial class Form1 : Form
    {


        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();

            // Forms properties
            this.Text = "AI Chat Query App";
            this.Size = new System.Drawing.Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Heading labels
            Label headingLabel = new Label();
            headingLabel.Text = "AI Chat Query Interface";
            headingLabel.Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold); // Bigger font
            headingLabel.AutoSize = true;
            headingLabel.Location = new System.Drawing.Point(10, 10); // Position at top-left
            this.Controls.Add(headingLabel);


            // Chat display box
            chatBox = new RichTextBox();
            chatBox.Location = new System.Drawing.Point(10, 50);
            chatBox.Font = new System.Drawing.Font("Arial", 14);
            chatBox.Size = new System.Drawing.Size(460, 400);
            chatBox.ReadOnly = true;
            chatBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.Controls.Add(chatBox);

            // Input box
            inputBox = new TextBox();
            inputBox.Location = new System.Drawing.Point(10, 480);
            inputBox.Size = new System.Drawing.Size(360, 40);
            inputBox.Font = new System.Drawing.Font("Segoe UI", 12);
            this.Controls.Add(inputBox);
            inputBox.ReadOnly = false;

            // Send button
            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Location = new System.Drawing.Point(380, 478);
            sendButton.Size = new System.Drawing.Size(90, 30);
            sendButton.Click += new EventHandler(SendButton_Click);
            this.Controls.Add(sendButton);
        }


        private async void SendButton_Click(object sender, EventArgs e)
        {
            string userMessage = inputBox.Text.Trim();

            if (!string.IsNullOrEmpty(userMessage))
            {
                // Displays user message
                chatBox.AppendText("User: " + userMessage + "\n");


                // Calls FastAPI
                try
                {

                    // Send request to FastAPI
                    var payload = new { prompt = userMessage };
                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("http://127.0.0.1:8000/ask", content);
                    string aiResponse = await response.Content.ReadAsStringAsync();
                    chatBox.AppendText("System: " + aiResponse + "\n");

                }
                catch (Exception ex)
                {
                    chatBox.AppendText($"System Error: {ex.Message}\n");
                }

                // Clears input
                inputBox.Clear();
            }
        }
    }
}


// How to run this code:
// 1. Ensure you have .NET Framework installed (version 4.8 or compatible).
// 2. Create a new Windows Forms App project in Visual Studio.
// 3. Ensure you have the Newtonsoft.Json package installed via NuGet.
// 5. Tools > NuGet Package Manager > Manage NuGet Packages for solution > Install-Package Newtonsoft.Json
// 4. Replace the auto-generated code in Form1.cs with the code above.
// 5. Build and run the project.

// How it works:
// 1. The application creates a basic GUI with a chat display, an input box, and a send button.
// 2. When the user types a message and clicks "Send", the message is displayed in the chat box.
// 3. WinForms sends the request to the FastAPI/ask endpoint
// 4. The FastAPI sends the request to the Ollama (AI model) and gets a SQL query as a response.
// 5. It runs the SQL query on PostgreSQL and gets the results.
// 6. The results are sent back to the FastAPI and then to the WinForms app and displayed in the chat box.
// 

