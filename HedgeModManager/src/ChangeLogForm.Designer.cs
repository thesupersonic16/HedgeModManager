namespace HedgeModManager
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
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Button_Update = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(112, 9);
            this.titleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(247, 38);
            this.titleLbl.TabIndex = 0;
            this.titleLbl.Text = "HedgeModManager v";
            // 
            // changelogLbl
            // 
            this.changelogLbl.AutoSize = true;
            this.changelogLbl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.changelogLbl.Location = new System.Drawing.Point(17, 63);
            this.changelogLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 32);
            this.changelogLbl.MaximumSize = new System.Drawing.Size(512, 0);
            this.changelogLbl.Name = "changelogLbl";
            this.changelogLbl.Size = new System.Drawing.Size(139, 19);
            this.changelogLbl.TabIndex = 1;
            this.changelogLbl.Text = "Changelog goes here";
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_Cancel.Location = new System.Drawing.Point(224, 102);
            this.Button_Cancel.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(120, 28);
            this.Button_Cancel.TabIndex = 3;
            this.Button_Cancel.Text = "Cancel Update";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            this.Button_Cancel.Click += new System.EventHandler(this.Button_Click);
            // 
            // Button_Update
            // 
            this.Button_Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Update.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Button_Update.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_Update.Location = new System.Drawing.Point(348, 102);
            this.Button_Update.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.Button_Update.Name = "Button_Update";
            this.Button_Update.Size = new System.Drawing.Size(120, 28);
            this.Button_Update.TabIndex = 0;
            this.Button_Update.Text = "Start Update Now";
            this.Button_Update.UseVisualStyleBackColor = true;
            this.Button_Update.Click += new System.EventHandler(this.Button_Click);
            // 
            // ChangeLogForm
            // 
            this.AcceptButton = this.Button_Cancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.Button_Cancel;
            this.ClientSize = new System.Drawing.Size(479, 141);
            this.Controls.Add(this.Button_Update);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.changelogLbl);
            this.Controls.Add(this.titleLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeLogForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "v1.0 Changelog";
            this.Load += new System.EventHandler(this.ChangeLogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.Label changelogLbl;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Button Button_Update;
    }
}