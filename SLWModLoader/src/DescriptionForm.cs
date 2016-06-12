using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class DescriptionForm : Form
    {
        public DescriptionForm(string description, string title, string author, string date, string URL, string version, string authorURL, string textColor, string headerColor)
        {
            InitializeComponent();
        }

        private void okbtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
