namespace HedgeModManager
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
            this.RadioButton_Archive = new System.Windows.Forms.RadioButton();
            this.titleLbl = new System.Windows.Forms.Label();
            this.RadioButton_Folder = new System.Windows.Forms.RadioButton();
            this.RadioButton_Make = new System.Windows.Forms.RadioButton();
            this.okBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RadioButton_Archive
            // 
            this.RadioButton_Archive.AutoSize = true;
            this.RadioButton_Archive.Checked = true;
            this.RadioButton_Archive.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RadioButton_Archive.Location = new System.Drawing.Point(15, 51);
            this.RadioButton_Archive.Margin = new System.Windows.Forms.Padding(2);
            this.RadioButton_Archive.Name = "RadioButton_Archive";
            this.RadioButton_Archive.Size = new System.Drawing.Size(156, 18);
            this.RadioButton_Archive.TabIndex = 0;
            this.RadioButton_Archive.TabStop = true;
            this.RadioButton_Archive.Text = "&Installing it from an archive";
            this.RadioButton_Archive.UseVisualStyleBackColor = true;
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(0, 0);
            this.titleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(268, 38);
            this.titleLbl.TabIndex = 1;
            this.titleLbl.Text = "Add a new mod by...";
            // 
            // RadioButton_Folder
            // 
            this.RadioButton_Folder.AutoSize = true;
            this.RadioButton_Folder.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RadioButton_Folder.Location = new System.Drawing.Point(15, 73);
            this.RadioButton_Folder.Margin = new System.Windows.Forms.Padding(2);
            this.RadioButton_Folder.Name = "RadioButton_Folder";
            this.RadioButton_Folder.Size = new System.Drawing.Size(141, 18);
            this.RadioButton_Folder.TabIndex = 1;
            this.RadioButton_Folder.Text = "Installing it from a &folder";
            this.RadioButton_Folder.UseVisualStyleBackColor = true;
            // 
            // RadioButton_Make
            // 
            this.RadioButton_Make.AutoSize = true;
            this.RadioButton_Make.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RadioButton_Make.Location = new System.Drawing.Point(15, 95);
            this.RadioButton_Make.Margin = new System.Windows.Forms.Padding(2);
            this.RadioButton_Make.Name = "RadioButton_Make";
            this.RadioButton_Make.Size = new System.Drawing.Size(87, 18);
            this.RadioButton_Make.TabIndex = 3;
            this.RadioButton_Make.Text = "&Making one";
            this.RadioButton_Make.UseVisualStyleBackColor = true;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.Location = new System.Drawing.Point(295, 122);
            this.okBtn.Margin = new System.Windows.Forms.Padding(2);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(64, 22);
            this.okBtn.TabIndex = 4;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // AddModForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(367, 152);
            this.Controls.Add(this.titleLbl);
            this.Controls.Add(this.RadioButton_Archive);
            this.Controls.Add(this.RadioButton_Folder);
            this.Controls.Add(this.RadioButton_Make);
            this.Controls.Add(this.okBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddModForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HedgeModManager";
            this.Load += new System.EventHandler(this.AddModForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton RadioButton_Archive;
        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.RadioButton RadioButton_Folder;
        private System.Windows.Forms.RadioButton RadioButton_Make;
        private System.Windows.Forms.Button okBtn;
    }
}