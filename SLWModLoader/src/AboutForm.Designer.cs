namespace SLWModLoader
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
            this.aboutLbl = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.iconPBx = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.iconPBx)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.titleLbl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.titleLbl.Location = new System.Drawing.Point(85, 15);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(344, 57);
            this.titleLbl.TabIndex = 0;
            this.titleLbl.Text = "SLW Mod Loader";
            // 
            // aboutLbl
            // 
            this.aboutLbl.AutoSize = true;
            this.aboutLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.aboutLbl.Location = new System.Drawing.Point(91, 85);
            this.aboutLbl.Name = "aboutLbl";
            this.aboutLbl.Size = new System.Drawing.Size(499, 126);
            this.aboutLbl.TabIndex = 1;
            this.aboutLbl.Text = resources.GetString("aboutLbl.Text");
            // 
            // okBtn
            // 
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(591, 217);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(113, 33);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // iconPBx
            // 
            this.iconPBx.Image = global::SLWModLoader.Properties.Resources.icon96;
            this.iconPBx.Location = new System.Drawing.Point(12, 9);
            this.iconPBx.Name = "iconPBx";
            this.iconPBx.Size = new System.Drawing.Size(67, 67);
            this.iconPBx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iconPBx.TabIndex = 2;
            this.iconPBx.TabStop = false;
            // 
            // AboutFrm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.okBtn;
            this.ClientSize = new System.Drawing.Size(716, 258);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.iconPBx);
            this.Controls.Add(this.aboutLbl);
            this.Controls.Add(this.titleLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About SLW Mod Loader";
            ((System.ComponentModel.ISupportInitialize)(this.iconPBx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.Label aboutLbl;
        private System.Windows.Forms.PictureBox iconPBx;
        private System.Windows.Forms.Button okBtn;
    }
}