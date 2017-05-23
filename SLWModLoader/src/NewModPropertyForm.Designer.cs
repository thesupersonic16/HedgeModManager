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
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            this.ValueContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_SelectDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.FileToolStripMenuItem_SelectFile = new System.Windows.Forms.ToolStripMenuItem();
            this.PropNameLabel = new System.Windows.Forms.Label();
            this.PropValueLabel = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.PropGroupLabel = new System.Windows.Forms.Label();
            this.GroupComboBox = new System.Windows.Forms.ComboBox();
            this.TypeComboBox = new System.Windows.Forms.ComboBox();
            this.ValueNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ValueCheckBox = new System.Windows.Forms.CheckBox();
            this.ToolStripMenuItem_Multiline = new System.Windows.Forms.ToolStripMenuItem();
            this.ValueContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(59, 38);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(332, 20);
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
            // ValueTextBox
            // 
            this.ValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueTextBox.ContextMenuStrip = this.ValueContextMenu;
            this.ValueTextBox.Location = new System.Drawing.Point(59, 64);
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.Size = new System.Drawing.Size(431, 20);
            this.ValueTextBox.TabIndex = 1;
            this.ValueTextBox.TextChanged += new System.EventHandler(this.ValueTextBox_TextChanged);
            this.ValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ValueTextBox_KeyPress);
            // 
            // ValueContextMenu
            // 
            this.ValueContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_SelectDirectory,
            this.FileToolStripMenuItem_SelectFile,
            this.ToolStripMenuItem_Multiline});
            this.ValueContextMenu.Name = "ValueContextMenu";
            this.ValueContextMenu.Size = new System.Drawing.Size(157, 70);
            // 
            // ToolStripMenuItem_SelectDirectory
            // 
            this.ToolStripMenuItem_SelectDirectory.Name = "ToolStripMenuItem_SelectDirectory";
            this.ToolStripMenuItem_SelectDirectory.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItem_SelectDirectory.Text = "Select Directory";
            this.ToolStripMenuItem_SelectDirectory.Click += new System.EventHandler(this.ToolStripMenuItem_SelectDirectory_Click);
            // 
            // FileToolStripMenuItem_SelectFile
            // 
            this.FileToolStripMenuItem_SelectFile.Name = "FileToolStripMenuItem_SelectFile";
            this.FileToolStripMenuItem_SelectFile.Size = new System.Drawing.Size(156, 22);
            this.FileToolStripMenuItem_SelectFile.Text = "Select File";
            this.FileToolStripMenuItem_SelectFile.Click += new System.EventHandler(this.ToolStripMenuItem_SelectFile_Click);
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
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Location = new System.Drawing.Point(415, 88);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 5;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(334, 88);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // PropGroupLabel
            // 
            this.PropGroupLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PropGroupLabel.AutoSize = true;
            this.PropGroupLabel.Location = new System.Drawing.Point(13, 93);
            this.PropGroupLabel.Name = "PropGroupLabel";
            this.PropGroupLabel.Size = new System.Drawing.Size(42, 13);
            this.PropGroupLabel.TabIndex = 7;
            this.PropGroupLabel.Text = "Group: ";
            // 
            // GroupComboBox
            // 
            this.GroupComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GroupComboBox.FormattingEnabled = true;
            this.GroupComboBox.Items.AddRange(new object[] {
            "Main",
            "Desc",
            "CPKs"});
            this.GroupComboBox.Location = new System.Drawing.Point(59, 90);
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.Size = new System.Drawing.Size(139, 21);
            this.GroupComboBox.TabIndex = 3;
            this.GroupComboBox.Text = "Main";
            this.GroupComboBox.SelectedIndexChanged += new System.EventHandler(this.GroupComboBox_SelectedIndexChanged);
            // 
            // TypeComboBox
            // 
            this.TypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeComboBox.FormattingEnabled = true;
            this.TypeComboBox.Items.AddRange(new object[] {
            "String",
            "Integer",
            "Boolean"});
            this.TypeComboBox.Location = new System.Drawing.Point(397, 38);
            this.TypeComboBox.Name = "TypeComboBox";
            this.TypeComboBox.Size = new System.Drawing.Size(93, 21);
            this.TypeComboBox.TabIndex = 2;
            this.TypeComboBox.Text = "String";
            this.TypeComboBox.SelectedIndexChanged += new System.EventHandler(this.TypeComboBox_SelectedIndexChanged);
            // 
            // ValueNumericUpDown
            // 
            this.ValueNumericUpDown.Location = new System.Drawing.Point(59, 64);
            this.ValueNumericUpDown.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.ValueNumericUpDown.Minimum = new decimal(new int[] {
            8192,
            0,
            0,
            -2147483648});
            this.ValueNumericUpDown.Name = "ValueNumericUpDown";
            this.ValueNumericUpDown.Size = new System.Drawing.Size(431, 20);
            this.ValueNumericUpDown.TabIndex = 10;
            this.ValueNumericUpDown.Visible = false;
            // 
            // ValueCheckBox
            // 
            this.ValueCheckBox.AutoSize = true;
            this.ValueCheckBox.Location = new System.Drawing.Point(103, 66);
            this.ValueCheckBox.Name = "ValueCheckBox";
            this.ValueCheckBox.Size = new System.Drawing.Size(15, 14);
            this.ValueCheckBox.TabIndex = 11;
            this.ValueCheckBox.UseVisualStyleBackColor = true;
            this.ValueCheckBox.Visible = false;
            // 
            // ToolStripMenuItem_Multiline
            // 
            this.ToolStripMenuItem_Multiline.Name = "ToolStripMenuItem_Multiline";
            this.ToolStripMenuItem_Multiline.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItem_Multiline.Text = "Multiline";
            this.ToolStripMenuItem_Multiline.Click += new System.EventHandler(this.ToolStripMenuItem_Multiline_Click);
            // 
            // NewModPropNewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(502, 127);
            this.Controls.Add(this.TypeComboBox);
            this.Controls.Add(this.GroupComboBox);
            this.Controls.Add(this.PropGroupLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.PropValueLabel);
            this.Controls.Add(this.PropNameLabel);
            this.Controls.Add(this.ValueTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ValueNumericUpDown);
            this.Controls.Add(this.ValueCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewModPropNewForm";
            this.Text = "SLW Mod Loader";
            this.Load += new System.EventHandler(this.NewModPropNewForm_Load);
            this.ValueContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ValueNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ValueTextBox;
        private System.Windows.Forms.Label PropNameLabel;
        private System.Windows.Forms.Label PropValueLabel;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label PropGroupLabel;
        private System.Windows.Forms.ComboBox GroupComboBox;
        private System.Windows.Forms.ComboBox TypeComboBox;
        private System.Windows.Forms.NumericUpDown ValueNumericUpDown;
        private System.Windows.Forms.CheckBox ValueCheckBox;
        private System.Windows.Forms.ContextMenuStrip ValueContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_SelectDirectory;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem_SelectFile;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Multiline;
    }
}