using HedgeModManager.Properties;

namespace SS16
{
    partial class SS16MessageBox
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
            this.Label_Title = new System.Windows.Forms.Label();
            this.Label_Message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label_Title
            // 
            this.Label_Title.AutoSize = true;
            this.Label_Title.Font = new System.Drawing.Font("Segoe UI", 21F);
            this.Label_Title.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.Label_Title.Location = new System.Drawing.Point(11, 9);
            this.Label_Title.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label_Title.Name = "Label_Title";
            this.Label_Title.Size = new System.Drawing.Size(78, 38);
            this.Label_Title.TabIndex = 6;
            this.Label_Title.Text = "TEXT";
            // 
            // Label_Message
            // 
            this.Label_Message.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Label_Message.Location = new System.Drawing.Point(15, 64);
            this.Label_Message.Name = "Label_Message";
            this.Label_Message.Size = new System.Drawing.Size(596, 143);
            this.Label_Message.TabIndex = 7;
            this.Label_Message.Text = "TEXT";
            this.Label_Message.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SS16MessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(623, 276);
            this.Controls.Add(this.Label_Message);
            this.Controls.Add(this.Label_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = global::HedgeModManager.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SS16MessageBox";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TEXT";
            this.Load += new System.EventHandler(this.SS16MessageBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label Label_Title;
        public System.Windows.Forms.Label Label_Message;
    }
}