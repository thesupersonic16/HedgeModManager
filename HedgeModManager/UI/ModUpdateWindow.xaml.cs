using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HedgeModManager;

using HMMResources = HedgeModManager.Properties.Resources;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ModUpdateWindow.xaml
    /// </summary>
    public partial class ModUpdateWindow : Window
    {
        public WebClient DownloadClient;
        public Action DownloadCompleted;
        protected ModUpdate.ModUpdateInfo UpdateInfo;
        protected HedgeMessageBox WarningDialog;
        protected int CurrentFile = 0;

        public ModUpdateWindow(ModUpdate.ModUpdateInfo info)
        {
            InitializeComponent();
            UpdateInfo = info;
            Title = $"Downloading update for {info.Mod.Title}";
            TotalProgress.Maximum = info.Files.Count;
        }

        public void Start()
        {
            DownloadClient = new WebClient();
            DownloadClient.Headers.Add("user-agent", HedgeApp.WebRequestUserAgent);
            DownloadClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            DownloadClient.DownloadFileCompleted += WebClient_DownloadCompleted;
            ProcessCommand();
            ShowDialog();
        }

        protected void ProcessCommand()
        {
            if (CurrentFile >= UpdateInfo.Files.Count)
            {
                Close();
                DownloadCompleted.Invoke();
                return;
            }

            var file = UpdateInfo.Files[CurrentFile];
            
            switch(file.Command)
            {
                case "add":
                    // Creates directories
                    var fileInfo = new FileInfo(Path.Combine(UpdateInfo.Mod.RootDirectory, file.FileName));
                    if (!fileInfo.Directory.Exists)
                        Directory.CreateDirectory(fileInfo.Directory.FullName);

                    DownloadClient.DownloadFileAsync(new Uri(file.URL), fileInfo.FullName);
                    Header.Text = $"Downloading {file.FileName}";
                    AddLogLine($"Adding {file.FileName}");
                    break;

                case "delete":
                    Header.Text = $"Delete {file.FileName}";
                    AddLogLine($"Deleting {file.FileName}");

                    if(File.Exists(Path.Combine(UpdateInfo.Mod.RootDirectory, file.FileName)))
                        File.Delete(Path.Combine(UpdateInfo.Mod.RootDirectory, file.FileName));

                    CurrentFile++;
                    ProcessCommand();
                    break;
            }

            CurrentFile++;
            TotalProgress.Value = CurrentFile;
        }

        protected void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                FileProgress.Maximum = args.TotalBytesToReceive;
                FileProgress.Value = args.BytesReceived;
                TaskbarItemInfo.ProgressValue = (float)args.BytesReceived / (float)args.TotalBytesToReceive;
            });
        }

        protected void WebClient_DownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                FileProgress.Value = 0;
                ProcessCommand();
            });
        }

        protected void AddLog(string str)
        {
            Log.Text += str;
        }

        protected void AddLogLine(string str)
        {
            AddLog($"{str}\r\n");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (CurrentFile >= UpdateInfo.Files.Count)
            {
                WarningDialog?.Close();
                return;
            }

            if (WarningDialog != null)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = true;
            WarningDialog = new HedgeMessageBox("Hedge Mod Manager", string.Format(HMMResources.STR_CANCEL_WARNING, UpdateInfo.Mod.Title));

            WarningDialog.AddButton("No", () => 
            {
                WarningDialog.Close();
            });

            WarningDialog.AddButton("Yes", () => 
            {
                Close();
                WarningDialog.Close();
            });

            WarningDialog.Show();
        }
    }
}
