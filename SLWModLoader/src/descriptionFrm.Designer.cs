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
            this.descriptionLbl = new System.Windows.Forms.Label();
            this.titleLbl = new System.Windows.Forms.LinkLabel();
            this.okBtn = new System.Windows.Forms.Button();
            this.authorLbl = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // descriptionLbl
            // 
            this.descriptionLbl.AutoSize = true;
            this.descriptionLbl.BackColor = System.Drawing.Color.Transparent;
            this.descriptionLbl.Location = new System.Drawing.Point(16, 91);
            this.descriptionLbl.Margin = new System.Windows.Forms.Padding(3, 0, 3, 50);
            this.descriptionLbl.MaximumSize = new System.Drawing.Size(730, 0);
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Size = new System.Drawing.Size(89, 20);
            this.descriptionLbl.TabIndex = 0;
            this.descriptionLbl.Text = "Description";
            // 
            // titleLbl
            // 
            this.titleLbl.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.titleLbl.AutoSize = true;
            this.titleLbl.BackColor = System.Drawing.Color.Transparent;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 17F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.titleLbl.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.titleLbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(12, 9);
            this.titleLbl.MaximumSize = new System.Drawing.Size(750, 46);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(84, 46);
            this.titleLbl.TabIndex = 1;
            this.titleLbl.Text = "Title";
            this.titleLbl.UseMnemonic = false;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(327, 131);
            this.okBtn.Margin = new System.Windows.Forms.Padding(3, 50, 3, 3);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 30);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okbtn_Click);
            // 
            // authorLbl
            // 
            this.authorLbl.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            this.authorLbl.AutoSize = true;
            this.authorLbl.BackColor = System.Drawing.Color.Transparent;
            this.authorLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.authorLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.authorLbl.LinkArea = new System.Windows.Forms.LinkArea(8, 7);
            this.authorLbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.authorLbl.Location = new System.Drawing.Point(20, 55);
            this.authorLbl.Name = "authorLbl";
            this.authorLbl.Size = new System.Drawing.Size(253, 27);
            this.authorLbl.TabIndex = 4;
            this.authorLbl.TabStop = true;
            this.authorLbl.Text = "Made by Mod Guy on 99/99/9999";
            this.authorLbl.UseCompatibleTextRendering = true;
            this.authorLbl.UseMnemonic = false;
            // 
            // descriptionFrm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(414, 173);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.authorLbl);
            this.Controls.Add(this.titleLbl);
            this.Controls.Add(this.descriptionLbl);
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

        private System.Windows.Forms.Label descriptionLbl;
        private System.Windows.Forms.LinkLabel titleLbl;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.LinkLabel authorLbl;
    }
}