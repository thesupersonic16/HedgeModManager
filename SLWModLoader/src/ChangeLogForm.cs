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
    public partial class ChangeLogForm : Form
    {
        public string downloadUrl;

        public ChangeLogForm(string version, string changeLog, string downloadUrl)
        {
            InitializeComponent();
            Text = $"A new version of { Program.ProgramName} is available.";
            titleLbl.Text = "SLW Mod Loader v" + version;
            changelogLbl.Text = changeLog;
            this.downloadUrl = downloadUrl;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChangeLogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
