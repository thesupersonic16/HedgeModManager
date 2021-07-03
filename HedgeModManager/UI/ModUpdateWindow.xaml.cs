﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using HedgeModManager.Misc;
using HMMResources = HedgeModManager.Properties.Resources;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ModUpdateWindow.xaml
    /// </summary>
    public partial class ModUpdateWindow : Window
    {
        public Action DownloadCompleted;
        protected ModUpdate.ModUpdateInfo UpdateInfo;
        protected HedgeMessageBox WarningDialog;
        protected int CurrentFile = 0;
        private bool Cancelled = false;
        private IProgress<double?> _progress;

        public ModUpdateWindow(ModUpdate.ModUpdateInfo info)
        {
            InitializeComponent();
            UpdateInfo = info;
            Title = $"Downloading update for {info.Mod.Title}";
            TotalProgress.Maximum = info.Files.Count;

            _progress = new Progress<double?>((v) =>
            {
                if (v.HasValue)
                {
                    FileProgress.IsIndeterminate = false;
                    FileProgress.Value = v.Value;
                }
                else
                {
                    FileProgress.IsIndeterminate = true;
                }
            });
        }

        public void Start()
        {
            _ = Task.Run(() => ProcessAsync());
            ShowDialog();
        }

        protected async Task ProcessAsync()
        {
            for (CurrentFile = 0; CurrentFile < UpdateInfo.Files.Count; CurrentFile++)
            {
                if (Cancelled)
                    break;

                try
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        TotalProgress.Value = CurrentFile + 1;
                        TaskbarItemInfo.ProgressValue = (CurrentFile + 1) / (double)UpdateInfo.Files.Count;
                    });
                    var file = UpdateInfo.Files[CurrentFile];

                    switch (file.Command)
                    {
                        case "add":
                            // Creates directories
                            var fileInfo = new FileInfo(Path.Combine(UpdateInfo.Mod.RootDirectory, file.FileName));
                            if (!fileInfo.Directory.Exists)
                                Directory.CreateDirectory(fileInfo.Directory.FullName);

                            await Dispatcher.InvokeAsync(() =>
                            {
                                Header.Text = $"Downloading {file.FileName}";
                                AddLogLine($"Adding {file.FileName}");
                            });

                            await HedgeApp.HttpClient.DownloadFileAsync(file.URL, fileInfo.FullName, _progress).ConfigureAwait(false);
                            break;

                        case "delete":
                            await Dispatcher.InvokeAsync(() =>
                            {
                                Header.Text = $"Delete {file.FileName}";
                                AddLogLine($"Deleting {file.FileName}");
                            });

                            if (File.Exists(Path.Combine(UpdateInfo.Mod.RootDirectory, file.FileName)))
                                File.Delete(Path.Combine(UpdateInfo.Mod.RootDirectory, file.FileName));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // todo: handle
                    break;
                }
            }

            await Dispatcher.InvokeAsync(() =>
            {
                Close();
                DownloadCompleted.Invoke();
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
            WarningDialog = new HedgeMessageBox("Hedge Mod Manager", string.Format(Lang.Localise("DialogUIModCancelUpdate"), UpdateInfo.Mod.Title));

            WarningDialog.AddButton("Yes", () =>
            {
                Close();
                Cancelled = true;
                WarningDialog.Close();
            });

            WarningDialog.AddButton("No", () =>
            {
                WarningDialog.Close();
            });

            WarningDialog.Show();
        }
    }
}
