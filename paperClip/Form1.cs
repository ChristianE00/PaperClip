using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.SharpClipboardNS;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace paperClip
{
    public partial class Form1 : Form
    {
        private SharpClipboard clipboard;
        private TextBox clipboardTextBox;
        private Panel scrollablePanel;

        public Form1()
        {
            InitializeComponent();
            Console.WriteLine("Form1");
            this.Load += new EventHandler(Form1_Load);

            // Initialize SharpClipboard
            clipboard = new SharpClipboard();
            clipboard.ClipboardChanged += ClipboardChanged;

            // Initialize Panel
            scrollablePanel = new Panel();
            scrollablePanel.AutoScroll = true;
            scrollablePanel.Dock = DockStyle.Fill; // Make the panel fill the form
            this.Controls.Add(scrollablePanel);

            // Initialize TextBox
            clipboardTextBox = new TextBox();
            clipboardTextBox.ReadOnly = true;
            clipboardTextBox.BorderStyle = BorderStyle.None;
            clipboardTextBox.Multiline = true;
            clipboardTextBox.WordWrap = true;
            clipboardTextBox.ScrollBars = ScrollBars.Vertical;
            clipboardTextBox.Location = new Point(10, 10); // Adjust the location as needed
            clipboardTextBox.Width = scrollablePanel.Width - 20; // Adjust the width as needed
            clipboardTextBox.Height = scrollablePanel.Height - 20; // Adjust the height as needed
            scrollablePanel.Controls.Add(clipboardTextBox);

            // Handle form resize to adjust TextBox size
            this.Resize += new EventHandler(Form1_Resize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();
            clipboardTextBox.Text = "Your current clipboard item: " + clipboardText;
        }

        private void ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        // NOTE: This not safe for non-text clipboard data
        
        //if (e.ContentType == SharpClipboard.ContentTypes.Text)
        //{
                string clipboardText = clipboard.ClipboardText;
                Console.WriteLine("Clipboard Text: " + clipboardText);
                clipboardTextBox.Text = "Clipboard Text: " + clipboardText;
            //}
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            clipboardTextBox.Width = scrollablePanel.Width - 20;
            clipboardTextBox.Height = scrollablePanel.Height - 20;
        }
    }
}
