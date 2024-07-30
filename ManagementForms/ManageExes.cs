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

        bool tableresized = false;
        bool windowresized = false;

        int CreateButtons = 10;
        List<Label> exelabels = new List<Label>();
        List<Button> deleteButtonList = new List<Button>();
        List<Button> argsButtonList = new List<Button>();
        List<Button> duplicateButtonList = new List<Button>();

        void RefreshExes()
        {
            int row = 0;
            tableLayoutPanel1.SuspendLayout();
            foreach (string exe in ProfileExes)
            {
                if (row == exelabels.Count)
                {

                    Console.WriteLine("Creating label and buttons, count at " + exelabels.Count);
                    int rowfun = row;
                    Label ExeLabel = new();
                    Button dupe = new();
                    Button commandargs = new();
                    Button remove = new();
                    dupe.Text = "Duplicate";
                    commandargs.Text = "Launch Arguments";
                    remove.Text = "Remove";
                    dupe.AutoSize = true;
                    commandargs.AutoSize = true;
                    remove.AutoSize = true;
                    ExeLabel.AutoSize = true;
                    ExeLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
                    ExeLabel.TextAlign = ContentAlignment.MiddleLeft;
                    dupe.BackColor = addExe.BackColor;
                    commandargs.BackColor = addExe.BackColor;
                    remove.BackColor = addExe.BackColor;

                    void DuplicateExe(object sender, EventArgs e)
                    {
                        AmendExe(ProfileExes[rowfun]);
                        RefreshExes();
                    }

                    void LaunchArgs(object sender, EventArgs e)
                    {
                        string exe = ProfileExes[rowfun];
                        string[] splitexe = exe.Split(';');

                        Form TextIn = new TextInput();
                        TextIn.Text = "Arguments for " + Path.GetFileName(splitexe[0]);
                        if (exe.IndexOf(";") > -1)
                            PassedString = splitexe[1];
                        TextIn.ShowDialog();
                        if (exe.IndexOf(";") > -1)
                        {
                            if (TextInputString != splitexe[1])
                            {
                                if (TextInputString != "")
                                    ProfileExes[rowfun] = splitexe[0] + ";" + TextInputString;
                                else
                                    ProfileExes[rowfun] = splitexe[0];
                                SaveProfileExes();
                                RefreshExes();
                            }
                        }
                        else if (TextInputString != "")
                        {
                            ProfileExes[rowfun] = splitexe[0] + ";" + TextInputString;
                            SaveProfileExes();
                            RefreshExes();
                        }

                    }

                    void RemoveExe(object sender, EventArgs e)
                    {
                        string exe = ProfileExes[rowfun];
                        if (MessageBox.Show("Remove exeutable \n\"" + exe.Replace(";", " ") + "\" from profile " + Settings.Default.ActiveProfile + "?", "Remove Executable", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ProfileExes.RemoveAt(rowfun);
                            SaveProfileExes();
                            RefreshExes();
                        }
                    }

                    remove.Click += RemoveExe;
                    commandargs.Click += LaunchArgs;
                    dupe.Click += DuplicateExe;

                    exelabels.Add(ExeLabel);
                    deleteButtonList.Add(remove);
                    argsButtonList.Add(commandargs);
                    duplicateButtonList.Add(dupe);

                }

                exelabels[row].Text = ProfileExes[row].Split(';')[0];

                if (tableLayoutPanel1.GetControlFromPosition(0, row) != exelabels[row])
                {
                    tableLayoutPanel1.Controls.Add(exelabels[row], 0, row);
                    tableLayoutPanel1.Controls.Add(duplicateButtonList[row], 1, row);
                    tableLayoutPanel1.Controls.Add(argsButtonList[row], 2, row);
                    tableLayoutPanel1.Controls.Add(deleteButtonList[row], 3, row);
                }
                row++;

            }
            int temp = row;
            while (temp < tableLayoutPanel1.RowCount && temp < exelabels.Count)
            {
                tableLayoutPanel1.Controls.Remove(exelabels[row]);
                tableLayoutPanel1.Controls.Remove(deleteButtonList[row]);
                tableLayoutPanel1.Controls.Remove(duplicateButtonList[row]);
                tableLayoutPanel1.Controls.Remove(argsButtonList[row]);
                temp++;
            }
            tableLayoutPanel1.RowCount = row + 1;
            tableLayoutPanel1.ResumeLayout();

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

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
