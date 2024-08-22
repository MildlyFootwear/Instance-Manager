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
using Instance_Manager.Methods;
using System.Windows.Documents;

namespace Instance_Manager
{
    public partial class MainUI : Form
    {

        bool JustRefreshedExes = false;
        List<Label> sourceLabels = new List<Label>();
        List<Label> destLabels = new List<Label>();
        LaunchMethods lM = new();

        public MainUI()
        {
            InitializeComponent();
        }

        private void RefreshProfiles()
        {
            WriteLineIfDebug("\nExecuting Method: RefreshProfiles in MainUI");
            ProfilesBox.Items.Clear();
            foreach (string str in Profiles)
            {
                ProfilesBox.Items.Add(str);
            }
            ProfilesBox.SelectedIndex = ProfilesBox.FindStringExact(Settings.Default.ActiveProfile);
        }

        void UpdateTitle()
        {
            string s = ToolName + " " + Settings.Default.Version + " - " + Settings.Default.ActiveProfile;
            if (VFSHookedProcesses > 0)
                s += " - " + VFSHookedProcesses + " processes hooked.";
            Text = s;
        }

        private void RefreshList()
        {
            tableLayoutPanel1.SuspendLayout();
            NeedLinkMenuRefresh = false;
            RefreshProfiles();
            WriteLineIfDebug("\nExecuting Method: RefreshList in MainUI");
            if (Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile) == false)
            {
                Settings.Default.ActiveProfile = Profiles[0];
                Settings.Default.Save();
                WriteLineIfDebug("    Set profile not found, setting to " + Profiles[0]);
                ProfilesBox.SelectedIndex = 0;
            }
            UpdateTitle();
            LoadProfileLinks();
            int row = 1;
            foreach (string link in ProfileDirectoryLinks)
            {
                string[] splitlink = link.Split("|");
                if (row - 1 == sourceLabels.Count)
                {
                    Label Source = new();
                    Label Destination = new();
                    Source.AutoSize = true;
                    Destination.AutoSize = true;
                    int index = row - 1;
                    void SourceClick(object sender, EventArgs e)
                    {
                        Instance_Manager.Methods.LinkModifierMethods.EditLink(index, false);
                        if (NeedLinkMenuRefresh) RefreshList();
                    }
                    void DestinationClick(object sender, EventArgs e)
                    {
                        Instance_Manager.Methods.LinkModifierMethods.EditLink(index, true);
                        if (NeedLinkMenuRefresh) RefreshList();
                    }

                    Source.Click += SourceClick;
                    Destination.Click += DestinationClick;
                    sourceLabels.Add(Source);
                    destLabels.Add(Destination);
                    WriteLineIfDebug("    Created labels " + Source.ToString() + " and " + Destination.ToString() + " for row " + row);
                }

                sourceLabels[row - 1].Text = splitlink[0];
                destLabels[row - 1].Text = splitlink[1];

                if (tableLayoutPanel1.GetControlFromPosition(0, row) != sourceLabels[row - 1])
                {
                    tableLayoutPanel1.Controls.Add(sourceLabels[row - 1], 0, row);
                    tableLayoutPanel1.Controls.Add(destLabels[row - 1], 1, row);
                    //WriteLineIfDebug("Added labels " + sourceLabels[row - 1].ToString() + " and " + destLabels[row - 1] + " to table layout at row " + row);
                }
                row++;
            }

            int temp = row - 1;
            while (temp < sourceLabels.Count)
            {
                if (!tableLayoutPanel1.Contains(sourceLabels[temp]))
                    break;
                tableLayoutPanel1.Controls.Remove(sourceLabels[temp]);
                tableLayoutPanel1.Controls.Remove(destLabels[temp]);
                WriteLineIfDebug("    Removed labels " + sourceLabels[temp] + " and " + destLabels[temp] + " from table layout panel");
                temp++;
            }
            tableLayoutPanel1.ResumeLayout();
            tableLayoutPanel1.RowCount = row + 1;
            WriteLineIfDebug("    Refreshed lists.");
        }

