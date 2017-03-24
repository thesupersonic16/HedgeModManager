namespace SLWModLoader
{
    partial class NewModPropNewForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.PropNameLabel = new System.Windows.Forms.Label();
            this.PropValueLabel = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.PropGroupLabel = new System.Windows.Forms.Label();
            this.GroupComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(59, 38);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(301, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "New Property";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "New Property: ";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(59, 64);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(301, 20);
            this.textBox2.TabIndex = 2;
            // 
            // PropNameLabel
            // 
            this.PropNameLabel.AutoSize = true;
            this.PropNameLabel.Location = new System.Drawing.Point(12, 41);
            this.PropNameLabel.Name = "PropNameLabel";
            this.PropNameLabel.Size = new System.Drawing.Size(41, 13);
            this.PropNameLabel.TabIndex = 3;
            this.PropNameLabel.Text = "Name: ";
            // 
            // PropValueLabel
            // 
            this.PropValueLabel.AutoSize = true;
            this.PropValueLabel.Location = new System.Drawing.Point(12, 67);
            this.PropValueLabel.Name = "PropValueLabel";
            this.PropValueLabel.Size = new System.Drawing.Size(40, 13);
            this.PropValueLabel.TabIndex = 4;
            this.PropValueLabel.Text = "Value: ";
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(285, 92);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 5;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(204, 92);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // PropGroupLabel
            // 
            this.PropGroupLabel.AutoSize = true;
            this.PropGroupLabel.Location = new System.Drawing.Point(13, 93);
            this.PropGroupLabel.Name = "PropGroupLabel";
            this.PropGroupLabel.Size = new System.Drawing.Size(42, 13);
            this.PropGroupLabel.TabIndex = 7;
            this.PropGroupLabel.Text = "Group: ";
            // 
            // GroupComboBox
            // 
            this.GroupComboBox.FormattingEnabled = true;
            this.GroupComboBox.Items.AddRange(new object[] {
            "Main",
            "Desc"});
            this.GroupComboBox.Location = new System.Drawing.Point(59, 90);
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.Size = new System.Drawing.Size(121, 21);
            this.GroupComboBox.TabIndex = 8;
            this.GroupComboBox.Text = "Main";
            // 
            // NewModPropNewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(384, 127);
            this.Controls.Add(this.GroupComboBox);
            this.Controls.Add(this.PropGroupLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.PropValueLabel);
            this.Controls.Add(this.PropNameLabel);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewModPropNewForm";
            this.Text = "SLW Mod Loader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label PropNameLabel;
        private System.Windows.Forms.Label PropValueLabel;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label PropGroupLabel;
        private System.Windows.Forms.ComboBox GroupComboBox;
    }
}