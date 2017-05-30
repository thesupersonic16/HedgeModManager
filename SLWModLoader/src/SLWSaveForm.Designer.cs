namespace SLWModLoader
{
    partial class SLWSaveForm
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
            this.label = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Button_Install = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Segoe UI", 17F);
            this.label.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label.Location = new System.Drawing.Point(41, 9);
            this.label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(268, 31);
            this.label.TabIndex = 3;
            this.label.Text = "Please select an account ";
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 43);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(338, 247);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView1_DrawItem);
            // 
            // Button_Install
            // 
            this.Button_Install.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_Install.Location = new System.Drawing.Point(64, 308);
            this.Button_Install.Name = "Button_Install";
            this.Button_Install.Size = new System.Drawing.Size(227, 23);
            this.Button_Install.TabIndex = 5;
            this.Button_Install.Text = "Install";
            this.Button_Install.UseVisualStyleBackColor = true;
            this.Button_Install.Click += new System.EventHandler(this.Button_Install_Click);
            // 
            // SLWSaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(362, 343);
            this.Controls.Add(this.Button_Install);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SLWSaveForm";
            this.Text = "Install SLW Save File - SLW Mod Loader";
            this.Load += new System.EventHandler(this.SLWSaveForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button Button_Install;
    }
}