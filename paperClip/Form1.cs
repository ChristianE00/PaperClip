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
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace paperClip
{
    public partial class Form1 : Form
    {
        private SharpClipboard clipboard;
        private FlowLayoutPanel flowLayoutPanel;
        private ArrayList textBoxList = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            this.Resize += new EventHandler(Form1_Resize); // Add this line
            this.BackColor = Color.FromArgb(28, 28 , 28); // Dark background
            this.Padding = new Padding(10);
            //this.FormBorderStyle = FormBorderStyle.None; // No border

            // Initialize SharpClipboard
            clipboard = new SharpClipboard();
            clipboard.ClipboardChanged += ClipboardChanged;

            // Initialize FlowLayoutPanel
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.WrapContents = false;
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.Dock = DockStyle.Fill; // Make the panel fill the form
            flowLayoutPanel.Padding = new Padding(10); // Add padding to the panel
            flowLayoutPanel.BackColor = Color.FromArgb(30, 30, 30); // Dark background
            this.Controls.Add(flowLayoutPanel);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            foreach (Control control in flowLayoutPanel.Controls)
            {
                if (control is ClipboardItemControl itemControl)
                {
                    itemControl.flowLayoutPanelWidth = flowLayoutPanel.Width;
                    itemControl.flowLayoutPanelHeight = flowLayoutPanel.Height;
                    itemControl.SetTextBoxWidth(itemControl.text); // Adjust the width as needed
                    itemControl.setTextBoxHeight(itemControl.text);
                }
            }
            flowLayoutPanel.PerformLayout();
        }


        private void AddClipboardItemControl(string text)
        {
            var itemControl = new ClipboardItemControl(text, flowLayoutPanel.ClientSize.Height, flowLayoutPanel.ClientSize.Width);

            flowLayoutPanel.Controls.Add(itemControl);

            // Force the FlowLayoutPanel to update its layout
            flowLayoutPanel.PerformLayout();

            // Scroll to the latest item
            flowLayoutPanel.ScrollControlIntoView(itemControl);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Load the first textbox
            string clipboardText = Clipboard.GetText();

        }


        private void ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            if (e.ContentType == SharpClipboard.ContentTypes.Text)
            {
                string clipboardText = clipboard.ClipboardText;
                AddClipboardItemControl(clipboardText);
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



    }
