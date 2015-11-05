namespace SLWModLoader
{
    partial class Form1
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
            this.modslist = new System.Windows.Forms.ListView();
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.vcolumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.authorcolumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.savecolumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.updatedcolumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MoveDownAll = new System.Windows.Forms.Button();
            this.MoveUpAll = new System.Windows.Forms.Button();
            this.MoveDownbtn = new System.Windows.Forms.Button();
            this.MoveUpbtn = new System.Windows.Forms.Button();
            this.modsdirbtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.modsdir = new System.Windows.Forms.TextBox();
            this.nomodsfound = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.playbtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // modslist
            // 
            this.modslist.CheckBoxes = true;
            this.modslist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.vcolumn,
            this.authorcolumn,
            this.savecolumn,
            this.updatedcolumn});
            this.modslist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modslist.Location = new System.Drawing.Point(0, 0);
            this.modslist.MultiSelect = false;
            this.modslist.Name = "modslist";
            this.modslist.Size = new System.Drawing.Size(515, 409);
            this.modslist.TabIndex = 0;
            this.modslist.UseCompatibleStateImageBehavior = false;
            this.modslist.View = System.Windows.Forms.View.Details;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 175;
            // 
            // vcolumn
            // 
            this.vcolumn.Text = "V";
            this.vcolumn.Width = 31;
            // 
            // authorcolumn
            // 
            this.authorcolumn.Text = "Author";
            this.authorcolumn.Width = 119;
            // 
            // savecolumn
            // 
            this.savecolumn.Text = "Save";
            this.savecolumn.Width = 66;
            // 
            // updatedcolumn
            // 
            this.updatedcolumn.Text = "Updated";
            this.updatedcolumn.Width = 90;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Controls.Add(this.modsdirbtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.modsdir);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 50, 0, 0);
            this.panel1.Size = new System.Drawing.Size(548, 459);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.modslist);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.MoveDownAll);
            this.splitContainer1.Panel2.Controls.Add(this.MoveUpAll);
            this.splitContainer1.Panel2.Controls.Add(this.MoveDownbtn);
            this.splitContainer1.Panel2.Controls.Add(this.MoveUpbtn);
            this.splitContainer1.Size = new System.Drawing.Size(548, 409);
            this.splitContainer1.SplitterDistance = 515;
            this.splitContainer1.TabIndex = 5;
            // 
            // MoveDownAll
            // 
            this.MoveDownAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveDownAll.Location = new System.Drawing.Point(3, 306);
            this.MoveDownAll.Name = "MoveDownAll";
            this.MoveDownAll.Size = new System.Drawing.Size(19, 100);
            this.MoveDownAll.TabIndex = 3;
            this.MoveDownAll.Text = "↓";
            this.MoveDownAll.UseVisualStyleBackColor = true;
            this.MoveDownAll.Click += new System.EventHandler(this.MoveDownAll_Click);
            // 
            // MoveUpAll
            // 
            this.MoveUpAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUpAll.Location = new System.Drawing.Point(3, 3);
            this.MoveUpAll.Name = "MoveUpAll";
            this.MoveUpAll.Size = new System.Drawing.Size(19, 100);
            this.MoveUpAll.TabIndex = 2;
            this.MoveUpAll.Text = "↑";
            this.MoveUpAll.UseVisualStyleBackColor = true;
            this.MoveUpAll.Click += new System.EventHandler(this.MoveUpAll_Click);
            // 
            // MoveDownbtn
            // 
            this.MoveDownbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveDownbtn.Location = new System.Drawing.Point(3, 205);
            this.MoveDownbtn.Name = "MoveDownbtn";
            this.MoveDownbtn.Size = new System.Drawing.Size(19, 100);
            this.MoveDownbtn.TabIndex = 1;
            this.MoveDownbtn.Text = "▼";
            this.MoveDownbtn.UseVisualStyleBackColor = true;
            this.MoveDownbtn.Click += new System.EventHandler(this.MoveDownbtn_Click);
            // 
            // MoveUpbtn
            // 
            this.MoveUpbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveUpbtn.Location = new System.Drawing.Point(3, 104);
            this.MoveUpbtn.Name = "MoveUpbtn";
            this.MoveUpbtn.Size = new System.Drawing.Size(19, 100);
            this.MoveUpbtn.TabIndex = 0;
            this.MoveUpbtn.Text = "▲";
            this.MoveUpbtn.UseVisualStyleBackColor = true;
            this.MoveUpbtn.Click += new System.EventHandler(this.MoveUpbtn_Click);
            // 
            // modsdirbtn
            // 
            this.modsdirbtn.AutoSize = true;
            this.modsdirbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.modsdirbtn.Location = new System.Drawing.Point(467, 6);
            this.modsdirbtn.Name = "modsdirbtn";
            this.modsdirbtn.Size = new System.Drawing.Size(49, 29);
            this.modsdirbtn.TabIndex = 3;
            this.modsdirbtn.Text = "...";
            this.modsdirbtn.UseVisualStyleBackColor = true;
            this.modsdirbtn.Click += new System.EventHandler(this.modsdirbtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "SLW directory:";
            // 
            // modsdir
            // 
            this.modsdir.Location = new System.Drawing.Point(131, 9);
            this.modsdir.Name = "modsdir";
            this.modsdir.Size = new System.Drawing.Size(330, 26);
            this.modsdir.TabIndex = 1;
            this.modsdir.TextChanged += new System.EventHandler(this.modsdir_TextChanged);
            // 
            // nomodsfound
            // 
            this.nomodsfound.AutoSize = true;
            this.nomodsfound.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.nomodsfound.ForeColor = System.Drawing.Color.Red;
            this.nomodsfound.Location = new System.Drawing.Point(138, 201);
            this.nomodsfound.Name = "nomodsfound";
            this.nomodsfound.Size = new System.Drawing.Size(272, 56);
            this.nomodsfound.TabIndex = 4;
            this.nomodsfound.Text = "No mods found!\r\nPlease check your mod folder.";
            this.nomodsfound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.nomodsfound.Visible = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.linkLabel1.Location = new System.Drawing.Point(211, 271);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(127, 21);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Refresh Mod List";
            this.linkLabel1.Visible = false;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // playbtn
            // 
            this.playbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.playbtn.Location = new System.Drawing.Point(17, 476);
            this.playbtn.Name = "playbtn";
            this.playbtn.Size = new System.Drawing.Size(515, 43);
            this.playbtn.TabIndex = 3;
            this.playbtn.Text = "Play";
            this.playbtn.UseVisualStyleBackColor = true;
            this.playbtn.Click += new System.EventHandler(this.playbtn_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(124, 525);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(300, 33);
            this.button1.TabIndex = 4;
            this.button1.Text = "Refresh Mod List";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(548, 563);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.nomodsfound);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.playbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "SLW Mod Loader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView modslist;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader vcolumn;
        private System.Windows.Forms.ColumnHeader authorcolumn;
        private System.Windows.Forms.ColumnHeader savecolumn;
        private System.Windows.Forms.ColumnHeader updatedcolumn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox modsdir;
        private System.Windows.Forms.Button modsdirbtn;
        private System.Windows.Forms.Button playbtn;
        private System.Windows.Forms.Label nomodsfound;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button MoveDownbtn;
        private System.Windows.Forms.Button MoveUpbtn;
        private System.Windows.Forms.Button MoveDownAll;
        private System.Windows.Forms.Button MoveUpAll;
    }
}

