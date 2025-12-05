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
using System.Data;
using System.Runtime.InteropServices;


namespace FivosChatbotGUI
{
    public partial class Form1 : Form
    {
        private DataGridView resultsGrid;
        private FlowLayoutPanel chatPanel;



        private static readonly HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(300)
        };



        public Form1()
        {
            InitializeComponent();

            this.Text = "AI Chat Query App";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create the main table layout
            var layout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
            };

            // Define row heights
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));   // Header
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Chat display
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));  // Results grid
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));   // Bottom input
            this.Controls.Add(layout);

            // Heading label
            var headingLabel = new Label()
            {
                Text = "AI Chat Query Interface",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            layout.Controls.Add(headingLabel, 0, 0);

            // Split container for Chat (top) and Results Grid (bottom)
            var chatGridSplitter = new SplitContainer()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterWidth = 8,
                SplitterDistance = 300, // starting height of the chat box
            };

            // ----- Chat Display (TOP PANEL) -----
            chatPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
            };
            chatGridSplitter.Panel1.Controls.Add(chatPanel);

            chatPanel.BorderStyle = BorderStyle.FixedSingle;  // gives a simple black line
            chatPanel.Padding = new Padding(5);               // space between border and text
            chatPanel.Margin = new Padding(10);              // space between chatPanel and splitter edges



            // ----- Results Grid (BOTTOM PANEL) -----
            resultsGrid = new DataGridView()
            {
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            chatGridSplitter.Panel2.Controls.Add(resultsGrid);

            // Add the splitter to your layout instead of the two separate controls
            layout.Controls.Add(chatGridSplitter, 0, 1);
            layout.SetRowSpan(chatGridSplitter, 2);   // The splitter replaces both rows 1 & 2

            resultsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            resultsGrid.RowsDefaultCellStyle.BackColor = Color.White;


            // Bottom input panel
            var bottomPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            layout.Controls.Add(bottomPanel, 0, 3);

            // Input textbox
            inputBox = new TextBox()
            {
                Font = new Font("Segoe UI", 12),
                Left = 10,
                Top = 12,
                Height = 30,
                Width = bottomPanel.Width - 130,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            bottomPanel.Controls.Add(inputBox);

            // Send button
            sendButton = new Button()
            {
                Text = "Send",
                Width = 100,
                Height = 30,
                Left = bottomPanel.Width - 110,
                Top = 12,
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.DarkBlue,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            bottomPanel.Controls.Add(sendButton);
            


            // Adjust input box and button on resize
            bottomPanel.Resize += (s, e) =>
            {
                sendButton.Left = bottomPanel.Width - sendButton.Width - 10;
                inputBox.Width = sendButton.Left - inputBox.Left - 10;
            };


            // Positioning inside bottom panel
            bottomPanel.Resize += (s, e) =>
            {
                sendButton.Left = bottomPanel.Width - sendButton.Width - 10;
                inputBox.Width = sendButton.Left - 20;
            };

            sendButton.Click += SendButton_Click;
            inputBox.KeyDown += InputBox_KeyDown;
        }


        // Add this P/Invoke for rounded corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );


        private void AddMessageLabel(string text, bool isUser = false)
        {
            Label lbl = new Label()
            {
                Text = text,
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                Font = new Font("Segoe UI", 11),
                BackColor = isUser ? Color.LightBlue : Color.LightGray,
                ForeColor = Color.Black,
                Padding = new Padding(5),
                Margin = new Padding(5)
            };

            // Align left/right using FlowDirection trick
            lbl.TextAlign = isUser ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            lbl.Anchor = isUser ? AnchorStyles.Left : AnchorStyles.Left;

            chatPanel.Controls.Add(lbl);
            chatPanel.ScrollControlIntoView(lbl);
        }



        private async void SendButton_Click(object sender, EventArgs e)
        {
            string userMessage = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            // 1️⃣ Display user message in a bubble
            AddMessageLabel(userMessage, true);

            // Clear input immediately
            inputBox.Clear();

            try
            {
                // 2️⃣ Prepare payload for FastAPI
                var payload = new { prompt = userMessage };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 3️⃣ Connection check
                AddMessageLabel("🔌 Checking connection to API...", false);
                try
                {
                    var healthCheck = await client.GetAsync("http://127.0.0.1:8000/ping");
                    if (healthCheck.IsSuccessStatusCode)
                        AddMessageLabel("✅ Connection successful.", false);
                    else
                    {
                        AddMessageLabel($"❌ Could not connect to FastAPI backend. Status: {healthCheck.StatusCode}", false);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    AddMessageLabel($"❌ Could not connect to FastAPI backend. Error: {ex.Message}", false);
                    return;
                }

                // 4️⃣ Send the request
                AddMessageLabel("🤖 Sending request... waiting on tinyllama", false);

                var response = await client.PostAsync("http://127.0.0.1:8000/ask", content);
                response.EnsureSuccessStatusCode();
                string aiResponse = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(aiResponse);

                // 5️⃣ Handle response
                if (parsed["error"] != null)
                {
                    AddMessageLabel("❌ System Error: " + parsed["error"].ToString(), false);
                }
                else
                {
                    string sql = parsed["sql"]?.ToString();
                    AddMessageLabel("System (SQL): " + sql, false);

                    // 6️⃣ Display SQL results in the DataGridView
                    var results = parsed["result"] as JArray;
                    if (results != null && results.Count > 0)
                    {
                        var table = new DataTable();
                        int columnCount = results[0].Count();
                        for (int i = 0; i < columnCount; i++)
                            table.Columns.Add($"Column{i + 1}");

                        foreach (JArray rowArray in results)
                        {
                            DataRow row = table.NewRow();
                            for (int i = 0; i < rowArray.Count; i++)
                                row[i] = rowArray[i]?.ToString() ?? "";
                            table.Rows.Add(row);
                        }
                        resultsGrid.DataSource = table;
                    }
                    else
                    {
                        resultsGrid.DataSource = null;
                    }

                    // 7️⃣ Display summary if available
                    if (parsed.ContainsKey("summary"))
                    {
                        string summary = parsed["summary"]?.ToString();
                        AddMessageLabel("System (Summary): " + (string.IsNullOrEmpty(summary) ? "No summary generated." : summary.Trim()), false);
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessageLabel($"System Error: {ex.Message}", false);
            }

            // 8️⃣ Add separator bubble
            AddMessageLabel("----------------------------------------", false);
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift) // Send on Enter, allow Shift+Enter for new lines
            {
                e.SuppressKeyPress = true; // Prevents the ding sound
                SendButton_Click(this, new EventArgs());
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

