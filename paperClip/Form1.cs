using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.SharpClipboardNS;
using System.Data.SQLite;
using System.IO;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;
using System.Drawing.Text;

namespace paperClip
{
    public partial class Form1 : Form
    {
        private SharpClipboard _clipboard;
        private FlowLayoutPanel _flowLayoutPanel;
        private List<string> _textBoxList;
        private DatabaseHelper _dbHelper;
        private int _clipboardCount;

        public Form1()
        {

            _clipboardCount = -1;
            InitializeComponent();

            // Load database
            _dbHelper = new DatabaseHelper("ClipboardHistory.db");
            _dbHelper.CreateDatabase();
            _dbHelper.CreateTables();
            
            this.Load += new EventHandler(Form1_Load);
            this.Resize += new EventHandler(Form1_Resize); // Add this line
            this.BackColor = Color.FromArgb(28, 28, 28); // Dark background
            this.Padding = new Padding(10);
            //this.FormBorderStyle = FormBorderStyle.None; // No border

            // Initialize SharpClipboard
            _clipboard = new SharpClipboard();
            _clipboard.ClipboardChanged += ClipboardChanged;

            // Initialize FlowLayoutPanel
            _flowLayoutPanel = new FlowLayoutPanel();
            _flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            _flowLayoutPanel.WrapContents = false;
            _flowLayoutPanel.AutoScroll = true;
            _flowLayoutPanel.Dock = DockStyle.Fill; // Make the panel fill the form
            _flowLayoutPanel.Padding = new Padding(10); // Add padding to the panel
            _flowLayoutPanel.BackColor = Color.FromArgb(30, 30, 30); // Dark background
            this.Controls.Add(_flowLayoutPanel);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            foreach (Control control in _flowLayoutPanel.Controls)
            {
                if (control is ClipboardItemControl itemControl)
                {
                    itemControl.flowLayoutPanelWidth = _flowLayoutPanel.Width;
                    itemControl.flowLayoutPanelHeight = _flowLayoutPanel.Height;
                    itemControl.SetTextBoxWidth(itemControl.text); // Adjust the width as needed
                    itemControl.setTextBoxHeight(itemControl.text);
                }
            }
            _flowLayoutPanel.PerformLayout();
        }


        private void AddClipboardItemControl(string text, bool isCache)
        {
            if (!isCache)
            {
                // NOTE: Position `1` is for testing, this will be updated later
                _dbHelper.InsertClipboardText(text, 1);
            }

            var itemControl = new ClipboardItemControl(text, _flowLayoutPanel.ClientSize.Height, _flowLayoutPanel.ClientSize.Width);

            _flowLayoutPanel.Controls.Add(itemControl);

            // Force the FlowLayoutPanel to update its layout
            _flowLayoutPanel.PerformLayout();

            // Scroll to the latest item
            _flowLayoutPanel.ScrollControlIntoView(itemControl);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Load the first textbox
           // string clipboardText = Clipboard.GetText();
           _textBoxList = _dbHelper.GetClipboardText();
           foreach (string clipboard in _textBoxList)
           {
               AddClipboardItemControl(clipboard, true);
           }
        }


