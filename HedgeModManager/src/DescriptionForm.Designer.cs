namespace HedgeModManager
{
    partial class DescriptionForm
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
            this.descriptionLbl.Location = new System.Drawing.Point(11, 59);
            this.descriptionLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 32);
            this.descriptionLbl.MaximumSize = new System.Drawing.Size(487, 0);
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Size = new System.Drawing.Size(60, 13);
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
            this.titleLbl.Location = new System.Drawing.Point(8, 6);
            this.titleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLbl.MaximumSize = new System.Drawing.Size(500, 90);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(58, 31);
            this.titleLbl.TabIndex = 1;
            this.titleLbl.Text = "Title";
            this.titleLbl.UseMnemonic = false;
            this.titleLbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(218, 85);
            this.okBtn.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 19);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.Okbtn_Click);
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
            this.authorLbl.Location = new System.Drawing.Point(13, 36);
            this.authorLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.authorLbl.Name = "authorLbl";
            this.authorLbl.Size = new System.Drawing.Size(169, 19);
            this.authorLbl.TabIndex = 4;
            this.authorLbl.TabStop = true;
            this.authorLbl.Text = "Made by Mod Guy on 99/99/9999";
            this.authorLbl.UseCompatibleTextRendering = true;
            this.authorLbl.UseMnemonic = false;
            this.authorLbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            // 
            // DescriptionForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(276, 112);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.authorLbl);
            this.Controls.Add(this.titleLbl);
            this.Controls.Add(this.descriptionLbl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DescriptionForm";
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