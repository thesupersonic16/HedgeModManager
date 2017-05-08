using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class UpdateForm : Form
    {

        public Thread thread;
        public string url;

        public UpdateForm(string url)
        {
            InitializeComponent();
            this.url = url;
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            var wc = new WebClient();

            thread = new Thread(() =>
            {
                // Update batch file if you are changing this.
                string tempPath = Path.Combine(Program.StartDirectory, "updateTemp");
                // Creates a temp directory to store all the update files.
                Directory.CreateDirectory(tempPath);
                
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);
                // Starts downloading the update zip file.
                Invoke(new Action(() => wc.DownloadFileAsync(new Uri(url), Path.Combine(tempPath, "SLWModLoaderUpdate.zip"))));
                // Waits for the update to finish.
                while (wc.IsBusy)
                {
                    Thread.Sleep(50);
                }

                LogFile.AddMessage("Finished Downloading update. Installing");

                LogFile.AddMessage("Extracting Update...");
                // Extract the update files.
                ZipFile.ExtractToDirectory(Path.Combine(tempPath, "SLWModLoaderUpdate.zip"), tempPath);
                // Deletes the zip file as we don't need it anymore.
                File.Delete(Path.Combine(tempPath, "SLWModLoaderUpdate.zip"));
                LogFile.AddMessage("Finished extracting update.");

                // Writes the batch file so we can continue the update process.
                File.WriteAllBytes(Path.Combine(Program.StartDirectory, "update.bat"), Properties.Resources.update);
                
                // Runs the batch file and closes the mod loader.
                LogFile.AddMessage($"Closing {Program.ProgramName} to continue the update...");
                new Process(){ StartInfo = new ProcessStartInfo(Path.Combine(Program.StartDirectory, "update.bat")) }.Start();
                Application.Exit();
            });
            thread.Start();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Maximum = (int)e.TotalBytesToReceive;
            progressBar.Value = (int)e.BytesReceived;
        }
    }
}
