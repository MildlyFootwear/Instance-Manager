using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Instance_Manager.UtilityForms
{
    public partial class QuickExe : Form
    {
        public QuickExe()
        {
            InitializeComponent();
        }

        private void QuickExe_Load(object sender, EventArgs e)
        {
            this.Text = ToolName + " - " + Settings.Default.ActiveProfile + " - Select Exe";
            foreach (string str in ProfileExes)
            {
                string[] split = str.Split(";");
                if (str.IndexOf(";") != -1)
                {
                    comboBox1.Items.Add(Path.GetFileName(split[0]) + " " + split[1]);
                }
                else comboBox1.Items.Add(Path.GetFileName(split[0]));
            }

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedExe = ProfileExes[comboBox1.SelectedIndex];
            this.FormClosing -= QuickExe_FormClosing;
            this.Close();
        }

        private void QuickExe_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
