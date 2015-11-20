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
    public partial class descriptionFrm : Form
    {
        public descriptionFrm(string description, string title, string author, string date, string URL, string version, string authorURL, string bgimage, string textcol, string headercol)
        {
            InitializeComponent();

            descriptionlbl.Text = (!string.IsNullOrEmpty(description))?description.Replace(@"\n",Environment.NewLine):"This mod does not contain a description.";
            titlelbl.Text = $"{((!string.IsNullOrEmpty(title)) ? title : "Un-Named Mod")} {((!string.IsNullOrEmpty(version))? "v"+version:"")}";
            authorlbl.Text = (!string.IsNullOrEmpty(author) || !string.IsNullOrEmpty(date))?$"Made{((!string.IsNullOrEmpty(author))?" by "+author:"")}{((!string.IsNullOrEmpty(date))?" on "+date:"")}":"";

            if (!string.IsNullOrEmpty(URL) && !string.IsNullOrEmpty(title)) { titlelbl.Links.Add(new LinkLabel.Link(0,title.Length,URL)); }
            if (!string.IsNullOrEmpty(authorURL) && !string.IsNullOrEmpty(author))
            {
                int i = 0;
                foreach (string str in author.Split(new string[] { " & " }, StringSplitOptions.None))
                {
                    authorlbl.Links.Add(new LinkLabel.Link(8 + author.IndexOf(str), str.Length, ((authorURL.Split(new string[] { " & " }, StringSplitOptions.None).Length > i)?authorURL.Split(new string[] { " & " }, StringSplitOptions.None)[i]:"")));
                    i++;
                }
            }

            Text = $"About \"{((!string.IsNullOrEmpty(title))? title:"Un-Named Mod")}\"";
            if (!string.IsNullOrEmpty(bgimage) && File.Exists(bgimage))
            {
                using (Stream imagestream = new FileStream(bgimage, FileMode.Open)) { BackgroundImage = Image.FromStream(imagestream); }
            }
            if (!string.IsNullOrEmpty(headercol)) { titlelbl.LinkColor = titlelbl.ForeColor = authorlbl.LinkColor = authorlbl.ForeColor = Color.FromArgb(Convert.ToInt32(headercol.Split(',')[0]), Convert.ToInt32(headercol.Split(',')[1]), Convert.ToInt32(headercol.Split(',')[2])); }
            if (!string.IsNullOrEmpty(textcol)) { descriptionlbl.ForeColor = Color.FromArgb(Convert.ToInt32(textcol.Split(',')[0]), Convert.ToInt32(textcol.Split(',')[1]), Convert.ToInt32(textcol.Split(',')[2])); }
        }

        private void okbtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void authorlbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)e.Link.LinkData)) { Process.Start((string)e.Link.LinkData); }
        }

        private void titlelbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)titlelbl.Links[0].LinkData)) { Process.Start((string)titlelbl.Links[0].LinkData); }
        }
    }
}
