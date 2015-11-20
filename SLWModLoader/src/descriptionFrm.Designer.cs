namespace SLWModLoader
{
    partial class descriptionFrm
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
            this.descriptionlbl = new System.Windows.Forms.Label();
            this.titlelbl = new System.Windows.Forms.LinkLabel();
            this.okbtn = new System.Windows.Forms.Button();
            this.authorlbl = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // descriptionlbl
            // 
            this.descriptionlbl.AutoSize = true;
            this.descriptionlbl.BackColor = System.Drawing.Color.Transparent;
            this.descriptionlbl.Location = new System.Drawing.Point(16, 91);
            this.descriptionlbl.Margin = new System.Windows.Forms.Padding(3, 0, 3, 50);
            this.descriptionlbl.MaximumSize = new System.Drawing.Size(730, 0);
            this.descriptionlbl.Name = "descriptionlbl";
            this.descriptionlbl.Size = new System.Drawing.Size(89, 20);
            this.descriptionlbl.TabIndex = 0;
            this.descriptionlbl.Text = "Description";
            // 
            // titlelbl
            // 
            this.titlelbl.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.titlelbl.AutoSize = true;
            this.titlelbl.BackColor = System.Drawing.Color.Transparent;
            this.titlelbl.Font = new System.Drawing.Font("Segoe UI", 17F);
            this.titlelbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titlelbl.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.titlelbl.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.titlelbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.titlelbl.Location = new System.Drawing.Point(12, 9);
            this.titlelbl.MaximumSize = new System.Drawing.Size(750, 46);
            this.titlelbl.Name = "titlelbl";
            this.titlelbl.Size = new System.Drawing.Size(84, 46);
            this.titlelbl.TabIndex = 1;
            this.titlelbl.Text = "Title";
            this.titlelbl.UseMnemonic = false;
            this.titlelbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.titlelbl_LinkClicked);
            // 
            // okbtn
            // 
            this.okbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okbtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okbtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okbtn.Location = new System.Drawing.Point(327, 131);
            this.okbtn.Margin = new System.Windows.Forms.Padding(3, 50, 3, 3);
            this.okbtn.Name = "okbtn";
            this.okbtn.Size = new System.Drawing.Size(75, 30);
            this.okbtn.TabIndex = 2;
            this.okbtn.Text = "&OK";
            this.okbtn.UseVisualStyleBackColor = true;
            this.okbtn.Click += new System.EventHandler(this.okbtn_Click);
            // 
            // authorlbl
            // 
            this.authorlbl.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.authorlbl.AutoSize = true;
            this.authorlbl.BackColor = System.Drawing.Color.Transparent;
            this.authorlbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.authorlbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.authorlbl.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.authorlbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.authorlbl.Location = new System.Drawing.Point(20, 55);
            this.authorlbl.Name = "authorlbl";
            this.authorlbl.Size = new System.Drawing.Size(285, 21);
            this.authorlbl.TabIndex = 4;
            this.authorlbl.Text = "Made by Radfordhound on 11/19/2015";
            this.authorlbl.UseMnemonic = false;
            this.authorlbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.authorlbl_LinkClicked);
            // 
            // descriptionFrm
            // 
            this.AcceptButton = this.okbtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CancelButton = this.okbtn;
            this.ClientSize = new System.Drawing.Size(414, 173);
            this.Controls.Add(this.authorlbl);
            this.Controls.Add(this.okbtn);
            this.Controls.Add(this.titlelbl);
            this.Controls.Add(this.descriptionlbl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "descriptionFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About this mod";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label descriptionlbl;
        private System.Windows.Forms.LinkLabel titlelbl;
        private System.Windows.Forms.Button okbtn;
        private System.Windows.Forms.LinkLabel authorlbl;
    }
}