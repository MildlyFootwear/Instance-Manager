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

        public MainUI()
        {
            InitializeComponent();
        }

        private void RefreshProfiles()
        {
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
            Console.WriteLine("\nRefreshing lists\n");
            if (Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + Settings.Default.ActiveProfile) == false)
            {
                Settings.Default.ActiveProfile = Profiles[0];
                Console.WriteLine("Set profile not found, setting to " + Profiles[0]);
            }
            this.Text = ToolName +" - " + Settings.Default.ActiveProfile;
            LoadProfileLinks();
            int row = 1;
            tableLayoutPanel1.Controls.Clear();
            Label column0Label = new Label();
            Label column1Label = new Label();
            column0Label.Text = "File Save/Load Location";
            column1Label.Text = "Redirect From";
            column0Label.AutoSize = true;
            column1Label.AutoSize = true;
            column0Label.TextAlign = ContentAlignment.TopCenter;
            column1Label.TextAlign = ContentAlignment.TopCenter;
            column0Label.Anchor = AnchorStyles.Top;
            column1Label.Anchor = AnchorStyles.Top;
            tableLayoutPanel1.Controls.Add(column0Label);
            tableLayoutPanel1.Controls.Add(column1Label);
            foreach (string link in DirectoryLinks)
            {
                string[] splitlink = link.Split(";");
                Label Source = new();
                Label Destination = new();
                Source.Text = splitlink[0];
                Destination.Text = splitlink[1];
                Source.Name = (Source.Text) + row.ToString();
                Destination.Name = (Destination.Text) + row.ToString();
                Source.AutoSize = true;
                Destination.AutoSize = true;

                void SourceClick(object sender, EventArgs e)
                {
                    Console.WriteLine();
                    Form TextIn = new DirectoryTextEditor();
                    TextIn.Text = splitlink[0];
                    TextIn.ShowDialog();
                    if (TextInputString == "Remove")
                    {
                        DirectoryLinks.Remove(link);
                        SaveProfileLinks();
                        RefreshList();
                    }
                    else if (TextInputString != splitlink[0] && TextInputString != "Cancel" && TextInputString != "")
                    {
                        if (Directory.Exists(ReplaceVariables(TextInputString)))
                        {
                            DirectoryLinks[DirectoryLinks.IndexOf(link)] = TextInputString + ";" + splitlink[1];
                            SaveProfileLinks();
                            RefreshList();
                        }
                        else
                        {
                            if (MessageBox.Show("Could not find directory " + ReplaceVariables(TextInputString) + ". Continue save?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                DirectoryLinks[DirectoryLinks.IndexOf(link)] = TextInputString + ";" + splitlink[1];
                                SaveProfileLinks();
                                RefreshList();
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Canceling link edit for " + link);
                    }
                    RefreshList();
                }
                void DestinationClick(object sender, EventArgs e)
                {
                    Console.WriteLine();
                    Form TextIn = new DirectoryTextEditor();
                    TextIn.Text = splitlink[1];
                    TextIn.ShowDialog();
                    if (TextInputString == "Remove")
                    {
                        DirectoryLinks.Remove(link);
                        SaveProfileLinks();
                        RefreshList();
                    }
                    else if (TextInputString != splitlink[1] && TextInputString != "Cancel" && TextInputString != "")
                    {
                        if (Directory.Exists(ReplaceVariables(TextInputString)))
                        {
                            DirectoryLinks[DirectoryLinks.IndexOf(link)] = splitlink[0] + ";" + TextInputString;
                            SaveProfileLinks();
                            RefreshList();
                        }
                        else
                        {

                            if (MessageBox.Show("Could not find directory " + ReplaceVariables(TextInputString) + ". Continue save?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                DirectoryLinks[DirectoryLinks.IndexOf(link)] = splitlink[0] + ";" + TextInputString;
                                SaveProfileLinks();
                                RefreshList();
                            }
                            else { Console.WriteLine("Canceling link edit for " + link); }

                        }

                    }
                    else { Console.WriteLine("Canceling link edit for " + link); }


                }

                Source.Click += SourceClick;
                Destination.Click += DestinationClick;

                tableLayoutPanel1.Controls.Add(Source, 0, row);
                tableLayoutPanel1.Controls.Add(Destination, 1, row);
                Console.WriteLine("Added " + Source.Text + " to column 0, row " + row + ". Added " + Destination.Text + " to column 1, row " + row);
                row++;
            }
            tableLayoutPanel1.Controls.Add(new Label(), 0, row);
            tableLayoutPanel1.ResumeLayout();
            Console.WriteLine("Refreshed lists.");
        }

        void RefreshExes()
        {
            Console.WriteLine("\nRefreshing profile exes.");
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
                } else
                {
                    ExeBox.SelectedIndex = 0;
                    SelectedExe = ProfileExes[0];
                    Console.WriteLine(SelectedExe + " not found in prof exes.");
                }
            } else
            {
                SelectedExe = "";
            }
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            Console.WriteLine("\nLoading MainUI");
            if (Settings.Default.SavedPosition != new Point(1, 1))
                this.Location = Settings.Default.SavedPosition; Console.WriteLine("Set position to " + this.Location);
            if (Settings.Default.SavedSize != new Size(1, 1))
                this.Size = Settings.Default.SavedSize; Console.WriteLine("Set size to " + this.Size);
            SelectedExe = Settings.Default.SavedExe;
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

            string selected = ProfilesBox.SelectedItem.ToString();
            SetProfile(selected);
            RefreshList();
            RefreshExes();

        }

        private void ExeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (JustRefreshedExes)
            {
                JustRefreshedExes = false;
                return;
            }
            if (ExeBox.SelectedIndex != ProfileExes.IndexOf(SelectedExe))
            {

                SelectedExe = ProfileExes[ExeBox.SelectedIndex];
                Console.WriteLine("\nSelected EXE " + SelectedExe);
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void LinkButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nStarting link designation.");
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
            if (active != Settings.Default.ActiveProfile)
            {
                RefreshList();
                RefreshExes();
            }
            else { RefreshProfiles(); }
        }

        private void toolManageVariables_Click(object sender, EventArgs e)
        {
            Form VarM = new ManageVariables();
            VarM.Text = ToolName +" - Manage Variables";
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
    }
}
