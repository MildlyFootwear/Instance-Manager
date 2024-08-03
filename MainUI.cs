using Instance_Manager.Properties;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Instance_Manager.CommonVars;
using static Instance_Manager.Methods.CommonMethods;
using static Microsoft.VisualBasic.Interaction;
using Microsoft.Win32;

namespace Instance_Manager
{
    public partial class MainUI : Form
    {

        bool JustRefreshedExes = false;
        bool NeedRefresh = false;
        List<Label> sourceLabels = new List<Label>();
        List<Label> destLabels = new List<Label>();
        public MainUI()
        {
            InitializeComponent();
        }

        private void RefreshProfiles()
        {
            Console.WriteLine("\nExecuting Method: RefreshProfiles in MainUI");
            ProfilesBox.Items.Clear();
            foreach (string str in Profiles)
            {
                ProfilesBox.Items.Add(str);
            }
            ProfilesBox.SelectedIndex = ProfilesBox.FindStringExact(Settings.Default.ActiveProfile);
        }


        private void RefreshList()
        {
            tableLayoutPanel1.SuspendLayout();
            RefreshProfiles();
            Console.WriteLine("\nExecuting Method: RefreshList in MainUI");
            if (Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile) == false)
            {
                Settings.Default.ActiveProfile = Profiles[0];
                Console.WriteLine("Set profile not found, setting to " + Profiles[0]);
                ProfilesBox.SelectedIndex = 0;
            }
            this.Text = ToolName + " - " + Settings.Default.ActiveProfile;
            LoadProfileLinks();
            int row = 1;
            foreach (string link in DirectoryLinks)
            {
                string[] splitlink = link.Split(';');
                if (row - 1 == sourceLabels.Count)
                {
                    Label Source = new();
                    Label Destination = new();
                    Source.AutoSize = true;
                    Destination.AutoSize = true;
                    int index = row - 1;
                    void SourceClick(object sender, EventArgs e)
                    {
                        Console.WriteLine();
                        Instance_Manager.Methods.LinkModifierMethods.EditLink(index, false);
                        RefreshList();
                    }
                    void DestinationClick(object sender, EventArgs e)
                    {
                        Console.WriteLine();
                        Instance_Manager.Methods.LinkModifierMethods.EditLink(index, true);
                        RefreshList();
                    }

                    Source.Click += SourceClick;
                    Destination.Click += DestinationClick;
                    sourceLabels.Add(Source);
                    destLabels.Add(Destination);
                    Console.WriteLine("Created labels " + Source.ToString() + " and " + Destination.ToString() + " for row " + row);
                }

                sourceLabels[row - 1].Text = splitlink[0];
                destLabels[row - 1].Text = splitlink[1];

                if (tableLayoutPanel1.GetControlFromPosition(0, row) != sourceLabels[row - 1])
                {
                    tableLayoutPanel1.Controls.Add(sourceLabels[row - 1], 0, row);
                    tableLayoutPanel1.Controls.Add(destLabels[row - 1], 1, row);
                    Console.WriteLine("Added labels " + sourceLabels[row - 1].ToString() + " and " + destLabels[row - 1] + " to table layout at row " + row);
                }
                row++;
            }

            int temp = row - 1;
            while (temp < tableLayoutPanel1.RowCount - 2 && temp < sourceLabels.Count)
            {
                tableLayoutPanel1.Controls.Remove(sourceLabels[temp]);
                tableLayoutPanel1.Controls.Remove(destLabels[temp]);
                Console.WriteLine("Removed labels " + sourceLabels[temp] + " and " + destLabels[temp] + " from table layout panel");
                temp++;
            }
            tableLayoutPanel1.RowCount = row + 1;
            tableLayoutPanel1.ResumeLayout();
            Console.WriteLine("Refreshed lists.");
        }

