using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class UpdateModForm : Form
    {
        public Thread downloadThread;
        public Dictionary<string, string> files;
        public string modRoot;
        public bool cancel = false;
        
        public UpdateModForm(string modName, Dictionary<string, string> files, string modRoot)
        {
            InitializeComponent();
            this.files = files;
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
                // Sets the ProgressBarAll's maximum to the amount of files/lines.
                Invoke(new Action(() => ProgressBarAll.Maximum = files.Count));
                // Iterates all the lines.

                var fileList = files.ToList();
                for (int i = 0; i < files.Count; ++i)
                {
                    // Gets and stores the current line.
                    var fileName = fileList[i].Key;
                    var url = fileList[i].Value;

                    LogFile.AddMessage($"Downloading: {fileName} at {url}");
                    // Closes and returns if the user clicked cancel.
                    if (cancel) { Invoke(new Action(() => Close())); return; }
                    // Sets DownloadLabel's Text to show what file is being downloaded.
                    Invoke(new Action(() => DownloadLabel.Text = "Downloading... " + fileName));
                    // Centres DownloadLabel's position.
                    Invoke(new Action(() => DownloadLabel.Location =
                        new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y)));
                    // Adds the WebClient_DownloadProgressChanged event to the web client.
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);
                    // Sets ProgressBarAll's Value to the current file.
                    Invoke(new Action(() => ProgressBarAll.Value = i));
                    // Downloads the current file to the mod root.
                    Invoke(new Action(() => webClient.DownloadFileAsync(new Uri(url),
                        Path.Combine(modRoot, fileName))));
                    // Waits for the download to finish.
                    while (webClient.IsBusy)
                    {
                        Thread.Sleep(25);
                    }
                    Thread.Sleep(250);
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
