namespace SLWModLoader
{
    partial class EditPropertyForm
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
            this.nameLbl = new System.Windows.Forms.Label();
            this.nameTxtBx = new System.Windows.Forms.TextBox();
            this.valueLbl = new System.Windows.Forms.Label();
            this.valueComboBx = new System.Windows.Forms.ComboBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.typeComboBx = new System.Windows.Forms.ComboBox();
            this.typeLbl = new System.Windows.Forms.Label();
            this.valueNud = new System.Windows.Forms.NumericUpDown();
            this.groupComboBx = new System.Windows.Forms.ComboBox();
            this.groupLbl = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.valueNud)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLbl
            // 
            this.nameLbl.AutoSize = true;
            this.nameLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.nameLbl.Location = new System.Drawing.Point(12, 9);
            this.nameLbl.Name = "nameLbl";
            this.nameLbl.Size = new System.Drawing.Size(55, 21);
            this.nameLbl.TabIndex = 4;
            this.nameLbl.Text = "Name:";
            // 
            // nameTxtBx
            // 
            this.nameTxtBx.Location = new System.Drawing.Point(73, 7);
            this.nameTxtBx.Name = "nameTxtBx";
            this.nameTxtBx.Size = new System.Drawing.Size(315, 26);
            this.nameTxtBx.TabIndex = 5;
            // 
            // valueLbl
            // 
            this.valueLbl.AutoSize = true;
            this.valueLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.valueLbl.Location = new System.Drawing.Point(12, 160);
            this.valueLbl.Name = "valueLbl";
            this.valueLbl.Size = new System.Drawing.Size(51, 21);
            this.valueLbl.TabIndex = 0;
            this.valueLbl.Text = "Value:";
            // 
            // valueComboBx
            // 
            this.valueComboBx.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.valueComboBx.FormattingEnabled = true;
            this.valueComboBx.Location = new System.Drawing.Point(73, 158);
            this.valueComboBx.Name = "valueComboBx";
            this.valueComboBx.Size = new System.Drawing.Size(315, 28);
            this.valueComboBx.TabIndex = 1;
            // 
            // okBtn
            // 
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okBtn.Location = new System.Drawing.Point(294, 205);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(94, 36);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.btn_Click);
            // 
            // typeComboBx
            // 
            this.typeComboBx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBx.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.typeComboBx.FormattingEnabled = true;
            this.typeComboBx.Items.AddRange(new object[] {
            "String",
            "Integer",
            "Boolean"});
            this.typeComboBx.Location = new System.Drawing.Point(73, 107);
            this.typeComboBx.Name = "typeComboBx";
            this.typeComboBx.Size = new System.Drawing.Size(315, 28);
            this.typeComboBx.TabIndex = 9;
            // 
            // typeLbl
            // 
            this.typeLbl.AutoSize = true;
            this.typeLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.typeLbl.Location = new System.Drawing.Point(12, 109);
            this.typeLbl.Name = "typeLbl";
            this.typeLbl.Size = new System.Drawing.Size(45, 21);
            this.typeLbl.TabIndex = 8;
            this.typeLbl.Text = "Type:";
            // 
            // valueNud
            // 
            this.valueNud.Location = new System.Drawing.Point(73, 158);
            this.valueNud.Name = "valueNud";
            this.valueNud.Size = new System.Drawing.Size(315, 26);
            this.valueNud.TabIndex = 7;
            this.valueNud.Visible = false;
            // 
            // groupComboBx
            // 
            this.groupComboBx.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupComboBx.FormattingEnabled = true;
            this.groupComboBx.Location = new System.Drawing.Point(73, 56);
            this.groupComboBx.Name = "groupComboBx";
            this.groupComboBx.Size = new System.Drawing.Size(315, 28);
            this.groupComboBx.TabIndex = 7;
            this.groupComboBx.Text = "Desc";
            // 
            // groupLbl
            // 
            this.groupLbl.AutoSize = true;
            this.groupLbl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.groupLbl.Location = new System.Drawing.Point(12, 58);
            this.groupLbl.Name = "groupLbl";
            this.groupLbl.Size = new System.Drawing.Size(57, 21);
            this.groupLbl.TabIndex = 6;
            this.groupLbl.Text = "Group:";
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelBtn.Location = new System.Drawing.Point(194, 205);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(94, 36);
            this.cancelBtn.TabIndex = 3;
            this.cancelBtn.Text = "&Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.btn_Click);
            // 
            // editPropertyFrm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(400, 253);
            this.Controls.Add(this.valueComboBx);
            this.Controls.Add(this.valueNud);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.groupComboBx);
            this.Controls.Add(this.groupLbl);
            this.Controls.Add(this.typeComboBx);
            this.Controls.Add(this.typeLbl);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.valueLbl);
            this.Controls.Add(this.nameTxtBx);
            this.Controls.Add(this.nameLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "editPropertyFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SLW Mod Loader";
            ((System.ComponentModel.ISupportInitialize)(this.valueNud)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLbl;
        private System.Windows.Forms.Label valueLbl;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label typeLbl;
        public System.Windows.Forms.TextBox nameTxtBx;
        public System.Windows.Forms.ComboBox valueComboBx;
        public System.Windows.Forms.ComboBox typeComboBx;
        public System.Windows.Forms.NumericUpDown valueNud;
        public System.Windows.Forms.ComboBox groupComboBx;
        private System.Windows.Forms.Label groupLbl;
        private System.Windows.Forms.Button cancelBtn;
    }
}