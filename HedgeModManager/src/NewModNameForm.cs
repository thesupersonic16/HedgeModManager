using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class NewModNameForm : Form
    {
        public NewModNameForm()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public string GetModName()
        {
            return modNameTxtBx.Text;
        }

        private void ModNameTxtBx_TextChanged(object sender, EventArgs e)
        {
            okBtn.Enabled = modNameTxtBx.Text.Length > 0;
        }

        private void NewModNameForm_Load(object sender, EventArgs e)
        {
            Theme.ApplyDarkThemeToAll(this);
        }
    }
}
