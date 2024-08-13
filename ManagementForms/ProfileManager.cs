using Microsoft.VisualBasic.FileIO;

namespace Instance_Manager
{
    public partial class ProfileManager : Form
    {
        public ProfileManager()
        {
            InitializeComponent();
        }

        List<Label> Labels = new List<Label>();
        List<Button> deleteButtonList = new List<Button>();
        List<Button> renameButtonList = new List<Button>();
        List<Button> duplicateButtonList = new List<Button>();

        string RetrieveProfile(int row)
        {
            return tableLayoutPanel1.GetControlFromPosition(0, row).Text;
        }

        void PopulateManager()
        {
            int row = 0;
            foreach (string Profile in Profiles)
            {

                if (row == Labels.Count)
                {
                    int rowfun = row;
                    Button deleteProfile = new();
                    deleteProfile.Text = "Delete";
                    deleteProfile.AutoSize = true;
                    deleteProfile.BackColor = Color.DarkRed;
                    deleteProfile.ForeColor = Color.White;

                    Button renameProfile = new();
                    renameProfile.Text = "Rename";
                    renameProfile.AutoSize = true;
                    renameProfile.BackColor = AddProfile.BackColor;

                    Button duplicateProfile = new();
                    duplicateProfile.Text = "Duplicate";
                    duplicateProfile.AutoSize = true;
                    duplicateProfile.BackColor = AddProfile.BackColor;


                    void DeleteProfile(object sender, EventArgs e)
                    {
                        string Profile = RetrieveProfile(rowfun);
                        if (MessageBox.Show("Are you sure you want to delete profile " + Profile + "?\nAll subfolders and files will be deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            FileSystem.DeleteDirectory(Settings.Default.ProfilesDirectory + "\\" + Profile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            LoadProfiles();
                            PopulateManager();
                            Console.WriteLine("Deleting profile " + Profile);
                            NeedSelectedProfileRefresh = true;
                        }

                    }

                    void RenameProfile(object sender, EventArgs e)
                    {
                        string Profile = RetrieveProfile(rowfun);
                        Form TextIn = new TextInput();
                        TextIn.Text = "Rename Profile " + Profile;
                        TextIn.ShowDialog();
                        if (TextInputString != "")
                        {
                            Directory.Move(Settings.Default.ProfilesDirectory + "\\" + Profile, Settings.Default.ProfilesDirectory + "\\" + TextInputString);
                            Console.WriteLine("Renamed " + Profile + " to " + TextInputString);
                            LoadProfiles();
                            PopulateManager();
                            NeedSelectedProfileRefresh = true;

                        }
                    }

                    void DuplicateProfile(object sender, EventArgs e)
                    {
                        string Profile = RetrieveProfile(rowfun);
                        Form TextIn = new TextInput();
                        TextIn.Text = "Duplicate Profile Name";
                        PassedString = Profile + " - Copy";
                        TextIn.ShowDialog();
                        if (TextInputString != "")
                        {
                            if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\" + TextInputString))
                            {
                                CopyDirectory(Settings.Default.ProfilesDirectory + "\\" + Profile, Settings.Default.ProfilesDirectory + "\\" + TextInputString, true);
                                LoadProfiles();
                                PopulateManager();
                            }

                        }
                    }

                    deleteProfile.Click += DeleteProfile;
                    deleteButtonList.Add(deleteProfile);

                    renameProfile.Click += RenameProfile;
                    renameButtonList.Add(renameProfile);

                    duplicateProfile.Click += DuplicateProfile;
                    duplicateButtonList.Add(duplicateProfile);

                    Label profileName = new();
                    profileName.AutoSize = true;
                    profileName.TextAlign = ContentAlignment.MiddleLeft;
                    profileName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
                    Labels.Add(profileName);

                }

                Labels[row].Text = Profile;

                if (tableLayoutPanel1.GetControlFromPosition(0, row) != Labels[row])
                {
                    tableLayoutPanel1.Controls.Add(Labels[row], 0, row);
                    tableLayoutPanel1.Controls.Add(renameButtonList[row], 1, row);
                    tableLayoutPanel1.Controls.Add(duplicateButtonList[row], 2, row);
                    tableLayoutPanel1.Controls.Add(deleteButtonList[row], 3, row);
                    Console.WriteLine("Adding buttons for profile " + Profile + " on row" + row);
                }


                row++;

            }
            int temp = row;
            while (temp < Labels.Count)
            {
                if (tableLayoutPanel1.Contains(Labels[temp]) == false)
                    break;
                Console.WriteLine("Cleaning up " + Labels[temp]);
                tableLayoutPanel1.Controls.Remove(Labels[temp]);
                tableLayoutPanel1.Controls.Remove(deleteButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(duplicateButtonList[temp]);
                tableLayoutPanel1.Controls.Remove(renameButtonList[temp]);
                temp++;
            }

            tableLayoutPanel1.RowCount = row;
            Console.WriteLine("temp is " + temp + " row count is " + tableLayoutPanel1.RowCount + " and label count is " + Labels.Count);

        }

        private void ProfileManager_Load(object sender, EventArgs e)
        {
            PopulateManager();
            Text = ToolName + " - Manage Profiles";
            void Close(object sender, EventArgs e)
            {
                this.Close();
            }

            Button cancel = new();
            cancel.Click += Close;
            this.CancelButton = cancel;
            CenterToParent();
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
                else { MessageBox.Show("Profile " + TextInputString + " already exists."); }
            }
        }

        private void ProfileManager_Resize(object sender, EventArgs e)
        {
        }
    }
}