        void RefreshExes()
        {
            WriteLineIfDebug("\nExecuting Method: RefreshExes in MainUI.");
            JustRefreshedExes = true;
            LoadProfileExes();
            ExeBox.Items.Clear();
            if (ProfileExes.Count > 0)
            {
                foreach (string str in ProfileExes)
                {
                    string[] split = str.Split("|");
                    ExeBox.Items.Add(split[0]);
                }

                Thread.Sleep(50);

                if (ProfileExes.IndexOf(SelectedExe) > -1)
                {
                    ExeBox.SelectedIndex = ProfileExes.IndexOf(SelectedExe);
                    WriteLineIfDebug("    " + SelectedExe + " found in prof exes, setting exebox index to " + ExeBox.SelectedIndex);
                }
                else
                {
                    ExeBox.SelectedIndex = 0;
                    SelectedExe = ProfileExes[0];
                    WriteLineIfDebug("    " + SelectedExe + " not found in prof exes.");
                }
            }
            else
            {
                SelectedExe = "";
            }
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            WriteLineIfDebug("\nExecuting Method: MainUI_Load in MainUI");
            if (Settings.Default.SavedPosition != new Point(1, 1))
                this.Location = Settings.Default.SavedPosition; WriteLineIfDebug("    Set position to " + this.Location);
            if (Settings.Default.SavedSize != new Size(1, 1))
                this.Size = Settings.Default.SavedSize; WriteLineIfDebug("    Set size to " + this.Size);
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
            CommonVars.Closing = true;
            Settings.Default.SavedPosition = this.Location;
            Settings.Default.SavedSize = this.Size;
            Settings.Default.SavedExe = SelectedExe;
            Settings.Default.Save();
        }

        private void ProfilesBox_SelectedValueChanged(object sender, EventArgs e)
        {
            WriteLineIfDebug("\nExecuting Method: ProfilesBox_SelectedValueChanged in MainUI");
            string selected = ProfilesBox.SelectedItem.ToString();
            SetProfile(selected);
            RefreshList();
            RefreshExes();

        }

        private void ExeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            WriteLineIfDebug("\nExecuting Method: ExeBoxSelectedIndexChanged in MainUI");
            if (JustRefreshedExes)
            {
                JustRefreshedExes = false;
                return;
            }
            if (ExeBox.SelectedIndex != ProfileExes.IndexOf(SelectedExe))
            {

                SelectedExe = ProfileExes[ExeBox.SelectedIndex];
                WriteLineIfDebug("    Selected EXE " + SelectedExe);
                Settings.Default.Save();

            }
        }

        private void LinkButton_Click(object sender, EventArgs e)
        {
            WriteLineIfDebug("\nExecuting Method: LinkButton_Click in MainUI");
            SourceBrowserDialog.InitialDirectory = envAPPLOC;
            if (SourceBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                WriteLineIfDebug("    " + SourceBrowserDialog.SelectedPath + " chosen as source for link.");
                if (DestinationBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    WriteLineIfDebug("    " + DestinationBrowserDialog.SelectedPath + " chosen as destination for link.");
                    ProfileDirectoryLinks.Add(InsertVariables(SourceBrowserDialog.SelectedPath + "|" + DestinationBrowserDialog.SelectedPath));
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
            WriteLineIfDebug("\nShowing profile manager.\n");
            Form ProfileM = new ManageProfiles();
            ProfileM.ShowDialog();
            if (NeedSelectedProfileRefresh == true)
            {
                WriteLineIfDebug("\nSelected profile refreshing.");
                RefreshList();
                RefreshExes();
                NeedSelectedProfileRefresh = false;
            }
            else { WriteLineIfDebug("\nSelected profile not refreshing."); RefreshProfiles(); }
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

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            WriteLineIfDebug("\nExecuting Method: buttonLaunch_Click in MainUI");
            if (lM.LaunchExe())
            {

                Task UpTitle = new Task(() =>
                {
                    WriteLineIfDebug("UpTitle task started");
                    int lastHooked = 0;
                    while (VFSInitializing)
                        Thread.Sleep(100);
                    while (VFSActive)
                    {
                        if (CommonVars.Closing)
                            break;
                        if (lastHooked != VFSHookedProcesses)
                        {
                            lastHooked = VFSHookedProcesses;
                            WriteLineIfDebug("upTitle lastHooked updated to " + lastHooked);
                            this.Invoke(new Action((UpdateTitle)));

                        }
                        Thread.Sleep(100);
                    }
                    WriteLineIfDebug("UpTitle task ended");
                });
                UpTitle.Start();

            }
        }

        private void SourceBrowserDialog_HelpRequest(object sender, EventArgs e)
        {

        }
    }
}
