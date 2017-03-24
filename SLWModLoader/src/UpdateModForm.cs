using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class UpdateModForm : Form
    {
        public Thread thread;
        public string[] files;
        public string mod_files;
        public string modRoot;
        public bool cancel = false;
        
        public UpdateModForm(string modName, string mod_files, string modRoot)
        {
            InitializeComponent();
            this.mod_files = mod_files;
            this.modRoot = modRoot;
            label1.Text = "Updating " + modName;
            label1.Location = new Point(Size.Width/2-label1.Size.Width/2, label1.Location.Y);
            DownloadLabel.Text = "Starting Download...";
            DownloadLabel.Location = new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y);
        }

        // TODO: Find out how mod_files.txt is written
        private void UpdateModsForm_Load(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();

            thread = new Thread(() =>
            {
                string[] split = mod_files.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < split.Length; ++i)
                {
                    string s = split[i];
                    if (cancel) { Invoke(new Action(() => Close())); return; }
                    Invoke(new Action(() => DownloadLabel.Text = "Downloading... " + s.Split(':')[0]));
                    Invoke(new Action(() => DownloadLabel.Location = new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y)));
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);
                    Invoke(new Action(() => ProgressBarAll.Maximum = split.Length));
                    Invoke(new Action(() => ProgressBarAll.Value = i));
                    Invoke(new Action(() => wc.DownloadFileAsync(new Uri(s.Substring(s.IndexOf(":")+1)), Path.Combine(modRoot, s.Split(':')[0]))));
                    while(wc.IsBusy)
                    {
                        Thread.Sleep(50);
                    }
                    Thread.Sleep(500);
                }
                Invoke(new Action(() => Close()));
            });
            thread.Start();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            cancel = true;
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBarFile.Maximum = (int)e.TotalBytesToReceive;
            ProgressBarFile.Value = (int)e.BytesReceived;
        }

    }
}
