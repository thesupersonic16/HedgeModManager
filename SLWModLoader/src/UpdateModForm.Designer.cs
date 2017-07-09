namespace SLWModLoader
{
    partial class UpdateModForm
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
            this.ProgressBarFile = new System.Windows.Forms.ProgressBar();
            this.CancelUpdateButton = new System.Windows.Forms.Button();
            this.ProgressBarAll = new System.Windows.Forms.ProgressBar();
            this.UpdateLabel = new System.Windows.Forms.Label();
            this.DownloadLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProgressBarFile
            // 
            this.ProgressBarFile.Location = new System.Drawing.Point(12, 106);
            this.ProgressBarFile.Name = "ProgressBarFile";
            this.ProgressBarFile.Size = new System.Drawing.Size(472, 32);
            this.ProgressBarFile.TabIndex = 0;
            // 
            // CancelUpdateButton
            // 
            this.CancelUpdateButton.Location = new System.Drawing.Point(190, 182);
            this.CancelUpdateButton.Name = "CancelUpdateButton";
            this.CancelUpdateButton.Size = new System.Drawing.Size(123, 23);
            this.CancelUpdateButton.TabIndex = 1;
            this.CancelUpdateButton.Text = "Cancel Update";
            this.CancelUpdateButton.UseVisualStyleBackColor = true;
            this.CancelUpdateButton.Click += new System.EventHandler(this.CancelUpdateButton_Click);
            // 
            // ProgressBarAll
            // 
            this.ProgressBarAll.Location = new System.Drawing.Point(12, 144);
            this.ProgressBarAll.Name = "ProgressBarAll";
            this.ProgressBarAll.Size = new System.Drawing.Size(472, 32);
            this.ProgressBarAll.TabIndex = 2;
            // 
            // UpdateLabel
            // 
            this.UpdateLabel.AutoSize = true;
            this.UpdateLabel.Font = new System.Drawing.Font("Segoe UI", 24F);
            this.UpdateLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.UpdateLabel.Location = new System.Drawing.Point(128, 9);
            this.UpdateLabel.Name = "UpdateLabel";
            this.UpdateLabel.Size = new System.Drawing.Size(256, 45);
            this.UpdateLabel.TabIndex = 3;
            this.UpdateLabel.Text = "Updating {MOD}";
            // 
            // DownloadLabel
            // 
            this.DownloadLabel.AutoSize = true;
            this.DownloadLabel.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.DownloadLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.DownloadLabel.Location = new System.Drawing.Point(153, 54);
            this.DownloadLabel.Name = "DownloadLabel";
            this.DownloadLabel.Size = new System.Drawing.Size(206, 30);
            this.DownloadLabel.TabIndex = 4;
            this.DownloadLabel.Text = "Starting Download...";
            // 
            // UpdateModForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(496, 217);
            this.Controls.Add(this.DownloadLabel);
            this.Controls.Add(this.UpdateLabel);
            this.Controls.Add(this.ProgressBarAll);
            this.Controls.Add(this.CancelUpdateButton);
            this.Controls.Add(this.ProgressBarFile);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateModForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UpdateModsForm";
            this.Load += new System.EventHandler(this.UpdateModsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBarFile;
        private System.Windows.Forms.Button CancelUpdateButton;
        private System.Windows.Forms.ProgressBar ProgressBarAll;
        private System.Windows.Forms.Label UpdateLabel;
        private System.Windows.Forms.Label DownloadLabel;
    }
}