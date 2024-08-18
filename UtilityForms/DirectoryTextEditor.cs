using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Instance_Manager
{
    public partial class DirectoryTextEditor : Form
    {
        public DirectoryTextEditor()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            foreach (string invalid in ProhibPathChars)
            {
                if (s.Contains(invalid))
                {
                    ThreadedMessage("Path can't contain " + invalid + ".");
                    return;
                }
            }
            TextInputString = textBox1.Text;
            if (TextInputString != "")
            {
                if (TextInputString[TextInputString.Length - 1] == '\\' || TextInputString[TextInputString.Length - 1] == '/')
                {
                    TextInputString = TextInputString.Substring(0, TextInputString.Length - 1); WriteLineIfDebug(TextInputString + " has been truncated.");
                }
                this.FormClosing -= DirectoryTextEditor_FormClosing;
            }
            if (TextInputString.Replace(" ", "") == "")
                TextInputString = "Remove";
            this.Close();
        }

        private void DirectoryTextEditor_Load(object sender, EventArgs e)
        {
            textBox1.Text = this.Text;
            string dir = ReplaceVariables(this.Text);
            if (Directory.Exists(dir))
            {
                folderBrowserDialog1.InitialDirectory = dir;
            }
            this.Text = "Edit Path for Directory Link " + this.Text;
            TextInputString = "";

            void Close(object sender, EventArgs e)
            {
                this.Close();
            }

            Button cancel = new();
            cancel.Click += Close;
            this.CancelButton = cancel;
        }

        private void DirectoryTextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            TextInputString = "Cancel";
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = InsertVariables(folderBrowserDialog1.SelectedPath);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            TextInputString = "Remove";
            this.FormClosing -= DirectoryTextEditor_FormClosing;
            this.Close();
        }
    }
}
