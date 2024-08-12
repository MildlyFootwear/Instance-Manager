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
        List<Button> deleteButtonList = new List<Button>();
        List<Button> argsButtonList = new List<Button>();
        List<Button> duplicateButtonList = new List<Button>();
        ToolTip toolTip = new ToolTip();

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
                        ProfileExes.Add(ProfileExes[rowfun]);
                        SaveProfileExes();
                        RefreshExes();
                    }

                    void LaunchArgs(object sender, EventArgs e)
                    {
                        string exe = ProfileExes[rowfun];
                        Console.WriteLine("\nExecuting Method: LaunchArgs in ManageExes");
                        Console.WriteLine("Modifying args for "+exe);
                        string[] splitexe = exe.Split("|");

                        Form TextIn = new TextInput();
                        TextIn.Text = "Arguments for " + Path.GetFileName(splitexe[1]);
                        PassedString = splitexe[2];
                        TextIn.ShowDialog();
                        if (TextInputString != splitexe[2])
                        {
                            ProfileExes[rowfun] = splitexe[0] + "|" + splitexe[1] + "|" + TextInputString;
                            Console.WriteLine("Modified to " + ProfileExes[rowfun]);
                            SaveProfileExes();
                            RefreshExes();
                        }

                    }

                    void RemoveExe(object sender, EventArgs e)
                    {
                        string[] exe = ProfileExes[rowfun].Split("|");
                        string msg;
                        if (exe[2].Length ==  0)
                            msg = "Remove exeutable \"" + exe[0] + "\" from profile " + Settings.Default.ActiveProfile + "?";
                        else
                            msg = "Remove exeutable \"" + exe[0] +" " + exe[2] + "\" from profile " + Settings.Default.ActiveProfile + "?";

                        if (MessageBox.Show(msg, "Remove Executable", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ProfileExes.RemoveAt(rowfun);
                            SaveProfileExes();
                            RefreshExes();
                        }
                    }

                    void DisplayPath(object sender, EventArgs e)
                    {
                        int index = rowfun;
                        string[] split = ProfileExes[rowfun].Split("|");
                        string path = split[1] + " " + split[2];
                        toolTip.SetToolTip(ExeLabel, path);
                    }

                    remove.Click += RemoveExe;
                    commandargs.Click += LaunchArgs;
                    dupe.Click += DuplicateExe;
                    ExeLabel.MouseEnter += DisplayPath;

                    exelabels.Add(ExeLabel);
                    deleteButtonList.Add(remove);
                    argsButtonList.Add(commandargs);
                    duplicateButtonList.Add(dupe);

                }

                exelabels[row].Text = ProfileExes[row].Split("|")[0];

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
                Console.WriteLine("Cleaning up " + exelabels[temp]);
                tableLayoutPanel1.Controls.Remove(exelabels[temp]);
                tableLayoutPanel1.Controls.Remove(deleteButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(duplicateButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(argsButtonList[temp]);
                temp++;
            }
            Console.WriteLine("temp is " + temp + " row count is " + tableLayoutPanel1.RowCount + " and label count is " + exelabels.Count);

            tableLayoutPanel1.ResumeLayout();

            Thread.Sleep(50);

            tableLayoutPanel1.RowCount = row;

        }

        private void Remove_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ManageExes_Load(object sender, EventArgs e)
        {
            RefreshExes();
            tableLayoutPanel1.RowCount = 0;
            void Close(object sender, EventArgs e)
            {
                this.Close();
            }

            Button cancel = new();
            cancel.Click += Close;
            this.CancelButton = cancel;
            CenterToParent();
        }

        private void addExe_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening add EXE prompt.");
            ExeBrowserDialog.Filter = "Executables|*.exe";
            if (ExeBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                AmendExe(InsertVariables(ExeBrowserDialog.FileName));
                RefreshExes();
                this.CenterToParent();
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

        private void ManageExes_Resize_1(object sender, EventArgs e)
        {
        }
    }
}
