using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIChatApp
{
    public class ChatForm : Form
    {
        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;

        public ChatForm()
        {
            // Form properties
            this.Text = "AI Chat Query App";
            this.Size = new System.Drawing.Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Heading label
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

        private void SendButton_Click(object sender, EventArgs e)
        {
            string userMessage = inputBox.Text.Trim();

            if (!string.IsNullOrEmpty(userMessage))
            {
                // Display user message
                chatBox.AppendText("User: " + userMessage + "\n");

                // Simulate AI Response (replace with actual AI call later)
                string aiResponse = "System: (Sample response for '" + userMessage + "')\n";
                chatBox.AppendText(aiResponse);

                // Clear input
                inputBox.Clear();
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new ChatForm());
        }
    }
}