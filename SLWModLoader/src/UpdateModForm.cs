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
        public Thread downloadThread;
        public string[] files;
        public string mod_update_files;
        public string modRoot;
        public bool cancel = false;
        
        public UpdateModForm(string modName, string mod_update_files, string modRoot)
        {
            InitializeComponent();
            this.mod_update_files = mod_update_files;
            this.modRoot = modRoot;
            UpdateLabel.Text = "Updating " + modName;
            UpdateLabel.Location = new Point(Size.Width/2-UpdateLabel.Size.Width/2, UpdateLabel.Location.Y);
            DownloadLabel.Text = "Starting Download...";
            DownloadLabel.Location = new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y);
        }

        private void UpdateModsForm_Load(object sender, EventArgs e)
        {
            var webClient = new WebClient();

            downloadThread = new Thread(() =>
            {
                // Splits all the lines in mod_update_files.txt into an array.
                string[] split = mod_update_files.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                // Sets the ProgressBarAll's maximum to the amount of files/lines.
                Invoke(new Action(() => ProgressBarAll.Maximum = split.Length));
                // Iterates all the lines.
                for (int i = 0; i < split.Length; ++i)
                {
                    // Gets and stores the current line.
                    string s = split[i];
                    // Checks if the line starts with ';' or has nothing in it.
                    if (s.Length == 0 || s.StartsWith(";")) continue;
                    LogFile.AddMessage("Downloading: " + s.Split(':')[0] + " at " + s.Substring(s.IndexOf(":") + 1));
                    // Closes and returns if the user clicked cancel.
                    if (cancel) { Invoke(new Action(() => Close())); return; }
                    // Sets DownloadLabel's Text to show what file is being downloaded.
                    Invoke(new Action(() => DownloadLabel.Text = "Downloading... " + s.Split(':')[0]));
                    // Centres DownloadLabel's position.
                    Invoke(new Action(() => DownloadLabel.Location =
                        new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y)));
                    // Adds the WebClient_DownloadProgressChanged event to the web client.
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);
                    // Sets ProgressBarAll's Value to the current file.
                    Invoke(new Action(() => ProgressBarAll.Value = i));
                    // Downloads the current file to the mod root.
                    Invoke(new Action(() => webClient.DownloadFileAsync(new Uri(s.Substring(s.IndexOf(":")+1)),
                        Path.Combine(modRoot, s.Split(':')[0]))));
                    // Waits for the download to finish.
                    while (webClient.IsBusy)
                    {
                        Thread.Sleep(50);
                    }
                    Thread.Sleep(500);
                }
                // Closes the update dialog after all the file has been downloaded.
                Invoke(new Action(() => Close()));
            });
            // Starts the download thread.
            downloadThread.Start();
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
