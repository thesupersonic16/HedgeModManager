using SS16;
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

namespace HedgeModManager
{
    public partial class UpdateModForm : Form
    {
        // Variables/Constants
        public Thread DownloadThread;
        public bool CancelUpdate = false;
        public Mod _Mod;
        public ModUpdater.ModUpdate ModUpdate;

        // Constructors
        public UpdateModForm(Mod mod, ModUpdater.ModUpdate update)
        {
            InitializeComponent();
            _Mod = mod;
            ModUpdate = update;
            if (mod == null)
            {
                Text = "Downloading " + update.Name;
                UpdateLabel.Text = "Downloading " + update.Name;
                UpdateLabel.Location = new Point(Size.Width / 2 - UpdateLabel.Size.Width / 2, UpdateLabel.Location.Y);
                DownloadLabel.Text = "Starting Download...";
                DownloadLabel.Location = new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y);

            }else
            {
                Text = "Updating " + update.Name;
                UpdateLabel.Text = "Updating " + update.Name;
                UpdateLabel.Location = new Point(Size.Width/2-UpdateLabel.Size.Width/2, UpdateLabel.Location.Y);
                DownloadLabel.Text = "Starting Download...";
                DownloadLabel.Location = new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y);
            }
        }

        // GUI Events
        private void UpdateModsForm_Load(object sender, EventArgs e)
        {
            Theme.ApplyDarkThemeToAll(this);
            var webClient = new WebClient();
            // Adds the WebClient_DownloadProgressChanged event to the web client.
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);

            DownloadThread = new Thread(() =>
            {
                // Sets the ProgressBarAll's maximum to the amount of files/lines.
                Invoke(new Action(() => ProgressBarAll.Maximum = ModUpdate.Files.Count));

                var sha = new SHA256Managed();
                byte[] hash;
                for (int i = 0; i < ModUpdate.Files.Count; ++i)
                {
                    string fileName = ModUpdate.Files[i].FileName;
                    string fileUrl = ModUpdate.Files[i].URL;
                    string fileSha = ModUpdate.Files[i].SHA256;
                    if (_Mod != null)
                    {
                        var fileInfo = new FileInfo(Path.Combine(_Mod.RootDirectory, fileName));

                        // Creates the directorys
                        if (!fileInfo.Directory.Exists)
                            Directory.CreateDirectory(fileInfo.Directory.FullName);
                    }

                    LogFile.AddMessage($"Downloading: {fileName} from {fileUrl}");
                    // Closes and returns if the user clicked cancel.
                    if (CancelUpdate) { Invoke(new Action(() => Close())); return; }
                    // Sets DownloadLabel's Text to show what file is being downloaded.
                    Invoke(new Action(() => DownloadLabel.Text = "Downloading... " + fileName));
                    // Centres DownloadLabel's position.
                    Invoke(new Action(() => DownloadLabel.Location =
                        new Point(Size.Width / 2 - DownloadLabel.Size.Width / 2, DownloadLabel.Location.Y)));
                    // Sets ProgressBarAll's Value to the current file.
                    Invoke(new Action(() => ProgressBarAll.Value = i));
                    // Checks if the mod is installed
                    if (_Mod == null)
                    {
                        webClient.DownloadDataCompleted += DownloadDataCompleted;
                        Invoke(new Action(() => webClient.DownloadDataAsync(new Uri(fileUrl))));
                    }
                    else
                    {
                        // Downloads the current file to the mod root.
                        Invoke(new Action(() => webClient.DownloadFileAsync(new Uri(fileUrl),
                            Path.Combine(_Mod.RootDirectory, fileName))));
                    }
                    // Waits for the download to finish.
                    while (webClient.IsBusy)
                    {
                        Thread.Sleep(100);
                    }
                    if (_Mod == null)
                    {
                        continue;
                    }

                    using (var stream = File.OpenRead(Path.Combine(_Mod.RootDirectory, fileName)))
                    {
                        if (fileSha != 0.ToString("X64") && fileSha != null)
                        {
                            hash = sha.ComputeHash(stream);
                            if (Encoding.ASCII.GetString(hash) !=
                                Encoding.ASCII.GetString(StringToByteArray(fileSha)))
                            {
                                LogFile.AddMessage($"Hash Mismatch on file: {fileName}");
                                if (MessageBox.Show($"File Hash Mismatch.\n" +
                                    $"{ByteArrayToString(StringToByteArray(fileSha))}" +
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
            if (e.TotalBytesToReceive != -1)
            {
                ProgressBarFile.Style = ProgressBarStyle.Blocks;
                ProgressBarFile.Maximum = (int) e.TotalBytesToReceive;
                ProgressBarFile.Value = (int) e.BytesReceived;
            }else
                ProgressBarFile.Style = ProgressBarStyle.Marquee;
        }

        private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            byte[] bytes = e.Result;
            string tempFolder = Path.Combine(Program.StartDirectory, "downloads");
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);
            string filePath = Path.Combine(tempFolder, "download.bin");
            File.WriteAllBytes(filePath, bytes);
            if (bytes[0] == 'P')
                AddModForm.InstallFromZip(filePath); // Install from a zip
            else
                AddModForm.InstallFrom7zArchive(filePath); // Use 7Zip if its not a Zip
            Invoke(new Action(() => Close()));
            return;
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
