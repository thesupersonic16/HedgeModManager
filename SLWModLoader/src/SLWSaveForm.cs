using Microsoft.Win32;
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

namespace SLWModLoader
{
    public partial class SLWSaveForm : Form
    {

        public string FilePath;
        public string SteamPath;
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
                var image = GetCachedSteamProfilePicture(sid);
                if (image == null || ModifierKeys.HasFlag(Keys.Shift))
                {
                    // Downloads the icon
                    image = DownloadSteamProfilePicture(webClient, sid);
                }

                if (image != null)
                    Invoke(new Action(() => listView1.LargeImageList.Images.Add(sid, image)));
            }
        }

        public Image GetCachedSteamProfilePicture(string SID)
        {
            string filePath = Path.Combine(SteamPath, "config/avatarcache/", SID + ".png");
            if (File.Exists(filePath))
                return Image.FromFile(filePath);
            return null;
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
            MainForm.ApplyDarkTheme(this);

            // Gets Steam's Registry Key
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
            // If null then try get it from the 64-bit Registry
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE\\Valve\\Steam");
            // Checks if the Key and Value exists.
            if (key != null && key.GetValue("SteamPath") is string steamPath)
            {
                SteamPath = steamPath;
                // Checks if "loginusers.vdf" exists.
                if (File.Exists(Path.Combine(steamPath, "config\\loginusers.vdf")))
                {
                    // loginusers.vdf
                    var file = VDFFile.ReadVDF(Path.Combine(steamPath, "config\\loginusers.vdf"));
                    foreach (var pair in file.Array.Elements.ToList())
                    {
                        // Adds ListViewItem
                        var array = pair.Value as VDFFile.VDFArray;
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

        #region Horrible code, Don't look
        public class VDFFile
        {

            public class VDFElement
            {
                public string Name;
                public object Value;

                
            }

            public class VDFArray : VDFElement
            {
                public Dictionary<string, VDFElement> Elements = new Dictionary<string, VDFElement>();

                public VDFArray(string name)
                {
                    Name = name;
                }
            }

            public VDFArray Array = null;

            protected VDFFile()
            {

            }

            // I know this is not how you read .vdf files. But it works for files that I need to read.
            public static VDFFile ReadVDF(string filePath)
            {
                var file = new VDFFile();
                string buffer = "";
                VDFArray mainArray = null;
                VDFArray lastArray = null;
                VDFArray currentArray = null;

                VDFElement element = null;
                using (var textReader = File.OpenText(filePath))
                {
                    while (textReader.Peek() != -1)
                    {
                        string line = textReader.ReadLine();
                        bool startReadingString = false;
                        for (int i = 0; i < line.Length; ++i)
                        {
                            // Read String
                            if (startReadingString)
                            {
                                if (line[i] == '\"')
                                {
                                    startReadingString = false;
                                    if (element != null)
                                    {
                                        element.Value = buffer;
                                        buffer = "";
                                        currentArray.Elements.Add(element.Name, element);
                                        element = null;
                                    }
                                    continue;
                                }
                                buffer += line[i];
                                continue;
                            }

                            switch (line[i])
                            {
                                case '\"':
                                    if (buffer.Length != 0)
                                    {
                                        if (element == null)
                                        {
                                            element = new VDFElement();
                                            element.Name = buffer;
                                            buffer = "";
                                        }
                                    }
                                    startReadingString = true;
                                    break;
                                case '{':
                                    if (mainArray == null)
                                    {
                                        mainArray = new VDFArray(buffer);
                                        currentArray = mainArray;
                                        buffer = "";
                                        break;
                                    }
                                    var array = new VDFArray(buffer);
                                    lastArray = currentArray;
                                    currentArray.Elements.Add(array.Name, array);
                                    currentArray = array;
                                    buffer = "";
                                    break;
                                case '}':
                                    currentArray = lastArray;
                                    lastArray = mainArray;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                file.Array = mainArray;
                return file;
            }


        }
        #endregion
    }
}
