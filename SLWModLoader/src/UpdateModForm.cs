using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class UpdateModForm : Form
    {
        // Variables/Constants
        public Thread DownloadThread;
        public Dictionary<string, Tuple<string, string>> Files;
        public string ModRoot;
        public bool CancelUpdate = false;

        // Constructors
        public UpdateModForm(string modName, Dictionary<string, Tuple<string, string>> files, string modRoot)
        {
            InitializeComponent();
            Files = files;
            ModRoot = modRoot;
            UpdateLabel.Text = "Updating " + modName;
            UpdateLabel.Location = new Point(Size.Width/2-UpdateLabel.Size.Width/2, UpdateLabel.Location.Y);
            DownloadLabel.Text = "Starting Download...";
            DownloadLabel.Location = new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y);
        }

        // GUI Events
        private void UpdateModsForm_Load(object sender, EventArgs e)
        {
            var webClient = new WebClient();

            DownloadThread = new Thread(() =>
            {
                // Sets the ProgressBarAll's maximum to the amount of files/lines.
                Invoke(new Action(() => ProgressBarAll.Maximum = Files.Count));

                var sha = new SHA256Managed();
                byte[] hash;
                var fileList = Files.ToList();
                for (int i = 0; i < Files.Count; ++i)
                {
                    // Gets and stores the current line.
                    string fileName = fileList[i].Key;
                    string fileHash = fileList[i].Value.Item1;
                    string fileUrl = fileList[i].Value.Item2;

                    LogFile.AddMessage($"Downloading: {fileName} at {fileUrl}");
                    // Closes and returns if the user clicked cancel.
                    if (CancelUpdate) { Invoke(new Action(() => Close())); return; }
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
                    Invoke(new Action(() => webClient.DownloadFileAsync(new Uri(fileUrl),
                        Path.Combine(ModRoot, fileName))));
                    // Waits for the download to finish.
                    while (webClient.IsBusy)
                    {
                        Thread.Sleep(25);
                    }
                    using (var stream = File.OpenRead(Path.Combine(ModRoot, fileName)))
                    {
                        if (fileHash != 0.ToString("X64"))
                        {
                            hash = sha.ComputeHash(stream);
                            if (Encoding.ASCII.GetString(hash) !=
                                Encoding.ASCII.GetString(StringToByteArray(fileHash)))
                            {
                                LogFile.AddMessage($"Hash Mismatch on file: {fileName}");
                                if (MessageBox.Show($"File Hash Mismatch.\n" +
                                    $"{ByteArrayToString(StringToByteArray(fileHash))}" +
                                    $" != {ByteArrayToString(hash)}\n" +
                                    $"Try Redownloading?", "File Hash Mismatch.",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                    i--;
                            }
                        }
                    }
                    Thread.Sleep(250);
                }
                // Closes the update dialog after all the file has been downloaded.
                Invoke(new Action(() => Close()));
            });
            // Starts the download thread.
            DownloadThread.Start();
        }

        private void CancelUpdateButton_Click(object sender, EventArgs e)
        {
            CancelUpdate = true;
            CancelUpdateButton.Enabled = false;
            CancelUpdateButton.Text = "Canceling..";
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBarFile.Maximum = (int) e.TotalBytesToReceive;
            ProgressBarFile.Value = (int) e.BytesReceived;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

    }
}
