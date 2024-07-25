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
            this.comboBox1.SelectedIndex = Profiles.IndexOf(Settings.Default.ActiveProfile);
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
            QuickLaunch = false;
            Application.Exit();
        }
    }
}
