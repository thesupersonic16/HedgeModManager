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
            this.CancelButton = new System.Windows.Forms.Button();
            this.ProgressBarAll = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
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
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(190, 182);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(123, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel Update";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ProgressBarAll
            // 
            this.ProgressBarAll.Location = new System.Drawing.Point(12, 144);
            this.ProgressBarAll.Name = "ProgressBarAll";
            this.ProgressBarAll.Size = new System.Drawing.Size(472, 32);
            this.ProgressBarAll.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 24F);
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(161, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 45);
            this.label1.TabIndex = 3;
            this.label1.Text = "Updating";
            // 
            // DownloadLabel
            // 
            this.DownloadLabel.AutoSize = true;
            this.DownloadLabel.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.DownloadLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.DownloadLabel.Location = new System.Drawing.Point(132, 54);
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
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProgressBarAll);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ProgressBarFile);
            this.Name = "UpdateModForm";
            this.ShowIcon = false;
            this.Text = "UpdateModsForm";
            this.Load += new System.EventHandler(this.UpdateModsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBarFile;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.ProgressBar ProgressBarAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DownloadLabel;
    }
}