using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    // 
    // NEEDS MORE WORK!
    // 
    public partial class SLWSaveForm : Form
    {

        public string FilePath;

        public SLWSaveForm()
        {
            InitializeComponent();
        }

        public SLWSaveForm(string filePath) : this()
        {
            FilePath = filePath;
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
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Valve\\Steam");
            // Checks if the Key and Value exists.
            if (key != null && key.GetValue("SteamPath") is string steamPath)
            {
                // Checks if "loginusers.vdf" exists.
                if (File.Exists(Path.Combine(steamPath, "config\\loginusers.vdf")))
                {
                    // WebClient for downloading data from Steam's servers
                    var webClient = new WebClient();
                    // loginusers.vdf
                    var file = VDFFile.ReadVDF(Path.Combine(steamPath, "config\\loginusers.vdf"));
                    // 
                    foreach (var pair in file.Array.Elements.ToList())
                    {
                        // Adds ListViewItem
                        var array = pair.Value as VDFFile.VDFArray;
                        var lvi = new ListViewItem(array.Elements["PersonaName"].Value as string)
                        {
                            ImageKey = array.Name
                        };
                        listView1.Items.Add(lvi);

                        // Downloads the icons
                        #region Messy
                        string url = "http://steamcommunity.com/profiles/" + array.Name;
                        string PIURL = "http://cdn.edgecast.steamstatic.com/steamcommunity/public/images";
                        url = webClient.DownloadString(url);
                        url = Program.GetString(url.IndexOf(PIURL) - 1, url);
                        var profileImage = new Bitmap(new MemoryStream(webClient.DownloadData(url)));
                        listView1.LargeImageList.Images.Add(array.Name, profileImage);
                        #endregion
                    }
                }
            }
        }

        private void Button_Install_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string sid = listView1.SelectedItems[0].ImageKey;
                string path = Path.Combine(Program.StartDirectory, sid + ".sdat");
                if (File.Exists(path))
                    File.Move(path, path + ".backup");
                File.Copy(FilePath, path);
                Close();
            }
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

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
