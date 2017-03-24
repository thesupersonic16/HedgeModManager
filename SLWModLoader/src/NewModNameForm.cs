using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class NewModNameForm : Form
    {
        public NewModNameForm()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public String getModName()
        {
            return modNameTxtBx.Text;
        }

        private void modNameTxtBx_TextChanged(object sender, EventArgs e)
        {
            okBtn.Enabled = modNameTxtBx.Text.Length > 0;
        }
    }
}
