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
    public partial class ManageExes : Form
    {
        public ManageExes()
        {
            InitializeComponent();
        }

        List<Label> exelabels = new List<Label>();
        bool tableresized = false;
        bool windowresized = false;

        void RefreshExes()
        {
            int row = 0;
            tableLayoutPanel1.Controls.Clear();
            exelabels.Clear();
            foreach (string exe in ProfileExes)
            {

                Label ExeLabel = new();
                string[] splitexe = exe.Split(";");
                ExeLabel.Text = splitexe[0];
                ExeLabel.AutoSize = true;
                ExeLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
                ExeLabel.TextAlign = ContentAlignment.MiddleLeft;
                Button dupe = new();
                Button commandargs = new();
                Button remove = new();
                dupe.Text = "Duplicate";
                commandargs.Text = "Launch Arguments";
                remove.Text = "Remove";
                dupe.AutoSize = true;
                commandargs.AutoSize = true;
                remove.AutoSize = true;
                int thisrow = row;
                void DuplicateExe(object sender, EventArgs e)
                {
                    AmendExe(exe);
                    RefreshExes();
                }
                void LaunchArgs(object sender, EventArgs e)
                {
                    Form TextIn = new TextInput();
                    TextIn.Text = "Arguments for "+Path.GetFileName(splitexe[0]);
                    if (exe.IndexOf(";") > -1)
                        PassedString = splitexe[1];
                    TextIn.ShowDialog();
                    if (exe.IndexOf(";") > -1)
                    {
                        if (TextInputString != splitexe[1])
                        {
                            ProfileExes[thisrow] = splitexe[0] + ";" + TextInputString;
                            SaveProfileExes();
                            RefreshExes();
                        }
                    } else if (TextInputString != ""){
                        ProfileExes[thisrow] = splitexe[0] + ";" + TextInputString;
                        SaveProfileExes();
                        RefreshExes();
                    }
                    
                }
                void RemoveExe(object sender, EventArgs e)
                {
                    if (MessageBox.Show("Remove exeutable "+exe+" from profile "+Settings.Default.ActiveProfile, "Remove Executable", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ProfileExes.Remove(exe);
                        SaveProfileExes() ;
                        RefreshExes() ;
                    }
                }

                dupe.Click += DuplicateExe;
                commandargs.Click += LaunchArgs;
                remove.Click += RemoveExe;

                tableLayoutPanel1.Controls.Add(ExeLabel, 0, row);
                tableLayoutPanel1.Controls.Add(dupe, 1, row);
                tableLayoutPanel1.Controls.Add(commandargs, 2, row);
                tableLayoutPanel1.Controls.Add(remove, 3, row);
                row++;
            }

        }

        private void Remove_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ManageExes_Load(object sender, EventArgs e)
        {
            RefreshExes();
        }

        private void addExe_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening add EXE prompt.");
            ExeBrowserDialog.Filter = "Executables|*.exe";
            if (ExeBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                AmendExe(InsertVariables(ExeBrowserDialog.FileName));
                RefreshExes();
            }
        }

        private void ManageExes_SizeChanged(object sender, EventArgs e)
        {
        }

        private void tableLayoutPanel1_Resize(object sender, EventArgs e)
        {
            //Console.WriteLine("Resizing window for table.");
            //if (tableresized)
            //{
            //    tableresized = false;
            //}
            //else
            //{
            //    tableresized = true;
            //    this.Width = tableLayoutPanel1.Width + (6 * this.DeviceDpi / 100);
            //    Console.WriteLine(tableLayoutPanel1.Width + " " + this.Width);
            //} 
        }

        private void ManageExes_Resize(object sender, EventArgs e)
        {
            //if (windowresized)
            //{
            //    windowresized = false;
            //}
            //else
            //{
            //    windowresized = true;
            //    this.Width = tableLayoutPanel1.Width + (Convert.ToInt32(Convert.ToDouble(4) * Convert.ToDouble(this.DeviceDpi / 10)));
            //    Console.WriteLine(tableLayoutPanel1.Width + " " + this.Width);
            //}


        }
    }
}
