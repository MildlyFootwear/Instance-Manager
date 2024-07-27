﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Instance_Manager.Properties;
using static Instance_Manager.CommonVars;
using static Instance_Manager.Methods.CommonMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Instance_Manager
{
    public partial class ProfileManager : Form
    {
        public ProfileManager()
        {
            InitializeComponent();
        }

        int CreateButtons = 20;
        List<Button> deleteButtonList = new List<Button>();
        List<Button> renameButtonList = new List<Button>();
        List<Button> duplicateButtonList = new List<Button>();

        string RetrieveProfile(int row)
        {
            return tableLayoutPanel1.GetControlFromPosition(0, row).Text;
        }

        void InitButtons()
        {
            while (deleteButtonList.Count < CreateButtons)
            {
                int row = deleteButtonList.Count;

                Button deleteProfile = new();
                deleteProfile.Text = "Delete";
                deleteProfile.AutoSize = true;
                deleteProfile.BackColor = Color.DarkRed;
                deleteProfile.ForeColor = Color.White;

                Button renameProfile = new();
                renameProfile.Text = "Rename";
                renameProfile.AutoSize = true;

                Button duplicateProfile = new();
                duplicateProfile.Text = "Duplicate";
                duplicateProfile.AutoSize = true;

                void DeleteProfile(object sender, EventArgs e)
                {
                    string Profile = RetrieveProfile(row);
                    if (MessageBox.Show("Are you sure you want to delete profile " + Profile + "?\nAll subfolders and files will be deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Directory.Delete(Settings.Default.ProfilesDirectory + "\\" + Profile, true);
                        LoadProfiles();
                        PopulateManager();
                        Console.WriteLine("Deleting profile " + Profile);
                    }

                }

                void RenameProfile(object sender, EventArgs e)
                {
                    string Profile = RetrieveProfile(row);
                    Form TextIn = new TextInput();
                    TextIn.Text = "Rename Profile " + Profile;
                    TextIn.ShowDialog();
                    if (TextInputString != "")
                    {
                        Directory.Move(Settings.Default.ProfilesDirectory + "\\" + Profile, Settings.Default.ProfilesDirectory + "\\" + TextInputString);
                        Console.WriteLine("Renamed " + Profile + " to " + TextInputString);
                        LoadProfiles();
                        PopulateManager();
                    }
                }

                void DuplicateProfile(object sender, EventArgs e)
                {
                    string Profile = RetrieveProfile(row);
                    Form TextIn = new TextInput();
                    TextIn.Text = "Duplicate Profile Name";
                    TextIn.ShowDialog();
                    if (TextInputString != "")
                    {
                        CopyDirectory(Settings.Default.ProfilesDirectory + "\\" + Profile, Settings.Default.ProfilesDirectory + "\\" + TextInputString, true);
                        LoadProfiles();
                        PopulateManager();
                    }
                }

                deleteProfile.Click += DeleteProfile;
                deleteButtonList.Add(deleteProfile);

                renameProfile.Click += RenameProfile;
                renameButtonList.Add(renameProfile);

                duplicateProfile.Click += DuplicateProfile;
                duplicateButtonList.Add(duplicateProfile);

            }


        }

        void PopulateManager()
        {
            this.tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Clear();
            int row = 0;
            foreach (string Profile in Profiles)
            {

                if (row == 30)
                    break;

                Label profileName = new();
                profileName.Text = Profile;
                profileName.AutoSize = true;
                profileName.TextAlign = ContentAlignment.MiddleLeft;
                profileName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

                /*
                Button renameProfile = new();
                Button duplicateProfile = new();
                Button deleteProfile = new();
                renameProfile.Text = "Rename";
                duplicateProfile.Text = "Duplicate";
                deleteProfile.Text = "Delete";
                renameProfile.AutoSize = true;
                duplicateProfile.AutoSize = true;
                deleteProfile.AutoSize = true;
                deleteProfile.BackColor = Color.DarkRed;
                deleteProfile.ForeColor = Color.White;

                void DeleteProfile(object sender, EventArgs e)
                {

                    if (MessageBox.Show("Are you sure you want to delete profile " + Profile + "?\nAll subfolders and files will be deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Directory.Delete(Settings.Default.ProfilesDirectory + "\\" + Profile, true);
                        LoadProfiles();
                        PopulateManager();
                        Console.WriteLine("Deleting profile " + Profile);
                    }

                }

                void RenameProfile(object sender, EventArgs e)
                {
                    Form TextIn = new TextInput();
                    TextIn.Text = "Rename Profile "+Profile;
                    TextIn.ShowDialog();
                    if (TextInputString != "")
                    {
                        Directory.Move(Settings.Default.ProfilesDirectory + "\\" + Profile, Settings.Default.ProfilesDirectory + "\\" + TextInputString);
                        Console.WriteLine("Renamed "+Profile+" to "+ TextInputString);
                        LoadProfiles();
                        PopulateManager();
                    }
                }

                void DuplicateProfile(object sender, EventArgs e)
                {
                    Form TextIn = new TextInput();
                    TextIn.Text = "Duplicate Profile Name";
                    TextIn.ShowDialog();
                    if (TextInputString != "")
                    {
                        CopyDirectory(Settings.Default.ProfilesDirectory + "\\" + Profile, Settings.Default.ProfilesDirectory + "\\" + TextInputString, true);
                        LoadProfiles();
                        PopulateManager();
                    }
                }

                deleteProfile.Click += DeleteProfile;
                renameProfile.Click += RenameProfile;
                duplicateProfile.Click += DuplicateProfile;
                
                tableLayoutPanel1.Controls.Add(renameProfile, 1, row);
                tableLayoutPanel1.Controls.Add(duplicateProfile, 2, row);
                tableLayoutPanel1.Controls.Add(deleteProfile, 3, row);

                */

                tableLayoutPanel1.Controls.Add(profileName, 0, row);
                tableLayoutPanel1.Controls.Add(renameButtonList[row], 1, row);
                tableLayoutPanel1.Controls.Add(duplicateButtonList[row], 2, row);
                tableLayoutPanel1.Controls.Add(deleteButtonList[row], 3, row);

                row++;
            }
            tableLayoutPanel1.Controls.Add(new Label(), 0, row);
            this.tableLayoutPanel1.ResumeLayout();
        }
        private void ProfileManager_Load(object sender, EventArgs e)
        {
            InitButtons();
            PopulateManager();
            Text = ToolName+ " - Manage Profiles";
        }

        private void AddProfile_Click(object sender, EventArgs e)
        {
            Form TextIn = new TextInput();
            TextIn.Text = "Add Profile";
            int profnum = 1;
            while (Directory.Exists(Settings.Default.ProfilesDirectory + "\\Default" + profnum))
            {
                profnum++;
                Console.WriteLine("incremented profnum to " + profnum);
            }
            PassedString = "Default" + profnum;
            TextIn.ShowDialog();
            if (TextInputString != "")
            {
                if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + TextInputString))
                {
                    Directory.CreateDirectory(Settings.Default.ProfilesDirectory + "\\" + TextInputString);
                    LoadProfiles();
                    PopulateManager();
                }
                else { MessageBox.Show("Profile "+TextInputString+" already exists."); }
                
                
            }
        }
    }
}
