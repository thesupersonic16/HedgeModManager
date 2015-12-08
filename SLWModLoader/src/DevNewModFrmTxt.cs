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
    public partial class DevNewModFrmTxt : Form
    {
        public DevNewModFrmTxt()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            okBtn.Enabled = !string.IsNullOrEmpty(textBox1.Text);
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Close();
            new DevNewModFrm(textBox1.Text).ShowDialog();
        }
    }
}
