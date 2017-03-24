using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;

namespace SLWModLoader
{
    public partial class AddModForm : Form
    {
        public AddModForm()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Close();
            if(makingItRBtn.Checked)
            {
                NewModNameForm nmnf = new NewModNameForm();
                if (nmnf.ShowDialog() == DialogResult.OK)
                {
                    NewModForm nmf = new NewModForm(nmnf.getModName());
                    nmf.ShowDialog();
                }
            }
        }
    }
}
