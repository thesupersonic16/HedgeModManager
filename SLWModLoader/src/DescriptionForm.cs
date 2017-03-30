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

            if (Desc.ContainsParameter("TextFont"))
            {
                try
                {
                    descriptionLbl.Font = new Font(Desc["TextFont"], 8.25f);
                }
                catch { }
            }

            if (Desc.ContainsParameter("TextSize"))
            {
                try
                {
                    descriptionLbl.Font = new Font(descriptionLbl.Font.FontFamily, float.Parse(Desc["TextSize"]));
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

            if (Desc.ContainsParameter("HeaderFont"))
            {
                try
                {
                    titleLbl.Font = new Font(Desc["HeaderFont"], titleLbl.Font.Size);
                }
                catch { }
            }

            if(Desc.ContainsParameter("HeaderSize"))
            {
                try
                {
                    titleLbl.Font = new Font(titleLbl.Font.FontFamily, float.Parse(Desc["HeaderSize"]));
                    authorLbl.Location = new Point(authorLbl.Location.X, titleLbl.Location.Y + ((int)float.Parse(Desc["HeaderSize"])) + 23);
                    descriptionLbl.Location = new Point(descriptionLbl.Location.X, descriptionLbl.Location.Y + 23);
                }
                catch { }
            }

            if (Desc.ContainsParameter("AuthorColor"))
            {
                try
                {
                    authorLbl.LinkColor = authorLbl.ForeColor =
                        Color.FromArgb(Convert.ToInt32(Desc["HeaderColor"].Split(',')[0]),
                                       Convert.ToInt32(Desc["HeaderColor"].Split(',')[1]),
                                       Convert.ToInt32(Desc["HeaderColor"].Split(',')[2]));
                }
                catch { }

            }

            if (Desc.ContainsParameter("AuthorFont"))
            {
                try
                {
                    authorLbl.Font = new Font(Desc["AuthorFont"], authorLbl.Font.Size);
                }
                catch { }
            }

            if (Desc.ContainsParameter("AuthorSize"))
            {
                try
                {
                    authorLbl.Font = new Font(authorLbl.Font.FontFamily, float.Parse(Desc["AuthorSize"]));
                    descriptionLbl.Location = new Point(descriptionLbl.Location.X, descriptionLbl.Location.Y +
                        (int)float.Parse(Desc["AuthorSize"]));
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
