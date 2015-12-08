namespace SLWModLoader
{
    partial class DevNewModFrm
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
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Main", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Desc", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "IncludeDir0",
            "."}, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "IncludeDirCount",
            "1"}, -1);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
            "UpdateServer"}, -1, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0))))), System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
            "Title",
            ""}, -1, System.Drawing.Color.Black, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
            "Description",
            ""}, -1, System.Drawing.Color.Red, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
            "Version",
            "1.0"}, -1);
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(new string[] {
            "Date",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem(new string[] {
            "Author"}, -1, System.Drawing.Color.Red, System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem(new string[] {
            "AuthorURL"}, -1, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0))))), System.Drawing.Color.Empty, null);
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem(new string[] {
            "URL"}, -1, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0))))), System.Drawing.Color.Empty, null);
            this.label = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rmvBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.editBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Segoe UI", 17F);
            this.label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label.Location = new System.Drawing.Point(15, 9);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(672, 46);
            this.label.TabIndex = 2;
            this.label.Text = "Please fill out some details about your mod.";
            // 
            // okBtn
            // 
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(23, 546);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(664, 36);
            this.okBtn.TabIndex = 4;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            listViewGroup3.Header = "Main";
            listViewGroup3.Name = "listViewGroup1";
            listViewGroup4.Header = "Desc";
            listViewGroup4.Name = "listViewGroup2";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            listViewItem11.Group = listViewGroup3;
            listViewItem11.Tag = "String";
            listViewItem12.Group = listViewGroup3;
            listViewItem12.Tag = "Integer";
            listViewItem13.Group = listViewGroup3;
            listViewItem13.Tag = "String";
            listViewItem14.Group = listViewGroup4;
            listViewItem14.Tag = "String";
            listViewItem15.Group = listViewGroup4;
            listViewItem15.Tag = "String";
            listViewItem16.Group = listViewGroup4;
            listViewItem16.Tag = "String";
            listViewItem17.Group = listViewGroup4;
            listViewItem17.Tag = "String";
            listViewItem18.Group = listViewGroup4;
            listViewItem18.Tag = "String";
            listViewItem19.Group = listViewGroup4;
            listViewItem19.Tag = "String";
            listViewItem20.Group = listViewGroup4;
            listViewItem20.Tag = "String";
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20});
            this.listView1.Location = new System.Drawing.Point(23, 77);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(664, 422);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Property Name";
            this.columnHeader1.Width = 545;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 109;
            // 
            // rmvBtn
            // 
            this.rmvBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rmvBtn.Location = new System.Drawing.Point(23, 505);
            this.rmvBtn.Name = "rmvBtn";
            this.rmvBtn.Size = new System.Drawing.Size(165, 36);
            this.rmvBtn.TabIndex = 6;
            this.rmvBtn.Text = "-";
            this.rmvBtn.UseVisualStyleBackColor = true;
            this.rmvBtn.Click += new System.EventHandler(this.rmvBtn_Click);
            // 
            // addBtn
            // 
            this.addBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addBtn.Location = new System.Drawing.Point(522, 504);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(165, 36);
            this.addBtn.TabIndex = 7;
            this.addBtn.Text = "+";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // editBtn
            // 
            this.editBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.editBtn.Location = new System.Drawing.Point(194, 504);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(322, 36);
            this.editBtn.TabIndex = 8;
            this.editBtn.Text = "&Edit Property";
            this.editBtn.UseVisualStyleBackColor = true;
            this.editBtn.Click += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // DevNewModFrm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(702, 594);
            this.Controls.Add(this.editBtn);
            this.Controls.Add(this.addBtn);
            this.Controls.Add(this.rmvBtn);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DevNewModFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLW Mod Loader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button rmvBtn;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button editBtn;
    }
}