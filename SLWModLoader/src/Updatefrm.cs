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
    public partial class Updatefrm : Form
    {
        public static string latest, latestversion;
        private Thread updatethread;

        public Updatefrm()
        {
            InitializeComponent();

            Mainfrm.client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

            updatethread = new Thread(new ThreadStart(UpdateApplication));
            updatethread.Start();
        }

        private void UpdateApplication()
        {
            try
            {
                Mainfrm.logfile.Add("Started downloading update..."); Mainfrm.logfile.Add("");

                //Download the update
                if (Directory.Exists(Application.StartupPath + "\\temp")) { Directory.Delete(Application.StartupPath + "\\temp", true); }
                Directory.CreateDirectory(Application.StartupPath + "\\temp");
                Mainfrm.client.DownloadFileAsync(new Uri(latest.Substring(latest.IndexOf("browser_download_url") + 23, latest.IndexOf(".zip", latest.IndexOf("browser_download_url") + 23) + 4 - (latest.IndexOf("browser_download_url") + 23))), Application.StartupPath + "\\temp\\SLWModLoaderupdate.zip");

                //Wait until the update has been downloaded
                while (Mainfrm.client.IsBusy) { }

                Mainfrm.logfile.Add("Finished downloading update.");
                Mainfrm.logfile.Add("Started extracting update...");

                //Extract the update
                Invoke(new Action(() => { statuslbl.Text = "Extracting Update..."; }));
                ZipFile.ExtractToDirectory(Application.StartupPath + "\\temp\\SLWModLoaderupdate.zip", Application.StartupPath + "\\temp");
                File.Delete(Application.StartupPath + "\\temp\\SLWModLoaderupdate.zip");

                Mainfrm.logfile.Add("Finished extracting update.");
                Mainfrm.logfile.Add("Started writing batch file to install update...");

                //Make a batch file to copy the extracted update files over the current version of the application
                File.WriteAllLines(Application.StartupPath + "\\temp\\update.bat", new string[] { "@title Updating...", "@echo Waiting 3 seconds to ensure process is closed...", "@ping 192.0.2.2 -n 1 -w 3000 > nul", "@echo Copying update over old version of application...", $"@xcopy /s /y \"{Application.StartupPath + "\\temp"}\" \"{Application.StartupPath}\" > nul", "@echo Starting application...", $"@start \"\" \"{Application.StartupPath + "\\SLWModLoader"}\"" });
                Mainfrm.logfile.Add("Finished writing batch file to install update.");

                //Run that batch file, and close the application
                Mainfrm.logfile.Add("Closing application to install update...");
                Process updateprocess = new Process() { StartInfo = new ProcessStartInfo(Application.StartupPath + "\\temp\\update.bat") };
                updateprocess.Start();
            }
            catch (Exception ex)
            {
                Mainfrm.logfile.Add("ERROR: An error has occured and the application could not be updated. Please try again." + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + Environment.NewLine + "ADVANCED INFO (FOR DEBUGGING): " + Environment.NewLine + Environment.NewLine + ex.ToString());
                Invoke(new Action(() => { MessageBox.Show("An error has occured and the application could not be updated. Please try again.","SLW Mod Loader",MessageBoxButtons.OK,MessageBoxIcon.Error); }));
            }

            Application.Exit();
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke(new Action(() => { progressBar.Value = e.ProgressPercentage; }));
        }
    }
}
