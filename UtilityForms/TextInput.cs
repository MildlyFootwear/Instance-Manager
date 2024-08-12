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
    public partial class TextInput : Form
    {
        public TextInput()
        {
            InitializeComponent();
        }

        private void TextInput_Load(object sender, EventArgs e)
        {
            TextInputString = "";
            textBox1.Text = PassedString;
            string s = PassedString;
            PassedString = "";

            void Close(object sender, EventArgs e)
            {
                TextInputString = s;
                this.Close();
            }

            Button cancel = new();
            cancel.Click += Close;
            this.CancelButton = cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Replace(" ", "").Length > 0)
            {
                bool prohibChar = false;
                foreach (string s in ProhibChars)
                {
                    if (textBox1.Text.IndexOf(s) != -1)
                    {
                        prohibChar = true;
                        MessageBox.Show("Character " + s + " can't be input.", ToolName);
                    }
                }
                if (!prohibChar)
                {
                    TextInputString = textBox1.Text;
                    this.Close();
                }
            }
            else {
                TextInputString = textBox1.Text.Replace(" ", "");
                this.Close();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
