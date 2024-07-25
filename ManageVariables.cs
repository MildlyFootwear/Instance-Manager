using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Instance_Manager.Properties;

namespace Instance_Manager
{
    public partial class ManageVariables : Form
    {
        public ManageVariables()
        {
            InitializeComponent();
        }

        void RefreshVars()
        {
            tableLayoutPanel1.Controls.Clear();
            int row = 0;
            foreach (string s in SystemVariables)
            {
                Label var = new Label();
                Label varval = new Label();
                var.Text = s;
                var.AutoSize = true;
                varval.Text = SystemVariablesValues[SystemVariables.IndexOf(s)];
                varval.AutoSize = true;
                tableLayoutPanel1.Controls.Add(var, 0, row);
                tableLayoutPanel1.Controls.Add(varval, 1, row);
                row++;
            }
            tableLayoutPanel1.Controls.Add(new Label(), 0, row);
        }

        private void ManageVariables_Load(object sender, EventArgs e)
        {
            this.Text = "Instance Manager - " + Settings.Default.ActiveProfile+" - Manage Variables";
            RefreshVars();
        }

    }
}
