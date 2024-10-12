using System;
using System.Collections;
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
        //private TextBox clipboardTextBox;
        private FlowLayoutPanel flowLayoutPanel;
        private ArrayList textBoxList = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            Console.WriteLine("Form1");
            this.Load += new EventHandler(Form1_Load);

            // Initialize SharpClipboard
            clipboard = new SharpClipboard();
            clipboard.ClipboardChanged += ClipboardChanged;

            // Initialize FlowLayoutPanel
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.WrapContents = false;
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.Dock = DockStyle.Fill; // Make the panel fill the form
            this.Controls.Add(flowLayoutPanel);
            
        }
        private void CreateNewTextBox(string text)
        {
            TextBox tBox = new TextBox();
            tBox.ReadOnly = true;
            tBox.BorderStyle = BorderStyle.None;
            tBox.Multiline = true;
            tBox.WordWrap = true;
            tBox.ScrollBars = ScrollBars.Vertical;
            tBox.Width = flowLayoutPanel.ClientSize.Width - 20; // Adjust the width as needed
            tBox.Height = generateTextBoxHeight(text, tBox); // Set a fixed height for the TextBox
            tBox.Text = text;
            tBox.BackColor = Color.Gray;
            flowLayoutPanel.Controls.Add(tBox);
            textBoxList.Add(tBox);
            flowLayoutPanel.Controls.Add(tBox);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load the first textbox
            string clipboardText = Clipboard.GetText();

        }

        private void ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            // NOTE: This is not safe for non-text clipboard data
            if (e.ContentType == SharpClipboard.ContentTypes.Text)
            {
                string clipboardText = clipboard.ClipboardText;
                CreateNewTextBox(clipboardText);             
            }
        }

        private int generateTextBoxHeight(string text, TextBox box)
        {
            int height = TextRenderer.MeasureText(text, box.Font, new Size(box.Width, 0),
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;
            height = (height > flowLayoutPanel.ClientSize.Height) ? flowLayoutPanel.ClientSize.Height - 10 : height;
            return height;
        }

        /*
        private void Form1_Resize(object sender, EventArgs e)
        {
            clipboardTextBox.Width = flowLayoutPanel.ClientSize.Width - 20;
        }
        */
    }
}
