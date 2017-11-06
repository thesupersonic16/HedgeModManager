using HedgeModManager.Properties;

namespace HedgeModManager
{
    partial class DownloadModForm
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
            this.Button_Update = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.Description_Label = new System.Windows.Forms.Label();
            this.Title_Label = new System.Windows.Forms.Label();
            this.PictureBox_1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Credits_Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_1)).BeginInit();
            this.SuspendLayout();
            // 
            // Button_Update
            // 
            this.Button_Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Update.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Button_Update.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_Update.Location = new System.Drawing.Point(135, 297);
            this.Button_Update.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.Button_Update.Name = "Button_Update";
            this.Button_Update.Size = new System.Drawing.Size(120, 28);
            this.Button_Update.TabIndex = 4;
            this.Button_Update.Text = "Start Update Now";
            this.Button_Update.UseVisualStyleBackColor = true;
            this.Button_Update.Click += new System.EventHandler(this.Button_Update_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_Cancel.Location = new System.Drawing.Point(11, 297);
            this.Button_Cancel.Margin = new System.Windows.Forms.Padding(2, 32, 2, 2);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(120, 28);
            this.Button_Cancel.TabIndex = 7;
            this.Button_Cancel.Text = "Cancel Update";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // Description_Label
            // 
            this.Description_Label.AutoSize = true;
            this.Description_Label.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Description_Label.Location = new System.Drawing.Point(27, 47);
            this.Description_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 32);
            this.Description_Label.MaximumSize = new System.Drawing.Size(360, 0);
            this.Description_Label.Name = "Description_Label";
            this.Description_Label.Size = new System.Drawing.Size(78, 19);
            this.Description_Label.TabIndex = 6;
            this.Description_Label.Text = "Description";
            // 
            // Title_Label
            // 
            this.Title_Label.AutoSize = true;
            this.Title_Label.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.Title_Label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Title_Label.Location = new System.Drawing.Point(172, 9);
            this.Title_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Title_Label.Name = "Title_Label";
            this.Title_Label.Size = new System.Drawing.Size(173, 38);
            this.Title_Label.TabIndex = 5;
            this.Title_Label.Text = "MOD_NAME";
            // 
            // PictureBox_1
            // 
            this.PictureBox_1.Location = new System.Drawing.Point(416, 180);
            this.PictureBox_1.Name = "PictureBox_1";
            this.PictureBox_1.Size = new System.Drawing.Size(149, 145);
            this.PictureBox_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox_1.TabIndex = 8;
            this.PictureBox_1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(404, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 18);
            this.label1.TabIndex = 9;
            this.label1.Text = "CREDITS:";
            this.label1.Visible = false;
            // 
            // Credits_Label
            // 
            this.Credits_Label.AutoSize = true;
            this.Credits_Label.Location = new System.Drawing.Point(424, 47);
            this.Credits_Label.Name = "Credits_Label";
            this.Credits_Label.Size = new System.Drawing.Size(35, 13);
            this.Credits_Label.TabIndex = 10;
            this.Credits_Label.Text = "label2";
            this.Credits_Label.Visible = false;
            // 
            // DownloadModForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 336);
            this.Controls.Add(this.Credits_Label);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PictureBox_1);
            this.Controls.Add(this.Button_Update);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Description_Label);
            this.Controls.Add(this.Title_Label);
            this.Icon = global::HedgeModManager.Properties.Resources.icon;
            this.Name = "DownloadModForm";
            this.Text = "DownloadModForm";
            this.Load += new System.EventHandler(this.DownloadModForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_Update;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label Description_Label;
        private System.Windows.Forms.Label Title_Label;
        private System.Windows.Forms.PictureBox PictureBox_1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Credits_Label;
    }
}