namespace SLWModLoader
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ModsList = new System.Windows.Forms.ListView();
            this.NameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VersionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AuthorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SaveColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.ModPage = new System.Windows.Forms.TabPage();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.NoModsFoundLabel = new System.Windows.Forms.Label();
            this.MoveDownAll = new System.Windows.Forms.Button();
            this.MoveUpAll = new System.Windows.Forms.Button();
            this.MoveDownButton = new System.Windows.Forms.Button();
            this.MoveUpButton = new System.Windows.Forms.Button();
            this.SettingsPage = new System.Windows.Forms.TabPage();
            this.SettingsBottomPanel = new System.Windows.Forms.Panel();
            this.AboutButton = new System.Windows.Forms.Button();
            this.ReportLabel = new System.Windows.Forms.LinkLabel();
            this.SaveAndPlayButton = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DescriptionLabel = new System.Windows.Forms.LinkLabel();
            this.AddModButton = new System.Windows.Forms.Button();
            this.RemoveModButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.PiracyButtonTest = new System.Windows.Forms.Button();
            this.MainPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.ModPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SettingsPage.SuspendLayout();
            this.SettingsBottomPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ModsList
            // 
            this.ModsList.AllowDrop = true;
            this.ModsList.CheckBoxes = true;
            this.ModsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.VersionColumn,
            this.AuthorColumn,
            this.SaveColumn});
            this.ModsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModsList.FullRowSelect = true;
            this.ModsList.Location = new System.Drawing.Point(0, 0);
            this.ModsList.MultiSelect = false;
            this.ModsList.Name = "ModsList";
            this.ModsList.Size = new System.Drawing.Size(490, 412);
            this.ModsList.TabIndex = 0;
            this.ModsList.UseCompatibleStateImageBehavior = false;
            this.ModsList.View = System.Windows.Forms.View.Details;
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Name";
            this.NameColumn.Width = 175;
            // 
            // VersionColumn
            // 
            this.VersionColumn.Text = "V";
            this.VersionColumn.Width = 31;
            // 
            // AuthorColumn
            // 
            this.AuthorColumn.Text = "Author";
            this.AuthorColumn.Width = 119;
            // 
            // SaveColumn
            // 
            this.SaveColumn.Text = "Supports Save File Redirection";
            this.SaveColumn.Width = 66;
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.TabControl);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(537, 444);
            this.MainPanel.TabIndex = 1;
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.ModPage);
            this.TabControl.Controls.Add(this.SettingsPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(537, 444);
            this.TabControl.TabIndex = 0;
            // 
            // ModPage
            // 
            this.ModPage.BackColor = System.Drawing.Color.White;
            this.ModPage.Controls.Add(this.splitContainer);
            this.ModPage.Location = new System.Drawing.Point(4, 22);
            this.ModPage.Name = "ModPage";
            this.ModPage.Padding = new System.Windows.Forms.Padding(3);
            this.ModPage.Size = new System.Drawing.Size(529, 418);
            this.ModPage.TabIndex = 0;
            this.ModPage.Text = "Mods";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(3, 3);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.linkLabel1);
            this.splitContainer.Panel1.Controls.Add(this.NoModsFoundLabel);
            this.splitContainer.Panel1.Controls.Add(this.ModsList);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.MoveDownAll);
            this.splitContainer.Panel2.Controls.Add(this.MoveUpAll);
            this.splitContainer.Panel2.Controls.Add(this.MoveDownButton);
            this.splitContainer.Panel2.Controls.Add(this.MoveUpButton);
            this.splitContainer.Size = new System.Drawing.Size(523, 412);
            this.splitContainer.SplitterDistance = 490;
            this.splitContainer.TabIndex = 5;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel1.Location = new System.Drawing.Point(169, 246);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.linkLabel1.Size = new System.Drawing.Size(148, 19);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Open Mod Folder";
            // 
            // NoModsFoundLabel
            // 
            this.NoModsFoundLabel.BackColor = System.Drawing.Color.Transparent;
            this.NoModsFoundLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NoModsFoundLabel.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.NoModsFoundLabel.ForeColor = System.Drawing.Color.Red;
            this.NoModsFoundLabel.Location = new System.Drawing.Point(0, 0);
            this.NoModsFoundLabel.Name = "NoModsFoundLabel";
            this.NoModsFoundLabel.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.NoModsFoundLabel.Size = new System.Drawing.Size(490, 412);
            this.NoModsFoundLabel.TabIndex = 4;
            this.NoModsFoundLabel.Text = "No mods found!\r\nPlease check your mod folder.";
            this.NoModsFoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MoveDownAll
            // 
            this.MoveDownAll.Enabled = false;
            this.MoveDownAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveDownAll.Location = new System.Drawing.Point(5, 307);
            this.MoveDownAll.Name = "MoveDownAll";
            this.MoveDownAll.Size = new System.Drawing.Size(19, 100);
            this.MoveDownAll.TabIndex = 3;
            this.MoveDownAll.Text = "↓";
            this.MoveDownAll.UseVisualStyleBackColor = true;
            // 
            // MoveUpAll
            // 
            this.MoveUpAll.Enabled = false;
            this.MoveUpAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUpAll.Location = new System.Drawing.Point(5, 4);
            this.MoveUpAll.Name = "MoveUpAll";
            this.MoveUpAll.Size = new System.Drawing.Size(19, 100);
            this.MoveUpAll.TabIndex = 2;
            this.MoveUpAll.Text = "↑";
            this.MoveUpAll.UseVisualStyleBackColor = true;
            // 
            // MoveDownButton
            // 
            this.MoveDownButton.Enabled = false;
            this.MoveDownButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveDownButton.Location = new System.Drawing.Point(5, 206);
            this.MoveDownButton.Name = "MoveDownButton";
            this.MoveDownButton.Size = new System.Drawing.Size(19, 100);
            this.MoveDownButton.TabIndex = 1;
            this.MoveDownButton.Text = "▼";
            this.MoveDownButton.UseVisualStyleBackColor = true;
            // 
            // MoveUpButton
            // 
            this.MoveUpButton.Enabled = false;
            this.MoveUpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUpButton.Location = new System.Drawing.Point(5, 105);
            this.MoveUpButton.Name = "MoveUpButton";
            this.MoveUpButton.Size = new System.Drawing.Size(19, 100);
            this.MoveUpButton.TabIndex = 0;
            this.MoveUpButton.Text = "▲";
            this.MoveUpButton.UseVisualStyleBackColor = true;
            // 
            // SettingsPage
            // 
            this.SettingsPage.Controls.Add(this.PiracyButtonTest);
            this.SettingsPage.Controls.Add(this.SettingsBottomPanel);
            this.SettingsPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsPage.Name = "SettingsPage";
            this.SettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.SettingsPage.Size = new System.Drawing.Size(529, 418);
            this.SettingsPage.TabIndex = 1;
            this.SettingsPage.Text = "Settings";
            this.SettingsPage.UseVisualStyleBackColor = true;
            // 
            // SettingsBottomPanel
            // 
            this.SettingsBottomPanel.Controls.Add(this.AboutButton);
            this.SettingsBottomPanel.Controls.Add(this.ReportLabel);
            this.SettingsBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SettingsBottomPanel.Location = new System.Drawing.Point(3, 315);
            this.SettingsBottomPanel.Name = "SettingsBottomPanel";
            this.SettingsBottomPanel.Size = new System.Drawing.Size(523, 100);
            this.SettingsBottomPanel.TabIndex = 12;
            // 
            // AboutButton
            // 
            this.AboutButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AboutButton.Location = new System.Drawing.Point(125, 42);
            this.AboutButton.Name = "AboutButton";
            this.AboutButton.Size = new System.Drawing.Size(276, 33);
            this.AboutButton.TabIndex = 8;
            this.AboutButton.Text = "&About SLW Mod Loader";
            this.AboutButton.UseVisualStyleBackColor = true;
            // 
            // ReportLabel
            // 
            this.ReportLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ReportLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.ReportLabel.Location = new System.Drawing.Point(0, 0);
            this.ReportLabel.Name = "ReportLabel";
            this.ReportLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.ReportLabel.Size = new System.Drawing.Size(523, 100);
            this.ReportLabel.TabIndex = 11;
            this.ReportLabel.TabStop = true;
            this.ReportLabel.Text = "Report a problem/request a feature";
            this.ReportLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // SaveAndPlayButton
            // 
            this.SaveAndPlayButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SaveAndPlayButton.Location = new System.Drawing.Point(112, 489);
            this.SaveAndPlayButton.Name = "SaveAndPlayButton";
            this.SaveAndPlayButton.Size = new System.Drawing.Size(313, 43);
            this.SaveAndPlayButton.TabIndex = 2;
            this.SaveAndPlayButton.Text = "Save &and Play";
            this.SaveAndPlayButton.UseVisualStyleBackColor = true;
            this.SaveAndPlayButton.Click += new System.EventHandler(this.SaveAndPlayButton_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RefreshButton.Location = new System.Drawing.Point(112, 450);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(313, 33);
            this.RefreshButton.TabIndex = 6;
            this.RefreshButton.Text = "&Refresh Mod List";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.White;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 541);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(537, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.DescriptionLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DescriptionLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.DescriptionLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.DescriptionLabel.Location = new System.Drawing.Point(0, 0);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Padding = new System.Windows.Forms.Padding(0, 455, 0, 0);
            this.DescriptionLabel.Size = new System.Drawing.Size(537, 563);
            this.DescriptionLabel.TabIndex = 7;
            this.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AddModButton
            // 
            this.AddModButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AddModButton.Location = new System.Drawing.Point(431, 450);
            this.AddModButton.Name = "AddModButton";
            this.AddModButton.Size = new System.Drawing.Size(99, 33);
            this.AddModButton.TabIndex = 7;
            this.AddModButton.Text = "+";
            this.AddModButton.UseVisualStyleBackColor = true;
            // 
            // RemoveModButton
            // 
            this.RemoveModButton.Enabled = false;
            this.RemoveModButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RemoveModButton.Location = new System.Drawing.Point(7, 450);
            this.RemoveModButton.Name = "RemoveModButton";
            this.RemoveModButton.Size = new System.Drawing.Size(99, 33);
            this.RemoveModButton.TabIndex = 5;
            this.RemoveModButton.Text = "-";
            this.RemoveModButton.UseVisualStyleBackColor = true;
            // 
            // PlayButton
            // 
            this.PlayButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PlayButton.Location = new System.Drawing.Point(431, 489);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(99, 43);
            this.PlayButton.TabIndex = 4;
            this.PlayButton.Text = "&Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SaveButton.Location = new System.Drawing.Point(7, 489);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(99, 43);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "&Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // PiracyButtonTest
            // 
            this.PiracyButtonTest.Location = new System.Drawing.Point(9, 7);
            this.PiracyButtonTest.Name = "PiracyButtonTest";
            this.PiracyButtonTest.Size = new System.Drawing.Size(75, 23);
            this.PiracyButtonTest.TabIndex = 13;
            this.PiracyButtonTest.Text = "Piracy Form";
            this.PiracyButtonTest.UseVisualStyleBackColor = true;
            this.PiracyButtonTest.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.SaveAndPlayButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(537, 563);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.RemoveModButton);
            this.Controls.Add(this.AddModButton);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.SaveAndPlayButton);
            this.Controls.Add(this.DescriptionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLW Mod Loader";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainPanel.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.ModPage.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.SettingsPage.ResumeLayout(false);
            this.SettingsBottomPanel.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ModsList;
        private System.Windows.Forms.ColumnHeader NameColumn;
        private System.Windows.Forms.ColumnHeader VersionColumn;
        private System.Windows.Forms.ColumnHeader AuthorColumn;
        private System.Windows.Forms.ColumnHeader SaveColumn;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Button SaveAndPlayButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button MoveDownButton;
        private System.Windows.Forms.Button MoveUpButton;
        private System.Windows.Forms.Button MoveDownAll;
        private System.Windows.Forms.Button MoveUpAll;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage ModPage;
        private System.Windows.Forms.TabPage SettingsPage;
        private System.Windows.Forms.Button AboutButton;
        private System.Windows.Forms.LinkLabel DescriptionLabel;
        private System.Windows.Forms.LinkLabel ReportLabel;
        private System.Windows.Forms.Panel SettingsBottomPanel;
        private System.Windows.Forms.Button AddModButton;
        private System.Windows.Forms.Button RemoveModButton;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label NoModsFoundLabel;
        private System.Windows.Forms.Button PiracyButtonTest;
    }
}

