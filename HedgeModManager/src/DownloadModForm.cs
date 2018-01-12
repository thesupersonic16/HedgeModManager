using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class DownloadModForm : Form
    {

        public string Title, Author, Description, URL;

        public DownloadModForm(string title, string author, string description, string url, string credits, string image)
        {
            InitializeComponent();
            Title = title;
            Author = author;
            Description = description;
            URL = url;
            Text = "Download " + title + "?";
            Title_Label.Text = title + " Submitted by " + author;
            // Formatting
            description = description.Replace("<ul>", "").Replace("</ul>", "");
            description = description.Replace("<li>", "\n\u25A0").Replace("</li>", "\n");
            description = description.Replace("<br>", "\n");
            description = description.Replace("<i>", "").Replace("</i>", "");
            description = description.Replace("<b>", "").Replace("</b>", "");
            description = description.Replace("&nbsp;", "  ");

            Description_Label.Text = description;
            Credits_Label.Text = credits;
            if (Title_Label.Size.Width > Width)
                Width = Title_Label.Size.Width + 100;
            if (Description_Label.Size.Height + 200 > Height)
                Height = Description_Label.Size.Height + 200;
            UpdateForm();
            if (image != null)
                new Thread(() => 
                {
                    var client = new WebClient();
                    var stream = client.OpenRead(image);
                    var bitmap = new Bitmap(stream);
                    stream.Close();
                    client.Dispose();
                    Invoke(new Action(() => PictureBox_1.Image = bitmap));
                }).Start();

        }

        public void UpdateForm()
        {
            Title_Label.Location = new Point(Size.Width / 2 - Title_Label.Size.Width / 2, Title_Label.Location.Y);
            Description_Label.MaximumSize = new Size(Width - 256 - 64, Height);
            Description_Label.Location = new Point(Size.Width / 3 - Description_Label.Size.Width / 2, Description_Label.Location.Y);
            if (Description_Label.Location.X < 10)
                Description_Label.Location = new Point(10, Description_Label.Location.Y);
        }

        private void DownloadModForm_Load(object sender, EventArgs e)
        {
            Theme.ApplyDarkThemeToAll(this);
        }

        private void Button_Update_Click(object sender, EventArgs e)
        {
            Hide();
            var modUpdate = new ModUpdater.ModUpdate()
            {
                Name = Title
            };
            modUpdate.Files.Add(new ModUpdater.ModUpdateFile() { URL = URL, FileName = new Uri(URL).Segments.Last() });

            new UpdateModForm(null, modUpdate).ShowDialog();
            Close();
        }

        private void DownloadModForm_Resize(object sender, EventArgs e)
        {
            UpdateForm();
        }


    }
}
