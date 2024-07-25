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
    public partial class QuickProfile : Form
    {
        public QuickProfile()
        {
            InitializeComponent();
        }

        private void QuickProfile_Load(object sender, EventArgs e)
        {
            this.Text = "Select Profile";
            this.comboBox1.DataSource = Profiles;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.ActiveProfile = this.comboBox1.SelectedItem as string;
            Settings.Default.Save();
            this.FormClosing -= QuickProfile_FormClosing;
            this.Close();
        }

        private void QuickProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
