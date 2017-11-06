using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class DownloadModForm : Form
    {

        public string Title, Author, Description, URL;
        
        public DownloadModForm(string title, string author, string description, string url, string credits, Bitmap image)
        {
            InitializeComponent();
            Title = title;
            Author = author;
            Description = description;
            URL = url;
            PictureBox_1.Image = image;
            Text = "Download " + title + "?";
            Title_Label.Text = title + " Submitted by " + author;
            Title_Label.Location = new Point(Size.Width / 2 - Title_Label.Size.Width / 2, Title_Label.Location.Y);
            Description_Label.Text = description;
            Description_Label.Location = new Point(Size.Width / 2 - Description_Label.Size.Width / 2, Description_Label.Location.Y);
            Credits_Label.Text = credits; 
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
    }
}