        private void ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            ++_clipboardCount;
            if (e.ContentType == SharpClipboard.ContentTypes.Text && _clipboardCount > 0)
            {
                string clipboardText = _clipboard.ClipboardText;
                AddClipboardItemControl(clipboardText, false);
            }
        }


    }


    // Custom
    public class ClipboardItemControl : UserControl
    {
        public TextBox txtBox;
        public int flowLayoutPanelHeight;
        public int flowLayoutPanelWidth;
        public string text;

        public ClipboardItemControl(string t, int FLPHeight, int FLPWidth)
        {
            text = t.Trim(); // Trim any leading or trailing whitespace/newline characters
            flowLayoutPanelHeight = FLPHeight;
            flowLayoutPanelWidth = FLPWidth;

            // Set control styles for smooth, flicker-free drawing
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            // Set basic properties
            this.BackColor = Color.FromArgb(50, 50, 50);  // Dark background
            this.Font = new Font("Segoe UI", 12);
            this.Padding = new Padding(10);
            SetTextBoxWidth(text);
            setTextBoxHeight(text);

            // Initialize the TextBox to display the clipboard text
            txtBox = new TextBox();
            txtBox.Multiline = true;
            txtBox.ReadOnly = true;
            txtBox.BorderStyle = BorderStyle.None;
            txtBox.Dock = DockStyle.Fill;
            txtBox.ForeColor = Color.White;
            txtBox.BackColor = Color.FromArgb(50, 50, 50); // Match the background color of the UserControl
            txtBox.ScrollBars = ScrollBars.Vertical;
            txtBox.WordWrap = true;
            txtBox.Text = text; // Set the text once

            // Add the TextBox to the control
            this.Controls.Add(txtBox);
            //this.setVScroll();

            // Add hover effect for both the UserControl and the TextBox
            this.MouseEnter += (s, e) => this.BackColor = Color.FromArgb(70, 70, 70);
            this.MouseLeave += (s, e) => this.BackColor = Color.FromArgb(50, 50, 50);
            txtBox.MouseEnter += (s, e) => this.BackColor = Color.FromArgb(70, 70, 70);
            txtBox.MouseLeave += (s, e) => this.BackColor = Color.FromArgb(50, 50, 50);

            // Center the control within the FlowLayoutPanel
            this.Anchor = AnchorStyles.Top;
            CenterControl();
        }

        private int GetLineCount()
        {
            return txtBox.GetLineFromCharIndex(txtBox.TextLength) + 1;
        }

        private void setVScroll()
        {
            int count = GetLineCount();
            if (count > 2)
            {
                txtBox.ScrollBars = ScrollBars.Vertical;
            }
            else
            {
                txtBox.ScrollBars = ScrollBars.None;
            }
        }

        public void SetTextBoxWidth(string text)
        {
            this.Width = flowLayoutPanelWidth - (int)(flowLayoutPanelWidth * 0.1);
        }

        public void setTextBoxHeight(string text)
        {
            // Calculate the height of the text
            int textHeight = TextRenderer.MeasureText(text, this.Font, new Size(this.Width, 0),
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;

            // Determine the appropriate height for the control
            textHeight = (textHeight > this.flowLayoutPanelHeight) ? this.flowLayoutPanelHeight : (textHeight + 20) + (int)(textHeight * .2);

            // Set the height of the control
            this.Height = textHeight;
        }

        private void CenterControl()
        {
            int leftMargin = (flowLayoutPanelWidth - this.Width) / 2;
            this.Margin = new Padding(leftMargin, 10, 0, 10);
        }
    }

    // SQLite
    public class DatabaseHelper
    {
        private string _dbFilePath;

        public DatabaseHelper(string dbName)
        {
            _dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);

        }

        public void CreateDatabase()
        {
            if (!File.Exists(_dbFilePath))
            {
                SQLiteConnection.CreateFile(_dbFilePath);
            }
        }

        public void CreateTables()
        {
            string connectionString = $"Data Source={_dbFilePath};version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS ClipboardHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClipboardText TEXT NOT NULL,
                        DateTimeCopied DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Position INTEGER NOT NULL
                    )";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();

                }
            }

        }
        // A method to insert clipboard text into the database
        public void InsertClipboardText(string clipboardTest, int position)
        {
            string connectionString = $"Data Source={_dbFilePath};version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO ClipboardHistory (ClipboardText, DateTimeCopied, Position)
                    VALUES (@ClipboardText, @DateTimeCopied, @Position)
                ";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClipboardText", clipboardTest);
                    command.Parameters.AddWithValue("@DateTimeCopied", DateTime.Now);
                    command.Parameters.AddWithValue("@Position", position);

                    command.ExecuteNonQuery();
                }
            }

        }
        
        // A method to get all clipboard text from the "ClipboardHistory" table
        public List<string> GetClipboardText()
        {
            List<string> clipboardTexts = new List<string>();
            string connectionString = $"Data Source={_dbFilePath};version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = @"
                    SELECT ClipboardText
                    FROM ClipboardHistory
                    ORDER BY DateTimeCopied DESC
                ";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clipboardTexts.Add(reader["ClipboardText"].ToString());
                        }
                    }
                }
            }

            return clipboardTexts;
        }
        
    }

}
