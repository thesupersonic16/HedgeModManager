using HedgeModManager.Properties;

namespace HedgeModManager
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
            this.components = new System.ComponentModel.Container();
            this.ModsListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.desciptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createModUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.ModPage = new System.Windows.Forms.TabPage();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.NoModsFoundLabel = new System.Windows.Forms.Label();
            this.ModsList = new System.Windows.Forms.ListView();
            this.NameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VersionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AuthorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SaveColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UpdateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MoveDownAll = new System.Windows.Forms.Button();
            this.MoveUpAll = new System.Windows.Forms.Button();
            this.MoveDownButton = new System.Windows.Forms.Button();
            this.MoveUpButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GetCodeList_Button = new System.Windows.Forms.Button();
            this.InstallLoader_Button = new System.Windows.Forms.Button();
            this.Codes_CheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.PatchesTab = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ApplyPatches_Button = new System.Windows.Forms.Button();
            this.Patches_CheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.SettingsPage = new System.Windows.Forms.TabPage();
            this.LoaderVerLabel = new System.Windows.Forms.Label();
            this.Label_SaveFileBackupStatus = new System.Windows.Forms.Label();
            this.Button_RestoreSaveFile = new System.Windows.Forms.Button();
            this.ModOrderButton = new System.Windows.Forms.Button();
            this.Button_BackupSaveFile = new System.Windows.Forms.Button();
            this.Button_SaveAndReload = new System.Windows.Forms.Button();
            this.Label_CustomModsDirectory = new System.Windows.Forms.Label();
            this.TextBox_CustomModsDirectory = new System.Windows.Forms.TextBox();
            this.CheckBox_CustomModsDirectory = new System.Windows.Forms.CheckBox();
            this.EnableSaveFileRedirectionCheckBox = new System.Windows.Forms.CheckBox();
            this.EnableCPKREDIRConsoleCheckBox = new System.Windows.Forms.CheckBox();
            this.KeepModLoaderOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoCheckUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.InstallUninstallButton = new System.Windows.Forms.Button();
            this.PatchLabel = new System.Windows.Forms.Label();
            this.ScanExecutableButton = new System.Windows.Forms.Button();
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
            this.Search_TextBox = new System.Windows.Forms.TextBox();
            this.GameSelecterComboBox = new System.Windows.Forms.ComboBox();
            this.ModsListContextMenu.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.ModPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.PatchesTab.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SettingsPage.SuspendLayout();
            this.SettingsBottomPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ModsListContextMenu
            // 
            this.ModsListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.desciptionToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.openModFolderToolStripMenuItem,
            this.editModToolStripMenuItem,
            this.deleteModToolStripMenuItem,
            this.createModUpdateToolStripMenuItem});
            this.ModsListContextMenu.Name = "ModsListContextMenu";
            this.ModsListContextMenu.Size = new System.Drawing.Size(204, 136);
            // 
            // desciptionToolStripMenuItem
            // 
            this.desciptionToolStripMenuItem.Enabled = false;
            this.desciptionToolStripMenuItem.Name = "desciptionToolStripMenuItem";
            this.desciptionToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.desciptionToolStripMenuItem.Text = "Description";
            this.desciptionToolStripMenuItem.Click += new System.EventHandler(this.DesciptionToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Enabled = false;
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItem_Click);
            // 
            // openModFolderToolStripMenuItem
            // 
            this.openModFolderToolStripMenuItem.Enabled = false;
            this.openModFolderToolStripMenuItem.Name = "openModFolderToolStripMenuItem";
            this.openModFolderToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openModFolderToolStripMenuItem.Text = "Open Containing Folder";
            this.openModFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenModFolderToolStripMenuItem_Click);
            // 
            // editModToolStripMenuItem
            // 
            this.editModToolStripMenuItem.Enabled = false;
            this.editModToolStripMenuItem.Name = "editModToolStripMenuItem";
            this.editModToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.editModToolStripMenuItem.Text = "Edit Mod";
            this.editModToolStripMenuItem.Click += new System.EventHandler(this.EditModToolStripMenuItem_Click);
            // 
            // deleteModToolStripMenuItem
            // 
            this.deleteModToolStripMenuItem.Enabled = false;
            this.deleteModToolStripMenuItem.Name = "deleteModToolStripMenuItem";
            this.deleteModToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.deleteModToolStripMenuItem.Text = "Delete Mod";
            this.deleteModToolStripMenuItem.Click += new System.EventHandler(this.DeleteModToolStripMenuItem_Click);
            // 
            // createModUpdateToolStripMenuItem
            // 
            this.createModUpdateToolStripMenuItem.Enabled = false;
            this.createModUpdateToolStripMenuItem.Name = "createModUpdateToolStripMenuItem";
            this.createModUpdateToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.createModUpdateToolStripMenuItem.Text = "Create Mod Update Files";
            this.createModUpdateToolStripMenuItem.Click += new System.EventHandler(this.CreateModUpdateToolStripMenuItem_Click);
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
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.PatchesTab);
            this.TabControl.Controls.Add(this.SettingsPage);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(537, 444);
            this.TabControl.TabIndex = 0;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
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
            this.splitContainer.Panel1.Controls.Add(this.label1);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.label1.Location = new System.Drawing.Point(61, 370);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(385, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Tip: Try draging mod archives into this Window";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel1.Location = new System.Drawing.Point(169, 246);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.linkLabel1.Size = new System.Drawing.Size(154, 19);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Open Mods Folder";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // NoModsFoundLabel
            // 
            this.NoModsFoundLabel.AllowDrop = true;
            this.NoModsFoundLabel.BackColor = System.Drawing.Color.Transparent;
            this.NoModsFoundLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NoModsFoundLabel.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.NoModsFoundLabel.ForeColor = System.Drawing.Color.Red;
            this.NoModsFoundLabel.Location = new System.Drawing.Point(0, 0);
            this.NoModsFoundLabel.Name = "NoModsFoundLabel";
            this.NoModsFoundLabel.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.NoModsFoundLabel.Size = new System.Drawing.Size(490, 412);
            this.NoModsFoundLabel.TabIndex = 4;
            this.NoModsFoundLabel.Text = "No mods were found!\r\nPlease check your mods folder.";
            this.NoModsFoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NoModsFoundLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.ModsList_DragDrop);
            this.NoModsFoundLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.ModsList_DragEnter);
            // 
            // ModsList
            // 
            this.ModsList.AllowDrop = true;
            this.ModsList.CheckBoxes = true;
            this.ModsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.VersionColumn,
            this.AuthorColumn,
            this.SaveColumn,
            this.UpdateColumn});
            this.ModsList.ContextMenuStrip = this.ModsListContextMenu;
            this.ModsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModsList.FullRowSelect = true;
            this.ModsList.Location = new System.Drawing.Point(0, 0);
            this.ModsList.MultiSelect = false;
            this.ModsList.Name = "ModsList";
            this.ModsList.Size = new System.Drawing.Size(490, 412);
            this.ModsList.TabIndex = 0;
            this.ModsList.UseCompatibleStateImageBehavior = false;
            this.ModsList.View = System.Windows.Forms.View.Details;
            this.ModsList.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ModsList_DrawColumnHeader);
            this.ModsList.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ModsList_DrawItem);
            this.ModsList.SelectedIndexChanged += new System.EventHandler(this.ModsList_SelectedIndexChanged);
            this.ModsList.DragDrop += new System.Windows.Forms.DragEventHandler(this.ModsList_DragDrop);
            this.ModsList.DragEnter += new System.Windows.Forms.DragEventHandler(this.ModsList_DragEnter);
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Name";
            this.NameColumn.Width = 175;
            // 
            // VersionColumn
            // 
            this.VersionColumn.Text = "V";
            this.VersionColumn.Width = 48;
            // 
            // AuthorColumn
            // 
            this.AuthorColumn.Text = "Author";
            this.AuthorColumn.Width = 113;
            // 
            // SaveColumn
            // 
            this.SaveColumn.Text = "Supports Save";
            this.SaveColumn.Width = 74;
            // 
            // UpdateColumn
            // 
            this.UpdateColumn.Text = "Updates";
            this.UpdateColumn.Width = 59;
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
            this.MoveDownAll.Click += new System.EventHandler(this.MoveDownAll_Click);
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
            this.MoveUpAll.Click += new System.EventHandler(this.MoveUpAll_Click);
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
            this.MoveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
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
            this.MoveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.Codes_CheckedListBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(529, 418);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Codes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.GetCodeList_Button);
            this.panel1.Controls.Add(this.InstallLoader_Button);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 365);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(523, 50);
            this.panel1.TabIndex = 1;
            // 
            // GetCodeList_Button
            // 
            this.GetCodeList_Button.Location = new System.Drawing.Point(15, 12);
            this.GetCodeList_Button.Name = "GetCodeList_Button";
            this.GetCodeList_Button.Size = new System.Drawing.Size(123, 23);
            this.GetCodeList_Button.TabIndex = 27;
            this.GetCodeList_Button.Text = "Update Code List";
            this.GetCodeList_Button.UseVisualStyleBackColor = true;
            this.GetCodeList_Button.Click += new System.EventHandler(this.GetCodeList_Button_Click);
            // 
            // InstallLoader_Button
            // 
            this.InstallLoader_Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.InstallLoader_Button.Location = new System.Drawing.Point(374, 13);
            this.InstallLoader_Button.Name = "InstallLoader_Button";
            this.InstallLoader_Button.Size = new System.Drawing.Size(130, 23);
            this.InstallLoader_Button.TabIndex = 26;
            this.InstallLoader_Button.Text = "Install Code Loader";
            this.InstallLoader_Button.UseVisualStyleBackColor = true;
            this.InstallLoader_Button.Click += new System.EventHandler(this.InstallLoader_Button_Click);
            // 
            // Codes_CheckedListBox
            // 
            this.Codes_CheckedListBox.FormattingEnabled = true;
            this.Codes_CheckedListBox.Location = new System.Drawing.Point(0, 0);
            this.Codes_CheckedListBox.Name = "Codes_CheckedListBox";
            this.Codes_CheckedListBox.Size = new System.Drawing.Size(530, 364);
            this.Codes_CheckedListBox.TabIndex = 0;
            // 
            // PatchesTab
            // 
            this.PatchesTab.Controls.Add(this.panel2);
            this.PatchesTab.Controls.Add(this.Patches_CheckedListBox);
            this.PatchesTab.Location = new System.Drawing.Point(4, 22);
            this.PatchesTab.Name = "PatchesTab";
            this.PatchesTab.Padding = new System.Windows.Forms.Padding(3);
            this.PatchesTab.Size = new System.Drawing.Size(529, 418);
            this.PatchesTab.TabIndex = 3;
            this.PatchesTab.Text = "Patches";
            this.PatchesTab.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ApplyPatches_Button);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 365);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(523, 50);
            this.panel2.TabIndex = 3;
            // 
            // ApplyPatches_Button
            // 
            this.ApplyPatches_Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ApplyPatches_Button.Location = new System.Drawing.Point(374, 13);
            this.ApplyPatches_Button.Name = "ApplyPatches_Button";
            this.ApplyPatches_Button.Size = new System.Drawing.Size(130, 23);
            this.ApplyPatches_Button.TabIndex = 26;
            this.ApplyPatches_Button.Text = "Apply Patches";
            this.ApplyPatches_Button.UseVisualStyleBackColor = true;
            this.ApplyPatches_Button.Click += new System.EventHandler(this.ApplyPatches_Button_Click);
            // 
            // Patches_CheckedListBox
            // 
            this.Patches_CheckedListBox.FormattingEnabled = true;
            this.Patches_CheckedListBox.Location = new System.Drawing.Point(0, 0);
            this.Patches_CheckedListBox.Name = "Patches_CheckedListBox";
            this.Patches_CheckedListBox.Size = new System.Drawing.Size(530, 364);
            this.Patches_CheckedListBox.TabIndex = 2;
            // 
            // SettingsPage
            // 
            this.SettingsPage.Controls.Add(this.LoaderVerLabel);
            this.SettingsPage.Controls.Add(this.Label_SaveFileBackupStatus);
            this.SettingsPage.Controls.Add(this.Button_RestoreSaveFile);
            this.SettingsPage.Controls.Add(this.ModOrderButton);
            this.SettingsPage.Controls.Add(this.Button_BackupSaveFile);
            this.SettingsPage.Controls.Add(this.Button_SaveAndReload);
            this.SettingsPage.Controls.Add(this.Label_CustomModsDirectory);
            this.SettingsPage.Controls.Add(this.TextBox_CustomModsDirectory);
            this.SettingsPage.Controls.Add(this.CheckBox_CustomModsDirectory);
            this.SettingsPage.Controls.Add(this.EnableSaveFileRedirectionCheckBox);
            this.SettingsPage.Controls.Add(this.EnableCPKREDIRConsoleCheckBox);
            this.SettingsPage.Controls.Add(this.KeepModLoaderOpenCheckBox);
            this.SettingsPage.Controls.Add(this.AutoCheckUpdateCheckBox);
            this.SettingsPage.Controls.Add(this.InstallUninstallButton);
            this.SettingsPage.Controls.Add(this.PatchLabel);
            this.SettingsPage.Controls.Add(this.ScanExecutableButton);
            this.SettingsPage.Controls.Add(this.SettingsBottomPanel);
            this.SettingsPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsPage.Name = "SettingsPage";
            this.SettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.SettingsPage.Size = new System.Drawing.Size(529, 418);
            this.SettingsPage.TabIndex = 1;
            this.SettingsPage.Text = "Settings";
            this.SettingsPage.UseVisualStyleBackColor = true;
            // 
            // LoaderVerLabel
            // 
            this.LoaderVerLabel.Location = new System.Drawing.Point(150, 53);
            this.LoaderVerLabel.Name = "LoaderVerLabel";
            this.LoaderVerLabel.Size = new System.Drawing.Size(227, 13);
            this.LoaderVerLabel.TabIndex = 28;
            this.LoaderVerLabel.Text = "Loader Version: Unknown";
            // 
            // Label_SaveFileBackupStatus
            // 
            this.Label_SaveFileBackupStatus.Location = new System.Drawing.Point(369, 106);
            this.Label_SaveFileBackupStatus.Name = "Label_SaveFileBackupStatus";
            this.Label_SaveFileBackupStatus.Size = new System.Drawing.Size(146, 26);
            this.Label_SaveFileBackupStatus.TabIndex = 27;
            this.Label_SaveFileBackupStatus.Text = "SaveFile Backup Status:\r\n    Unknown";
            this.Label_SaveFileBackupStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label_SaveFileBackupStatus.Visible = false;
            // 
            // Button_RestoreSaveFile
            // 
            this.Button_RestoreSaveFile.Location = new System.Drawing.Point(377, 77);
            this.Button_RestoreSaveFile.Name = "Button_RestoreSaveFile";
            this.Button_RestoreSaveFile.Size = new System.Drawing.Size(128, 23);
            this.Button_RestoreSaveFile.TabIndex = 26;
            this.Button_RestoreSaveFile.Text = "Restore SaveFile";
            this.Button_RestoreSaveFile.UseVisualStyleBackColor = true;
            this.Button_RestoreSaveFile.Visible = false;
            this.Button_RestoreSaveFile.Click += new System.EventHandler(this.Button_RestoreSaveFile_Click);
            // 
            // ModOrderButton
            // 
            this.ModOrderButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ModOrderButton.Location = new System.Drawing.Point(377, 19);
            this.ModOrderButton.Name = "ModOrderButton";
            this.ModOrderButton.Size = new System.Drawing.Size(128, 23);
            this.ModOrderButton.TabIndex = 6;
            this.ModOrderButton.Text = "Priority: Bottom to Top";
            this.ModOrderButton.UseVisualStyleBackColor = true;
            this.ModOrderButton.Click += new System.EventHandler(this.ModOrderButton_Click);
            // 
            // Button_BackupSaveFile
            // 
            this.Button_BackupSaveFile.Location = new System.Drawing.Point(377, 48);
            this.Button_BackupSaveFile.Name = "Button_BackupSaveFile";
            this.Button_BackupSaveFile.Size = new System.Drawing.Size(128, 23);
            this.Button_BackupSaveFile.TabIndex = 25;
            this.Button_BackupSaveFile.Text = "Backup SaveFile";
            this.Button_BackupSaveFile.UseVisualStyleBackColor = true;
            this.Button_BackupSaveFile.Visible = false;
            this.Button_BackupSaveFile.Click += new System.EventHandler(this.Button_BackupSaveFile_Click);
            // 
            // Button_SaveAndReload
            // 
            this.Button_SaveAndReload.Location = new System.Drawing.Point(377, 169);
            this.Button_SaveAndReload.Name = "Button_SaveAndReload";
            this.Button_SaveAndReload.Size = new System.Drawing.Size(128, 23);
            this.Button_SaveAndReload.TabIndex = 24;
            this.Button_SaveAndReload.Text = "Save and Reload";
            this.Button_SaveAndReload.UseVisualStyleBackColor = true;
            this.Button_SaveAndReload.Click += new System.EventHandler(this.Button_SaveAndReload_Click);
            // 
            // Label_CustomModsDirectory
            // 
            this.Label_CustomModsDirectory.AutoSize = true;
            this.Label_CustomModsDirectory.Location = new System.Drawing.Point(61, 201);
            this.Label_CustomModsDirectory.Name = "Label_CustomModsDirectory";
            this.Label_CustomModsDirectory.Size = new System.Drawing.Size(106, 13);
            this.Label_CustomModsDirectory.TabIndex = 23;
            this.Label_CustomModsDirectory.Text = "Custom Mods Folder:";
            // 
            // TextBox_CustomModsDirectory
            // 
            this.TextBox_CustomModsDirectory.Enabled = false;
            this.TextBox_CustomModsDirectory.Location = new System.Drawing.Point(170, 198);
            this.TextBox_CustomModsDirectory.Name = "TextBox_CustomModsDirectory";
            this.TextBox_CustomModsDirectory.Size = new System.Drawing.Size(335, 20);
            this.TextBox_CustomModsDirectory.TabIndex = 22;
            this.TextBox_CustomModsDirectory.DoubleClick += new System.EventHandler(this.TextBox_CustomModsDirectory_DoubleClick);
            // 
            // CheckBox_CustomModsDirectory
            // 
            this.CheckBox_CustomModsDirectory.AutoSize = true;
            this.CheckBox_CustomModsDirectory.Location = new System.Drawing.Point(33, 175);
            this.CheckBox_CustomModsDirectory.Name = "CheckBox_CustomModsDirectory";
            this.CheckBox_CustomModsDirectory.Size = new System.Drawing.Size(157, 17);
            this.CheckBox_CustomModsDirectory.TabIndex = 21;
            this.CheckBox_CustomModsDirectory.Text = "Use Custom Mods Directory";
            this.CheckBox_CustomModsDirectory.UseVisualStyleBackColor = true;
            this.CheckBox_CustomModsDirectory.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // EnableSaveFileRedirectionCheckBox
            // 
            this.EnableSaveFileRedirectionCheckBox.AutoSize = true;
            this.EnableSaveFileRedirectionCheckBox.Location = new System.Drawing.Point(33, 129);
            this.EnableSaveFileRedirectionCheckBox.Name = "EnableSaveFileRedirectionCheckBox";
            this.EnableSaveFileRedirectionCheckBox.Size = new System.Drawing.Size(163, 17);
            this.EnableSaveFileRedirectionCheckBox.TabIndex = 20;
            this.EnableSaveFileRedirectionCheckBox.Text = "Enable Save File Redirection";
            this.EnableSaveFileRedirectionCheckBox.UseVisualStyleBackColor = true;
            this.EnableSaveFileRedirectionCheckBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // EnableCPKREDIRConsoleCheckBox
            // 
            this.EnableCPKREDIRConsoleCheckBox.AutoSize = true;
            this.EnableCPKREDIRConsoleCheckBox.Location = new System.Drawing.Point(33, 152);
            this.EnableCPKREDIRConsoleCheckBox.Name = "EnableCPKREDIRConsoleCheckBox";
            this.EnableCPKREDIRConsoleCheckBox.Size = new System.Drawing.Size(158, 17);
            this.EnableCPKREDIRConsoleCheckBox.TabIndex = 19;
            this.EnableCPKREDIRConsoleCheckBox.Text = "Enable CPKREDIR Console";
            this.EnableCPKREDIRConsoleCheckBox.UseVisualStyleBackColor = true;
            this.EnableCPKREDIRConsoleCheckBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // KeepModLoaderOpenCheckBox
            // 
            this.KeepModLoaderOpenCheckBox.AutoSize = true;
            this.KeepModLoaderOpenCheckBox.Location = new System.Drawing.Point(33, 106);
            this.KeepModLoaderOpenCheckBox.Name = "KeepModLoaderOpenCheckBox";
            this.KeepModLoaderOpenCheckBox.Size = new System.Drawing.Size(275, 17);
            this.KeepModLoaderOpenCheckBox.TabIndex = 18;
            this.KeepModLoaderOpenCheckBox.Text = "Keep HedgeModManager open after starting a game";
            this.KeepModLoaderOpenCheckBox.UseVisualStyleBackColor = true;
            this.KeepModLoaderOpenCheckBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // AutoCheckUpdateCheckBox
            // 
            this.AutoCheckUpdateCheckBox.AutoSize = true;
            this.AutoCheckUpdateCheckBox.Location = new System.Drawing.Point(33, 83);
            this.AutoCheckUpdateCheckBox.Name = "AutoCheckUpdateCheckBox";
            this.AutoCheckUpdateCheckBox.Size = new System.Drawing.Size(160, 17);
            this.AutoCheckUpdateCheckBox.TabIndex = 17;
            this.AutoCheckUpdateCheckBox.Text = "Auto check for mod updates";
            this.AutoCheckUpdateCheckBox.UseVisualStyleBackColor = true;
            this.AutoCheckUpdateCheckBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // InstallUninstallButton
            // 
            this.InstallUninstallButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.InstallUninstallButton.Location = new System.Drawing.Point(18, 48);
            this.InstallUninstallButton.Name = "InstallUninstallButton";
            this.InstallUninstallButton.Size = new System.Drawing.Size(128, 23);
            this.InstallUninstallButton.TabIndex = 15;
            this.InstallUninstallButton.Text = "Install / Uninstall";
            this.InstallUninstallButton.UseVisualStyleBackColor = true;
            this.InstallUninstallButton.Click += new System.EventHandler(this.InstallUninstallButton_Click);
            // 
            // PatchLabel
            // 
            this.PatchLabel.Location = new System.Drawing.Point(150, 24);
            this.PatchLabel.Name = "PatchLabel";
            this.PatchLabel.Size = new System.Drawing.Size(227, 13);
            this.PatchLabel.TabIndex = 14;
            this.PatchLabel.Text = "Unknown Game: Unknown";
            // 
            // ScanExecutableButton
            // 
            this.ScanExecutableButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ScanExecutableButton.Location = new System.Drawing.Point(18, 19);
            this.ScanExecutableButton.Name = "ScanExecutableButton";
            this.ScanExecutableButton.Size = new System.Drawing.Size(128, 23);
            this.ScanExecutableButton.TabIndex = 13;
            this.ScanExecutableButton.Text = "Scan Executable";
            this.ScanExecutableButton.UseVisualStyleBackColor = true;
            this.ScanExecutableButton.Click += new System.EventHandler(this.ScanExecutableButton_Click);
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
            this.AboutButton.Text = "&About HedgeModManager";
            this.AboutButton.UseVisualStyleBackColor = true;
            this.AboutButton.Click += new System.EventHandler(this.AboutButton_Click);
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
            this.ReportLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReportLabel_LinkClicked);
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
            this.RefreshButton.UseVisualStyleBackColor = false;
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
            this.AddModButton.Text = "Add Mod";
            this.AddModButton.UseVisualStyleBackColor = true;
            this.AddModButton.Click += new System.EventHandler(this.AddModButton_Click);
            // 
            // RemoveModButton
            // 
            this.RemoveModButton.Enabled = false;
            this.RemoveModButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RemoveModButton.Location = new System.Drawing.Point(7, 450);
            this.RemoveModButton.Name = "RemoveModButton";
            this.RemoveModButton.Size = new System.Drawing.Size(99, 33);
            this.RemoveModButton.TabIndex = 5;
            this.RemoveModButton.Text = "Remove Mod";
            this.RemoveModButton.UseVisualStyleBackColor = true;
            this.RemoveModButton.Click += new System.EventHandler(this.RemoveModButton_Click);
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
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // Search_TextBox
            // 
            this.Search_TextBox.Location = new System.Drawing.Point(360, 0);
            this.Search_TextBox.Name = "Search_TextBox";
            this.Search_TextBox.Size = new System.Drawing.Size(177, 20);
            this.Search_TextBox.TabIndex = 6;
            this.Search_TextBox.TextChanged += new System.EventHandler(this.Search_TextBox_TextChanged);
            // 
            // GameSelecterComboBox
            // 
            this.GameSelecterComboBox.FormattingEnabled = true;
            this.GameSelecterComboBox.Location = new System.Drawing.Point(200, -1);
            this.GameSelecterComboBox.Name = "GameSelecterComboBox";
            this.GameSelecterComboBox.Size = new System.Drawing.Size(158, 21);
            this.GameSelecterComboBox.TabIndex = 6;
            this.GameSelecterComboBox.Visible = false;
            // 
            // MainForm
            // 
            this.AcceptButton = this.SaveAndPlayButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(537, 563);
            this.Controls.Add(this.Search_TextBox);
            this.Controls.Add(this.GameSelecterComboBox);
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
            this.Icon = global::HedgeModManager.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(553, 602);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HedgeModManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ModsListContextMenu.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.ModPage.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.PatchesTab.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.SettingsPage.ResumeLayout(false);
            this.SettingsPage.PerformLayout();
            this.SettingsBottomPanel.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.Button ScanExecutableButton;
        private System.Windows.Forms.Label PatchLabel;
        private System.Windows.Forms.Button InstallUninstallButton;
        private System.Windows.Forms.ContextMenuStrip ModsListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ListView ModsList;
        private System.Windows.Forms.ColumnHeader NameColumn;
        private System.Windows.Forms.ColumnHeader VersionColumn;
        private System.Windows.Forms.ColumnHeader AuthorColumn;
        private System.Windows.Forms.ColumnHeader SaveColumn;
        private System.Windows.Forms.ColumnHeader UpdateColumn;
        private System.Windows.Forms.ToolStripMenuItem openModFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem desciptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editModToolStripMenuItem;
        private System.Windows.Forms.CheckBox AutoCheckUpdateCheckBox;
        private System.Windows.Forms.CheckBox KeepModLoaderOpenCheckBox;
        private System.Windows.Forms.ToolStripMenuItem createModUpdateToolStripMenuItem;
		private System.Windows.Forms.CheckBox EnableCPKREDIRConsoleCheckBox;
		private System.Windows.Forms.CheckBox EnableSaveFileRedirectionCheckBox;
        private System.Windows.Forms.Label Label_CustomModsDirectory;
        private System.Windows.Forms.TextBox TextBox_CustomModsDirectory;
        private System.Windows.Forms.CheckBox CheckBox_CustomModsDirectory;
        private System.Windows.Forms.Button Button_SaveAndReload;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckedListBox Codes_CheckedListBox;
        private System.Windows.Forms.TextBox Search_TextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button InstallLoader_Button;
        private System.Windows.Forms.Button GetCodeList_Button;
        private System.Windows.Forms.Label Label_SaveFileBackupStatus;
        private System.Windows.Forms.Button Button_RestoreSaveFile;
        private System.Windows.Forms.Button Button_BackupSaveFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ModOrderButton;
        private System.Windows.Forms.ComboBox GameSelecterComboBox;
        private System.Windows.Forms.Label LoaderVerLabel;
        private System.Windows.Forms.TabPage PatchesTab;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button ApplyPatches_Button;
        private System.Windows.Forms.CheckedListBox Patches_CheckedListBox;
    }
}

