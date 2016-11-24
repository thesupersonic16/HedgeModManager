namespace SLWModLoader
{
    partial class AddModForm
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
            this.fileInstallRBtn = new System.Windows.Forms.RadioButton();
            this.titleLbl = new System.Windows.Forms.Label();
            this.folderInstallRBtn = new System.Windows.Forms.RadioButton();
            this.makingItRBtn = new System.Windows.Forms.RadioButton();
            this.okBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // fileInstallRBtn
            // 
            this.fileInstallRBtn.AutoSize = true;
            this.fileInstallRBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.fileInstallRBtn.Location = new System.Drawing.Point(22, 85);
            this.fileInstallRBtn.Name = "fileInstallRBtn";
            this.fileInstallRBtn.Size = new System.Drawing.Size(181, 25);
            this.fileInstallRBtn.TabIndex = 0;
            this.fileInstallRBtn.Text = "&Installing it from an archive";
            this.fileInstallRBtn.UseVisualStyleBackColor = true;
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(0, 0);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(405, 57);
            this.titleLbl.TabIndex = 1;
            this.titleLbl.Text = "Add a new mod by...";
            // 
            // folderInstallRBtn
            // 
            this.folderInstallRBtn.AutoSize = true;
            this.folderInstallRBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.folderInstallRBtn.Location = new System.Drawing.Point(22, 115);
            this.folderInstallRBtn.Name = "folderInstallRBtn";
            this.folderInstallRBtn.Size = new System.Drawing.Size(201, 25);
            this.folderInstallRBtn.TabIndex = 2;
            this.folderInstallRBtn.Text = "Installing it from a &folder";
            this.folderInstallRBtn.UseVisualStyleBackColor = true;
            // 
            // makingItRBtn
            // 
            this.makingItRBtn.AutoSize = true;
            this.makingItRBtn.Checked = true;
            this.makingItRBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.makingItRBtn.Location = new System.Drawing.Point(22, 145);
            this.makingItRBtn.Name = "makingItRBtn";
            this.makingItRBtn.Size = new System.Drawing.Size(210, 25);
            this.makingItRBtn.TabIndex = 3;
            this.makingItRBtn.TabStop = true;
            this.makingItRBtn.Text = "&Making one";
            this.makingItRBtn.UseVisualStyleBackColor = true;
            // 
            // okBtn
            // 
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.Location = new System.Drawing.Point(442, 163);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(96, 34);
            this.okBtn.TabIndex = 4;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // NewModFrm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(550, 209);
            this.Controls.Add(this.titleLbl);
            this.Controls.Add(this.fileInstallRBtn);
            this.Controls.Add(this.folderInstallRBtn);
            this.Controls.Add(this.makingItRBtn);
            this.Controls.Add(this.okBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewModFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLW Mod Loader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton fileInstallRBtn;
        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.RadioButton folderInstallRBtn;
        private System.Windows.Forms.RadioButton makingItRBtn;
        private System.Windows.Forms.Button okBtn;
    }
}