        void RefreshExes()
        {
            Console.WriteLine("\nExecuting Method: RefreshExes in MainUI.");
            JustRefreshedExes = true;
            LoadProfileExes();
            ExeBox.Items.Clear();
            if (ProfileExes.Count > 0)
            {
                foreach (string str in ProfileExes)
                {
                    string[] split = str.Split(";");
                    ExeBox.Items.Add(Path.GetFileName(split[0]));
                }

                if (ProfileExes.IndexOf(SelectedExe) > -1)
                {
                    ExeBox.SelectedIndex = ProfileExes.IndexOf(SelectedExe);
                    Console.WriteLine(SelectedExe + " found in prof exes, setting exebox index to " + ExeBox.SelectedIndex);
                }
                else
                {
                    ExeBox.SelectedIndex = 0;
                    SelectedExe = ProfileExes[0];
                    Console.WriteLine(SelectedExe + " not found in prof exes.");
                }
            }
            else
            {
                SelectedExe = "";
            }
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            Console.WriteLine("\nExecuting Method: MainUI_Load");
            if (Settings.Default.SavedPosition != new Point(1, 1))
                this.Location = Settings.Default.SavedPosition; Console.WriteLine("Set position to " + this.Location);
            if (Settings.Default.SavedSize != new Size(1, 1))
                this.Size = Settings.Default.SavedSize; Console.WriteLine("Set size to " + this.Size);
            SelectedExe = Settings.Default.SavedExe;

            Label column0Label = new Label();
            Label column1Label = new Label();
            column0Label.Text = "File Save/Load Location";
            column1Label.Text = "Overlay On";
            column0Label.AutoSize = true;
            column1Label.AutoSize = true;
            column0Label.TextAlign = ContentAlignment.TopCenter;
            column1Label.TextAlign = ContentAlignment.TopCenter;
            column0Label.Anchor = AnchorStyles.Top;
            column1Label.Anchor = AnchorStyles.Top;
            tableLayoutPanel1.Controls.Add(column0Label);
            tableLayoutPanel1.Controls.Add(column1Label);

            RefreshList();
            RefreshExes();
        }

        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.SavedPosition = this.Location;
            Settings.Default.SavedSize = this.Size;
            Settings.Default.SavedExe = SelectedExe;
            Settings.Default.Save();
        }

        private void ProfilesBox_SelectedValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("\nExecuting Method: ProfilesBox_SelectedValueChanged");
            string selected = ProfilesBox.SelectedItem.ToString();
            SetProfile(selected);
            RefreshList();
            RefreshExes();

        }

        private void ExeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("\nExecuting Method: ExeBoxSelectedIndexChanged");
            if (JustRefreshedExes)
            {
                JustRefreshedExes = false;
                return;
            }
            if (ExeBox.SelectedIndex != ProfileExes.IndexOf(SelectedExe))
            {

                SelectedExe = ProfileExes[ExeBox.SelectedIndex];
                Console.WriteLine("Selected EXE " + SelectedExe);
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void LinkButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nExecuting Method: LinkButton_Click");
            SourceBrowserDialog.InitialDirectory = envEXELOC;
            if (SourceBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(SourceBrowserDialog.SelectedPath + " chosen as source for link.");
                if (DestinationBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine(DestinationBrowserDialog.SelectedPath + " chosen as destination for link.");
                    DirectoryLinks.Add(InsertVariables(SourceBrowserDialog.SelectedPath + ";" + DestinationBrowserDialog.SelectedPath));
                    SaveProfileLinks();
                    RefreshList();
                }
            }
        }

        private void ProfilesBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripManageProfiles_Click(object sender, EventArgs e)
        {
            string active = Settings.Default.ActiveProfile;
            Console.WriteLine("\nShowing profile manager.\n");
            Form ProfileM = new ProfileManager();
            ProfileM.ShowDialog();
            if (NeedRefresh == true)
            {
                RefreshList();
                RefreshExes();
            }
            else { RefreshProfiles(); }
        }

        private void toolManageVariables_Click(object sender, EventArgs e)
        {
            Form VarM = new ManageVariables();
            VarM.Text = ToolName + " - Manage Variables";
            VarM.ShowDialog();
        }

        private void toolManageExes_Click(object sender, EventArgs e)
        {
            Form ExeM = new ManageExes();
            ExeM.Text = ToolName + " - " + Settings.Default.ActiveProfile + " - Manage Executables";
            ExeM.ShowDialog();
            RefreshExes();
        }

        private void toolHelp_Click(object sender, EventArgs e)
        {
            Form Info = new Info();
            Info.ShowDialog();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            LaunchExe();
        }

        private void SourceBrowserDialog_HelpRequest(object sender, EventArgs e)
        {

        }
    }
}
