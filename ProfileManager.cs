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
using Instance_Manager.Properties;
using static Instance_Manager.CommonVars;
using static Instance_Manager.CommonMethods;

namespace Instance_Manager
{
    public partial class ProfileManager : Form
    {
        public ProfileManager()
        {
            InitializeComponent();
        }

        void PopulateManager()
        {
            tableLayoutPanel1.Controls.Clear();
            int row = 0;
            foreach (string Profile in Profiles)
            {
                Label profileName = new();
                Button renameProfile = new();
                Button duplicateProfile = new();
                Button deleteProfile = new();
                profileName.Text = Profile;
                profileName.AutoSize = true;
                profileName.TextAlign = ContentAlignment.MiddleLeft;
                profileName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
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
                    if (MessageBox.Show("Are you sure you want to delete profile " + Profile + "?\nAll subfolders and files will be deleted too.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
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

                tableLayoutPanel1.Controls.Add(profileName, 0, row);
                tableLayoutPanel1.Controls.Add(renameProfile, 1, row);
                tableLayoutPanel1.Controls.Add(duplicateProfile, 2, row);
                tableLayoutPanel1.Controls.Add(deleteProfile, 3, row);

                row++;
            }
        }
        private void ProfileManager_Load(object sender, EventArgs e)
        {
            PopulateManager();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AddProfile_Click(object sender, EventArgs e)
        {
            Form TextIn = new TextInput();
            TextIn.Text = "Add Profile";
            TextIn.ShowDialog();
            if (TextInputString != "")
            {
                Directory.CreateDirectory(Settings.Default.ProfilesDirectory+"\\"+TextInputString);
            }
            LoadProfiles();
            PopulateManager();
        }
    }
}
