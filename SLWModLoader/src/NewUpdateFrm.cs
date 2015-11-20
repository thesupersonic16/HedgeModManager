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
    public partial class NewUpdateFrm : Form
    {
        public NewUpdateFrm()
        {
            InitializeComponent();

            Text = $"v{Mainfrm.versionstring} Changelog";
            label1.Text = $"SLW Mod Loader v{Mainfrm.versionstring}";

            string updatechangelog = Updatefrm.latest.Substring(Updatefrm.latest.IndexOf("body") + 7, Updatefrm.latest.IndexOf("\"}", Updatefrm.latest.IndexOf("body") + 7) - (Updatefrm.latest.IndexOf("body") + 7));
            updatechangelog = updatechangelog.Replace("\\\"","\"").Replace("*", "");

            if (updatechangelog.Contains("[//]: # (MOD LOADER)\\r\\n\\r\\n")) { updatechangelog = updatechangelog.Substring(updatechangelog.IndexOf("[//]: # (MOD LOADER)\\r\\n\\r\\n")+ "[//]: # (MOD LOADER)\\r\\n\\r\\n".Length); }
            label2.Text = updatechangelog;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
