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
            this.modsList = new System.Windows.Forms.ListView();
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.versionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.authorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.saveColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainPnl = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.modPage = new System.Windows.Forms.TabPage();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.noModsFoundLbl = new System.Windows.Forms.Label();
            this.moveDownAll = new System.Windows.Forms.Button();
            this.moveUpAll = new System.Windows.Forms.Button();
            this.moveDownbtn = new System.Windows.Forms.Button();
            this.moveUpbtn = new System.Windows.Forms.Button();
            this.settingsPage = new System.Windows.Forms.TabPage();
            this.settingsBottomPnl = new System.Windows.Forms.Panel();
            this.aboutBtn = new System.Windows.Forms.Button();
            this.reportLbl = new System.Windows.Forms.LinkLabel();
            this.saveAndPlayBtn = new System.Windows.Forms.Button();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.descriptionLbl = new System.Windows.Forms.LinkLabel();
            this.addModBtn = new System.Windows.Forms.Button();
            this.rmModBtn = new System.Windows.Forms.Button();
            this.playBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.mainPnl.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.modPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.settingsPage.SuspendLayout();
            this.settingsBottomPnl.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // modsList
            // 
            this.modsList.AllowDrop = true;
            this.modsList.CheckBoxes = true;
            this.modsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.versionColumn,
            this.authorColumn,
            this.saveColumn});
            this.modsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modsList.FullRowSelect = true;
            this.modsList.Location = new System.Drawing.Point(0, 0);
            this.modsList.MultiSelect = false;
            this.modsList.Name = "modsList";
            this.modsList.Size = new System.Drawing.Size(501, 405);
            this.modsList.TabIndex = 0;
            this.modsList.UseCompatibleStateImageBehavior = false;
            this.modsList.View = System.Windows.Forms.View.Details;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 175;
            // 
            // versionColumn
            // 
            this.versionColumn.Text = "V";
            this.versionColumn.Width = 31;
            // 
            // authorColumn
            // 
            this.authorColumn.Text = "Author";
            this.authorColumn.Width = 119;
            // 
            // saveColumn
            // 
            this.saveColumn.Text = "Supports Save File Redirection";
            this.saveColumn.Width = 66;
            // 
            // mainPnl
            // 
            this.mainPnl.Controls.Add(this.tabControl);
            this.mainPnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainPnl.Location = new System.Drawing.Point(0, 0);
            this.mainPnl.Name = "mainPnl";
            this.mainPnl.Size = new System.Drawing.Size(548, 444);
            this.mainPnl.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.modPage);
            this.tabControl.Controls.Add(this.settingsPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(548, 444);
            this.tabControl.TabIndex = 0;
            // 
            // modPage
            // 
            this.modPage.BackColor = System.Drawing.Color.White;
            this.modPage.Controls.Add(this.splitContainer);
            this.modPage.Location = new System.Drawing.Point(4, 29);
            this.modPage.Name = "modPage";
            this.modPage.Padding = new System.Windows.Forms.Padding(3);
            this.modPage.Size = new System.Drawing.Size(540, 411);
            this.modPage.TabIndex = 0;
            this.modPage.Text = "Mods";
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
            this.splitContainer.Panel1.Controls.Add(this.noModsFoundLbl);
            this.splitContainer.Panel1.Controls.Add(this.modsList);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.moveDownAll);
            this.splitContainer.Panel2.Controls.Add(this.moveUpAll);
            this.splitContainer.Panel2.Controls.Add(this.moveDownbtn);
            this.splitContainer.Panel2.Controls.Add(this.moveUpbtn);
            this.splitContainer.Size = new System.Drawing.Size(534, 405);
            this.splitContainer.SplitterDistance = 501;
            this.splitContainer.TabIndex = 5;
            // 
            // noModsFoundLbl
            // 
            this.noModsFoundLbl.BackColor = System.Drawing.Color.Transparent;
            this.noModsFoundLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noModsFoundLbl.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.noModsFoundLbl.ForeColor = System.Drawing.Color.Red;
            this.noModsFoundLbl.Location = new System.Drawing.Point(0, 0);
            this.noModsFoundLbl.Name = "noModsFoundLbl";
            this.noModsFoundLbl.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.noModsFoundLbl.Size = new System.Drawing.Size(501, 405);
            this.noModsFoundLbl.TabIndex = 4;
            this.noModsFoundLbl.Text = "No mods found!\r\nPlease check your mod folder.";
            this.noModsFoundLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // moveDownAll
            // 
            this.moveDownAll.Enabled = false;
            this.moveDownAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveDownAll.Location = new System.Drawing.Point(3, 306);
            this.moveDownAll.Name = "moveDownAll";
            this.moveDownAll.Size = new System.Drawing.Size(19, 100);
            this.moveDownAll.TabIndex = 3;
            this.moveDownAll.Text = "↓";
            this.moveDownAll.UseVisualStyleBackColor = true;
            // 
            // moveUpAll
            // 
            this.moveUpAll.Enabled = false;
            this.moveUpAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveUpAll.Location = new System.Drawing.Point(3, 3);
            this.moveUpAll.Name = "moveUpAll";
            this.moveUpAll.Size = new System.Drawing.Size(19, 100);
            this.moveUpAll.TabIndex = 2;
            this.moveUpAll.Text = "↑";
            this.moveUpAll.UseVisualStyleBackColor = true;
            // 
            // moveDownbtn
            // 
            this.moveDownbtn.Enabled = false;
            this.moveDownbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveDownbtn.Location = new System.Drawing.Point(3, 205);
            this.moveDownbtn.Name = "moveDownbtn";
            this.moveDownbtn.Size = new System.Drawing.Size(19, 100);
            this.moveDownbtn.TabIndex = 1;
            this.moveDownbtn.Text = "▼";
            this.moveDownbtn.UseVisualStyleBackColor = true;
            // 
            // moveUpbtn
            // 
            this.moveUpbtn.Enabled = false;
            this.moveUpbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveUpbtn.Location = new System.Drawing.Point(3, 104);
            this.moveUpbtn.Name = "moveUpbtn";
            this.moveUpbtn.Size = new System.Drawing.Size(19, 100);
            this.moveUpbtn.TabIndex = 0;
            this.moveUpbtn.Text = "▲";
            this.moveUpbtn.UseVisualStyleBackColor = true;
            // 
            // settingsPage
            // 
            this.settingsPage.Controls.Add(this.settingsBottomPnl);
            this.settingsPage.Location = new System.Drawing.Point(4, 29);
            this.settingsPage.Name = "settingsPage";
            this.settingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.settingsPage.Size = new System.Drawing.Size(540, 411);
            this.settingsPage.TabIndex = 1;
            this.settingsPage.Text = "Settings";
            this.settingsPage.UseVisualStyleBackColor = true;
            // 
            // settingsBottomPnl
            // 
            this.settingsBottomPnl.Controls.Add(this.aboutBtn);
            this.settingsBottomPnl.Controls.Add(this.reportLbl);
            this.settingsBottomPnl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.settingsBottomPnl.Location = new System.Drawing.Point(3, 308);
            this.settingsBottomPnl.Name = "settingsBottomPnl";
            this.settingsBottomPnl.Size = new System.Drawing.Size(534, 100);
            this.settingsBottomPnl.TabIndex = 12;
            // 
            // aboutBtn
            // 
            this.aboutBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.aboutBtn.Location = new System.Drawing.Point(129, 36);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(276, 36);
            this.aboutBtn.TabIndex = 8;
            this.aboutBtn.Text = "&About SLW Mod Loader";
            this.aboutBtn.UseVisualStyleBackColor = true;
            // 
            // reportLbl
            // 
            this.reportLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.reportLbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.reportLbl.Location = new System.Drawing.Point(0, 0);
            this.reportLbl.Name = "reportLbl";
            this.reportLbl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.reportLbl.Size = new System.Drawing.Size(534, 100);
            this.reportLbl.TabIndex = 11;
            this.reportLbl.TabStop = true;
            this.reportLbl.Text = "Report a problem/request a feature";
            this.reportLbl.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // saveAndPlayBtn
            // 
            this.saveAndPlayBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveAndPlayBtn.Location = new System.Drawing.Point(122, 519);
            this.saveAndPlayBtn.Name = "saveAndPlayBtn";
            this.saveAndPlayBtn.Size = new System.Drawing.Size(305, 43);
            this.saveAndPlayBtn.TabIndex = 2;
            this.saveAndPlayBtn.Text = "Save &and Play";
            this.saveAndPlayBtn.UseVisualStyleBackColor = true;
            // 
            // refreshBtn
            // 
            this.refreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.refreshBtn.Location = new System.Drawing.Point(122, 480);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(305, 33);
            this.refreshBtn.TabIndex = 6;
            this.refreshBtn.Text = "&Refresh Mod List";
            this.refreshBtn.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.White;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLbl});
            this.statusStrip.Location = new System.Drawing.Point(0, 565);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(548, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLbl
            // 
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(0, 17);
            // 
            // descriptionLbl
            // 
            this.descriptionLbl.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.descriptionLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.descriptionLbl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.descriptionLbl.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.descriptionLbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.descriptionLbl.Location = new System.Drawing.Point(0, 0);
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Padding = new System.Windows.Forms.Padding(0, 455, 0, 0);
            this.descriptionLbl.Size = new System.Drawing.Size(548, 587);
            this.descriptionLbl.TabIndex = 7;
            this.descriptionLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // addModBtn
            // 
            this.addModBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addModBtn.Location = new System.Drawing.Point(433, 480);
            this.addModBtn.Name = "addModBtn";
            this.addModBtn.Size = new System.Drawing.Size(99, 33);
            this.addModBtn.TabIndex = 7;
            this.addModBtn.Text = "+";
            this.addModBtn.UseVisualStyleBackColor = true;
            // 
            // rmModBtn
            // 
            this.rmModBtn.Enabled = false;
            this.rmModBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rmModBtn.Location = new System.Drawing.Point(17, 480);
            this.rmModBtn.Name = "rmModBtn";
            this.rmModBtn.Size = new System.Drawing.Size(99, 33);
            this.rmModBtn.TabIndex = 5;
            this.rmModBtn.Text = "-";
            this.rmModBtn.UseVisualStyleBackColor = true;
            // 
            // playBtn
            // 
            this.playBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.playBtn.Location = new System.Drawing.Point(433, 519);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(99, 43);
            this.playBtn.TabIndex = 4;
            this.playBtn.Text = "&Play";
            this.playBtn.UseVisualStyleBackColor = true;
            // 
            // saveBtn
            // 
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveBtn.Location = new System.Drawing.Point(17, 519);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(99, 43);
            this.saveBtn.TabIndex = 3;
            this.saveBtn.Text = "&Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel1.Location = new System.Drawing.Point(180, 246);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.linkLabel1.Size = new System.Drawing.Size(198, 28);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Open Mod Folder";
            // 
            // mainFrm
            // 
            this.AcceptButton = this.saveAndPlayBtn;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(548, 587);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.rmModBtn);
            this.Controls.Add(this.addModBtn);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.refreshBtn);
            this.Controls.Add(this.mainPnl);
            this.Controls.Add(this.saveAndPlayBtn);
            this.Controls.Add(this.descriptionLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLW Mod Loader";
            this.mainPnl.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.modPage.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.settingsPage.ResumeLayout(false);
            this.settingsBottomPnl.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView modsList;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader versionColumn;
        private System.Windows.Forms.ColumnHeader authorColumn;
        private System.Windows.Forms.ColumnHeader saveColumn;
        private System.Windows.Forms.Panel mainPnl;
        private System.Windows.Forms.Button saveAndPlayBtn;
        private System.Windows.Forms.Label noModsFoundLbl;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button moveDownbtn;
        private System.Windows.Forms.Button moveUpbtn;
        private System.Windows.Forms.Button moveDownAll;
        private System.Windows.Forms.Button moveUpAll;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLbl;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage modPage;
        private System.Windows.Forms.TabPage settingsPage;
        private System.Windows.Forms.Button aboutBtn;
        private System.Windows.Forms.LinkLabel descriptionLbl;
        private System.Windows.Forms.LinkLabel reportLbl;
        private System.Windows.Forms.Panel settingsBottomPnl;
        private System.Windows.Forms.Button addModBtn;
        private System.Windows.Forms.Button rmModBtn;
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

