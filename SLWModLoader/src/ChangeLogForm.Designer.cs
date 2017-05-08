namespace SLWModLoader
{
    partial class ChangeLogForm
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
            this.titleLbl = new System.Windows.Forms.Label();
            this.changelogLbl = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.updateBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(1, 6);
            this.titleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(247, 38);
            this.titleLbl.TabIndex = 0;
            this.titleLbl.Text = "SLW Mod Loader v";
            // 
            // changelogLbl
            // 
            this.changelogLbl.AutoSize = true;
            this.changelogLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.changelogLbl.Location = new System.Drawing.Point(9, 51);
            this.changelogLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 32);
            this.changelogLbl.MaximumSize = new System.Drawing.Size(420, 0);
            this.changelogLbl.Name = "changelogLbl";
            this.changelogLbl.Size = new System.Drawing.Size(118, 13);
            this.changelogLbl.TabIndex = 1;
            this.changelogLbl.Text = "Changelog goes here";
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(174, 67);
            this.okBtn.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(120, 21);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "Cancel Update";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // updateBtn
            // 
            this.updateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.updateBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.updateBtn.Location = new System.Drawing.Point(298, 67);
            this.updateBtn.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(120, 21);
            this.updateBtn.TabIndex = 0;
            this.updateBtn.Text = "Start Update Now";
            this.updateBtn.UseVisualStyleBackColor = true;
            this.updateBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // ChangeLogForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(429, 99);
            this.Controls.Add(this.updateBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.changelogLbl);
            this.Controls.Add(this.titleLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeLogForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "v1.0 Changelog";
            this.Load += new System.EventHandler(this.ChangeLogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.Label changelogLbl;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button updateBtn;
    }
}