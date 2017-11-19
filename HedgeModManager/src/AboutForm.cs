using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GitLbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/thesupersonic16/HedgeModManager");
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            Theme.ApplyDarkThemeToAll(this);
            // Links
            AddLink("Radfordhound", "https://github.com/Radfordhound");
            AddLink("SuperSonic16", "https://github.com/thesupersonic16");
            AddLink("Skyth", "https://github.com/blueskythlikesclouds");
            AddLink("Korama", "https://forums.sonicretro.org/index.php?showuser=677");
            AddLink("CPKREDIR", "https://forums.sonicretro.org/index.php?showtopic=28795");
            AddLink("Slash", "https://github.com/slashiee");
            AddLink("MainMemory", "https://github.com/MainMemory");
            AddLink("mod-loader-common", "https://github.com/sonicretro/mania-mod-loader");
        }

        public void AddLink(string text, string link)
        {
            int index = linkLabel1.Text.IndexOf(text);
            linkLabel1.Links.Add(new LinkLabel.Link(index, text.Length, link));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
