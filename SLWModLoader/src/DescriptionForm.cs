using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class DescriptionForm : Form
    {

        private string authorUrl;

        public DescriptionForm(string description, string title, string author, string date, string url, string version, string authorUrl)
        {
            InitializeComponent();
            authorLbl.LinkArea = new LinkArea(0, 0);
            descriptionLbl.Text = Regex.Replace(description, "\\n", "\n"); ;
            titleLbl.Text = title + " v"+ version;
            authorLbl.Text = $"Made by {author} on {date}";
            if(authorUrl.Length > 0)
                authorLbl.LinkArea = new LinkArea(8, author.Length);
            this.authorUrl = authorUrl;
        }

        public DescriptionForm(Mod mod)
        : this(mod.Description, mod.Title, mod.Author, mod.Date, mod.Url, mod.Version, mod.AuthorUrl)
        {
            #region Nothing to see here
            IniGroup Desc = mod.GetIniFile()["Desc"];
            if (Desc.ContainsParameter("BackgroundImage"))
            {
                try
                {
                    BackgroundImage = Bitmap.FromFile(Path.Combine(mod.RootDirectory, Desc["BackgroundImage"]));
                    BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch { }
            }

            if (Desc.ContainsParameter("BackgroundImageCount"))
            {
                try
                {
                    var rand = new Random();
                    BackgroundImage = Bitmap.FromFile(Path.Combine(mod.RootDirectory, Desc["BackgroundImage" + rand.Next((int)Desc["BackgroundImageCount", typeof(int)])]));
                    BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch { }
            }

            if (Desc.ContainsParameter("TextColor"))
            {
                try
                {
                    descriptionLbl.ForeColor = Color.FromArgb(
                        Convert.ToInt32(Desc["TextColor"].Split(',')[0]),
                        Convert.ToInt32(Desc["TextColor"].Split(',')[1]),
                        Convert.ToInt32(Desc["TextColor"].Split(',')[2]));
                }
                catch { }
            }

            if (Desc.ContainsParameter("HeaderColor"))
            {
                try
                {
                    titleLbl.LinkColor = titleLbl.ForeColor =
                        authorLbl.LinkColor = authorLbl.ForeColor =
                        Color.FromArgb(Convert.ToInt32(Desc["HeaderColor"].Split(',')[0]),
                                       Convert.ToInt32(Desc["HeaderColor"].Split(',')[1]),
                                       Convert.ToInt32(Desc["HeaderColor"].Split(',')[2]));
                }
                catch { }

            }
            #endregion
        }

        private void okbtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void authorLbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(authorUrl);
        }
    }
}
