namespace Instance_Manager
{
    partial class Info
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
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            label2 = new Label();
            linkLabel2 = new LinkLabel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(12, 8);
            label1.Name = "label1";
            label1.Size = new Size(310, 15);
            label1.TabIndex = 0;
            label1.Text = "GitHub";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(12, 26);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(297, 15);
            linkLabel1.TabIndex = 1;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://github.com/MildlyFootwear/Instance-Manager";
            linkLabel1.TextAlign = ContentAlignment.TopCenter;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            linkLabel1.Resize += linkLabel1_Resize;
            // 
            // label2
            // 
            label2.Location = new Point(12, 44);
            label2.Margin = new Padding(3);
            label2.Name = "label2";
            label2.Size = new Size(310, 15);
            label2.TabIndex = 2;
            label2.Text = "Nexus Mods";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // linkLabel2
            // 
            linkLabel2.Location = new Point(12, 62);
            linkLabel2.Margin = new Padding(3, 3, 12, 12);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(310, 15);
            linkLabel2.TabIndex = 3;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "https://next.nexusmods.com/profile/MildlyFootwear";
            linkLabel2.TextAlign = ContentAlignment.TopCenter;
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // Info
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScrollMargin = new Size(9, 9);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(341, 117);
            Controls.Add(linkLabel2);
            Controls.Add(label2);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Info";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Instance Manager - Info";
            Load += Info_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private LinkLabel linkLabel1;
        private Label label2;
        private LinkLabel linkLabel2;
    }
}