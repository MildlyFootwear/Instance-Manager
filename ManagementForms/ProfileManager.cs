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
        Size profmansize = new Size();

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
                            Directory.Delete(Settings.Default.ProfilesDirectory + "\\" + Profile, true);
                            LoadProfiles();
                            PopulateManager();
                            Console.WriteLine("Deleting profile " + Profile);
                            NeedRefresh = true;
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
                            NeedRefresh = true;

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
                            if (!Directory.Exists(Settings.Default.ProfilesDirectory + "\\"+TextInputString))
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
                    Console.WriteLine("Adding buttons for profile "+Profile+" on row"+row);
                }


                row++;

            }
            int temp = row;
            while (temp < tableLayoutPanel1.RowCount && temp < Labels.Count)
            {
                Console.WriteLine("Cleaning up " + Labels[row]);
                tableLayoutPanel1.Controls.Remove(Labels[row]);
                tableLayoutPanel1.Controls.Remove(deleteButtonList[row]);
                tableLayoutPanel1.Controls.Remove(duplicateButtonList[row]);
                tableLayoutPanel1.Controls.Remove(renameButtonList[row]);
                temp++;
            }
            if (profmansize == new Size())
            {
                profmansize = tableLayoutPanel1.Size;
            } else
                tableLayoutPanel1.Size = profmansize;
            tableLayoutPanel1.RowCount = row + 1;
        }

        private void ProfileManager_Load(object sender, EventArgs e)
        {
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
