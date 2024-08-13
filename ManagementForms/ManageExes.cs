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
        List<Button> renameButtonList = new List<Button>();
        List<Button> deleteButtonList = new List<Button>();
        List<Button> argsButtonList = new List<Button>();
        List<Button> duplicateButtonList = new List<Button>();
        ToolTip toolTip = new ToolTip();

        void RefreshExes()
        {
            int row = 0;
            foreach (string exe in ProfileExes)
            {
                if (row == exelabels.Count)
                {

                    WriteLineIfDebug("Creating label and buttons, count at " + exelabels.Count);
                    int rowfun = row;
                    Label ExeLabel = new();
                    Button rename = new();
                    Button dupe = new();
                    Button commandargs = new();
                    Button remove = new();
                    rename.Text = "Rename";
                    dupe.Text = "Duplicate";
                    commandargs.Text = "Launch Arguments";
                    remove.Text = "Remove";
                    rename.AutoSize = true;
                    dupe.AutoSize = true;
                    commandargs.AutoSize = true;
                    remove.AutoSize = true;
                    ExeLabel.AutoSize = true;
                    ExeLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
                    ExeLabel.TextAlign = ContentAlignment.MiddleLeft;
                    rename.BackColor = addExe.BackColor;
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
                        WriteLineIfDebug("\nExecuting Method: LaunchArgs in ManageExes");
                        WriteLineIfDebug("Modifying args for "+exe);
                        string[] splitexe = exe.Split("|");

                        Form TextIn = new TextInput();
                        TextIn.Text = "Arguments for " + Path.GetFileName(splitexe[1]);
                        PassedString = splitexe[3];
                        TextIn.ShowDialog();
                        if (TextInputString != splitexe[3])
                        {
                            ProfileExes[rowfun] = splitexe[0] + "|" + splitexe[1] + "|" + splitexe[2]+"|"+ TextInputString;
                            WriteLineIfDebug("Modified to " + ProfileExes[rowfun]);
                            SaveProfileExes();
                            RefreshExes();
                        }

                    }

                    void RemoveExe(object sender, EventArgs e)
                    {
                        string[] exe = ProfileExes[rowfun].Split("|");
                        string msg;
                        if (exe[2].Length ==  0)
                            msg = "Remove executable \"" + exe[0] + "\" from profile " + Settings.Default.ActiveProfile + "?";
                        else
                            msg = "Remove executable \"" + exe[0] +" " + exe[2] + "\" from profile " + Settings.Default.ActiveProfile + "?";

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

                    void Rename(object sender, EventArgs e)
                    {
                        string[] split = ProfileExes[rowfun].Split("|");
                        PassedString = split[0];
                        Form TextIn = new TextInput();
                        TextIn.Text = "Rename " + Path.GetFileName(split[0]);
                        TextIn.ShowDialog();
                        WriteLineIfDebug("Returned string " + TextInputString);
                        if (TextInputString != "" &&  TextInputString != split[0])
                        {
                            WriteLineIfDebug("Setting exe index "+rowfun+" to "+ TextInputString + "|" + split[1] + "|" + split[2]);
                            ProfileExes[rowfun] = TextInputString + "|"+split[1]+"|"+split[2];
                            SaveProfileExes();
                            RefreshExes();
                        }

                    }
                    rename.Click += Rename;
                    remove.Click += RemoveExe;
                    commandargs.Click += LaunchArgs;
                    dupe.Click += DuplicateExe;
                    ExeLabel.MouseEnter += DisplayPath;
                    
                    exelabels.Add(ExeLabel);
                    renameButtonList.Add(rename);
                    deleteButtonList.Add(remove);
                    argsButtonList.Add(commandargs);
                    duplicateButtonList.Add(dupe);

                }

                exelabels[row].Text = ProfileExes[row].Split("|")[0];

                if (tableLayoutPanel1.GetControlFromPosition(0, row) != exelabels[row])
                {
                    tableLayoutPanel1.Controls.Add(exelabels[row], 0, row);
                    tableLayoutPanel1.Controls.Add(renameButtonList[row], 1, row);
                    tableLayoutPanel1.Controls.Add(duplicateButtonList[row], 2, row);
                    tableLayoutPanel1.Controls.Add(argsButtonList[row], 3, row);
                    tableLayoutPanel1.Controls.Add(deleteButtonList[row], 4, row);
                }
                row++;

            }
            int temp = row;
            while (temp < exelabels.Count)
            {
                if (tableLayoutPanel1.Contains(exelabels[temp]) == false)
                    break;
                WriteLineIfDebug("Cleaning up " + exelabels[temp]);
                tableLayoutPanel1.Controls.Remove(exelabels[temp]);
                tableLayoutPanel1.Controls.Remove(renameButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(deleteButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(duplicateButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(argsButtonList[temp]);
                temp++;
            }

            tableLayoutPanel1.RowCount = row;
            WriteLineIfDebug("temp is " + temp + " row count is " + tableLayoutPanel1.RowCount + " and label count is " + exelabels.Count);


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
            WriteLineIfDebug("Opening add EXE prompt.");
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
            //WriteLineIfDebug("Resizing window for table.");
            //if (tableresized)
            //{
            //    tableresized = false;
            //}
            //else
            //{
            //    tableresized = true;
            //    this.Width = tableLayoutPanel1.Width + (6 * this.DeviceDpi / 100);
            //    WriteLineIfDebug(tableLayoutPanel1.Width + " " + this.Width);
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
            //    WriteLineIfDebug(tableLayoutPanel1.Width + " " + this.Width);
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
