using Microsoft.Win32;
using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class SLWSaveForm : Form
    {

        public string FilePath;
        public Thread imageThread;
        public List<string> SIDs = new List<string>();

        public SLWSaveForm()
        {
            InitializeComponent();
        }

        public SLWSaveForm(string filePath) : this()
        {
            FilePath = filePath;
        }

        public void GetAndApplyImages()
        {
            // WebClient for downloading data from Steam's servers
            var webClient = new WebClient();

            foreach (string sid in SIDs)
            {
                // Gets the cached icon
                var image = Steam.GetCachedProfileImage(sid);
                if (image == null || ModifierKeys.HasFlag(Keys.Shift))
                {
                    // Downloads the icon
                    image = DownloadSteamProfilePicture(webClient, sid);
                }

                if (image != null)
                    Invoke(new Action(() => listView1.LargeImageList.Images.Add(sid, image)));
            }
        }

        
        public Bitmap DownloadSteamProfilePicture(WebClient webClient, string SID)
        {
            string url = "http://steamcommunity.com/profiles/" + SID;
            string PIURL = @"steamstatic.com/steamcommunity/public/images";
            url = webClient.DownloadString(url);
            url = Program.GetString(url.Substring(0, url.IndexOf(PIURL)).LastIndexOf('\"') - 1, url);
            return new Bitmap(new MemoryStream(webClient.DownloadData(url)));
        }

        private void SLWSaveForm_Load(object sender, EventArgs e)
        {
            // Sets up this ImageList
            listView1.LargeImageList = new ImageList()
            {
                ImageSize = new Size(64, 64),
                ColorDepth = ColorDepth.Depth32Bit
            };

            // Applies the Dark Theme, Because why not?
            if (Program.UseDarkTheme)
                Theme.ApplyDarkThemeToAll(this);

            // Checks if the Key and Value exists.
            if (Steam.SteamLocation != null)
            {
                // Checks if "loginusers.vdf" exists.
                if (File.Exists(Path.Combine(Steam.SteamLocation, "config\\loginusers.vdf")))
                {
                    // loginusers.vdf
                    var file = Steam.VDFFile.ReadVDF(Path.Combine(Steam.SteamLocation, "config\\loginusers.vdf"));
                    foreach (var pair in file.Array.Elements.ToList())
                    {
                        // Adds ListViewItem
                        var array = pair.Value as Steam.VDFFile.VDFArray;
                        var lvi = new ListViewItem(array.Elements["PersonaName"].Value as string)
                        {
                            ImageKey = array.Name
                        };
                        // Adds the SID to the SID list
                        SIDs.Add(array.Name);
                        listView1.Items.Add(lvi);
                    }
                }
                // Gets the icons in another thread
                imageThread = new Thread(new ThreadStart(GetAndApplyImages));
                imageThread.Start();
            }else
            {
                Close();
            }
        }

        private void SLWSaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (imageThread.IsAlive)
                imageThread.Abort();
        }

        private void Button_Install_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string sid = listView1.SelectedItems[0].ImageKey;
                string path = Path.Combine(Program.StartDirectory, sid + ".sdat");
                if (path == FilePath)
                {
                    MessageBox.Show("Thats pointless", Program.ProgramName);
                    return;
                }
                if (File.Exists(path))
                    File.Move(path, path + ".backup");
                File.Copy(FilePath, path);
                Close();
            }
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

    }
}
