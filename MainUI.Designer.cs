namespace Instance_Manager
{
    partial class MainUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUI));
            tableLayoutPanel1 = new TableLayoutPanel();
            ProfilesLabel = new Label();
            ProfilesBox = new ComboBox();
            SourceBrowserDialog = new FolderBrowserDialog();
            DestinationBrowserDialog = new FolderBrowserDialog();
            ExeBox = new ComboBox();
            ExeLabel = new Label();
            ExeBrowserDialog = new OpenFileDialog();
            LinkButton = new Button();
            toolStrip1 = new ToolStrip();
            toolManageVariables = new ToolStripButton();
            toolStripManageProfiles = new ToolStripButton();
            toolManageExes = new ToolStripButton();
            toolHelp = new ToolStripButton();
            contextMenuStrip1 = new ContextMenuStrip(components);
            buttonLaunch = new Button();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.BackColor = SystemColors.ButtonHighlight;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Location = new Point(12, 74);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(920, 386);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // ProfilesLabel
            // 
            ProfilesLabel.AutoSize = true;
            ProfilesLabel.Location = new Point(12, 27);
            ProfilesLabel.Name = "ProfilesLabel";
            ProfilesLabel.Size = new Size(46, 15);
            ProfilesLabel.TabIndex = 1;
            ProfilesLabel.Text = "Profiles";
            // 
            // ProfilesBox
            // 
            ProfilesBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ProfilesBox.FormattingEnabled = true;
            ProfilesBox.Items.AddRange(new object[] { "You should never see this." });
            ProfilesBox.Location = new Point(12, 45);
            ProfilesBox.MaxDropDownItems = 20;
            ProfilesBox.Name = "ProfilesBox";
            ProfilesBox.Size = new Size(150, 23);
            ProfilesBox.TabIndex = 2;
            ProfilesBox.SelectedIndexChanged += ProfilesBox_SelectedIndexChanged;
            ProfilesBox.SelectionChangeCommitted += ProfilesBox_SelectedValueChanged;
            // 
            // SourceBrowserDialog
            // 
            SourceBrowserDialog.Description = "Select Folder To Save and Load Files From";
            SourceBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            SourceBrowserDialog.UseDescriptionForTitle = true;
            SourceBrowserDialog.HelpRequest += SourceBrowserDialog_HelpRequest;
            // 
            // DestinationBrowserDialog
            // 
            DestinationBrowserDialog.Description = "Select Folder to Virtually Overlay On";
            DestinationBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            DestinationBrowserDialog.UseDescriptionForTitle = true;
            // 
            // ExeBox
            // 
            ExeBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ExeBox.FormattingEnabled = true;
            ExeBox.Items.AddRange(new object[] { "You should never see this." });
            ExeBox.Location = new Point(731, 45);
            ExeBox.MaxDropDownItems = 30;
            ExeBox.Name = "ExeBox";
            ExeBox.Size = new Size(200, 23);
            ExeBox.TabIndex = 3;
            ExeBox.SelectedIndexChanged += ExeBox_SelectedIndexChanged;
            // 
            // ExeLabel
            // 
            ExeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExeLabel.AutoSize = true;
            ExeLabel.Location = new Point(859, 27);
            ExeLabel.Name = "ExeLabel";
            ExeLabel.Size = new Size(69, 15);
            ExeLabel.TabIndex = 4;
            ExeLabel.Text = "Executables";
            // 
            // LinkButton
            // 
            LinkButton.Anchor = AnchorStyles.Top;
            LinkButton.AutoSize = true;
            LinkButton.Location = new Point(421, 46);
            LinkButton.Name = "LinkButton";
            LinkButton.Size = new Size(98, 25);
            LinkButton.TabIndex = 5;
            LinkButton.Text = "Link Directories";
            LinkButton.Click += LinkButton_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolManageVariables, toolStripManageProfiles, toolManageExes, toolHelp });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(944, 25);
            toolStrip1.TabIndex = 6;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolManageVariables
            // 
            toolManageVariables.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolManageVariables.Image = (Image)resources.GetObject("toolManageVariables.Image");
            toolManageVariables.ImageTransparentColor = Color.Magenta;
            toolManageVariables.Name = "toolManageVariables";
            toolManageVariables.Size = new Size(103, 22);
            toolManageVariables.Text = "Manage Variables";
            toolManageVariables.Click += toolManageVariables_Click;
            // 
            // toolStripManageProfiles
            // 
            toolStripManageProfiles.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripManageProfiles.Image = (Image)resources.GetObject("toolStripManageProfiles.Image");
            toolStripManageProfiles.ImageTransparentColor = Color.Magenta;
            toolStripManageProfiles.Name = "toolStripManageProfiles";
            toolStripManageProfiles.Size = new Size(96, 22);
            toolStripManageProfiles.Text = "Manage Profiles";
            toolStripManageProfiles.ToolTipText = "Create, rename, duplicate, or delete profiles.";
            toolStripManageProfiles.Click += toolStripManageProfiles_Click;
            // 
            // toolManageExes
            // 
            toolManageExes.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolManageExes.Image = (Image)resources.GetObject("toolManageExes.Image");
            toolManageExes.ImageTransparentColor = Color.Magenta;
            toolManageExes.Name = "toolManageExes";
            toolManageExes.Size = new Size(119, 22);
            toolManageExes.Text = "Manage Executables";
            toolManageExes.Click += toolManageExes_Click;
            // 
            // toolHelp
            // 
            toolHelp.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolHelp.Image = (Image)resources.GetObject("toolHelp.Image");
            toolHelp.ImageTransparentColor = Color.Magenta;
            toolHelp.Name = "toolHelp";
            toolHelp.Size = new Size(36, 22);
            toolHelp.Text = "Help";
            toolHelp.Click += toolHelp_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // buttonLaunch
            // 
            buttonLaunch.Anchor = AnchorStyles.Bottom;
            buttonLaunch.Location = new Point(433, 466);
            buttonLaunch.Name = "buttonLaunch";
            buttonLaunch.Size = new Size(75, 23);
            buttonLaunch.TabIndex = 7;
            buttonLaunch.Text = "Launch";
            buttonLaunch.UseVisualStyleBackColor = true;
            buttonLaunch.Click += buttonLaunch_Click;
            // 
            // MainUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(944, 501);
            Controls.Add(buttonLaunch);
            Controls.Add(toolStrip1);
            Controls.Add(LinkButton);
            Controls.Add(ExeLabel);
            Controls.Add(ExeBox);
            Controls.Add(ProfilesBox);
            Controls.Add(ProfilesLabel);
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 210);
            Name = "MainUI";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Instance Manager";
            FormClosing += MainUI_FormClosing;
            Load += MainUI_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label ProfilesLabel;
        public ComboBox ProfilesBox;
        private FolderBrowserDialog SourceBrowserDialog;
        private FolderBrowserDialog DestinationBrowserDialog;
        private ComboBox ExeBox;
        private Label ExeLabel;
        private OpenFileDialog ExeBrowserDialog;
        private Button LinkButton;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripManageProfiles;
        private ToolStripButton toolManageVariables;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripButton toolManageExes;
        private ToolStripButton toolHelp;
        private Button buttonLaunch;
    }
}