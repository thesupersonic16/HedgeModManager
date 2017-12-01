namespace HedgeModManager
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.titleLbl = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.iconPBx = new System.Windows.Forms.PictureBox();
            this.gitLbl = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.iconPBx)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(57, 10);
            this.titleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(266, 38);
            this.titleLbl.TabIndex = 0;
            this.titleLbl.Text = "HedgeModManager";
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(391, 242);
            this.okBtn.Margin = new System.Windows.Forms.Padding(2);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 21);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // iconPBx
            // 
            this.iconPBx.Image = global::HedgeModManager.Properties.Resources.icon96;
            this.iconPBx.Location = new System.Drawing.Point(8, 6);
            this.iconPBx.Margin = new System.Windows.Forms.Padding(2);
            this.iconPBx.Name = "iconPBx";
            this.iconPBx.Size = new System.Drawing.Size(45, 44);
            this.iconPBx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iconPBx.TabIndex = 2;
            this.iconPBx.TabStop = false;
            // 
            // gitLbl
            // 
            this.gitLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gitLbl.AutoSize = true;
            this.gitLbl.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.gitLbl.Location = new System.Drawing.Point(61, 246);
            this.gitLbl.Name = "gitLbl";
            this.gitLbl.Size = new System.Drawing.Size(66, 13);
            this.gitLbl.TabIndex = 4;
            this.gitLbl.TabStop = true;
            this.gitLbl.Text = "Github Page";
            this.gitLbl.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.gitLbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GitLbl_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.linkLabel1.Location = new System.Drawing.Point(61, 53);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(386, 165);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.Text = resources.GetString("linkLabel1.Text");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(477, 273);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.gitLbl);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.iconPBx);
            this.Controls.Add(this.titleLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About HedgeModManager";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconPBx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.PictureBox iconPBx;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.LinkLabel gitLbl;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}