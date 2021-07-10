using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using HedgeModManager.Misc;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        public string URL;
        public string DestinationPath;
        public Action DownloadCompleted;
        public Action<Exception> DownloadFailed;
        private IProgress<double?> _progress; 

        public DownloadWindow(string header, string url, string destinationFile)
        {
            InitializeComponent();
            Title = header;
            Header.Text = header;
            URL = url;
            DestinationPath = destinationFile;

            // this is here because it captures the current SynchronizationContext
            // for threading purposes
            _progress = new Progress<double?>((v) =>
            {
                if (v.HasValue)
                {
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    Progress.IsIndeterminate = false;
                    TaskbarItemInfo.ProgressValue = v.Value;
                    Progress.Value = v.Value;
                }
                else
                {
                    Progress.IsIndeterminate = true;
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                }
            });
        }

        public void Start()
        {
            _ = Task.Run(() => DoDownloadAsync());

            BringIntoView();
            ShowDialog();
        }

        private async Task DoDownloadAsync()
        {
            try
            {
                await Singleton.GetInstance<HttpClient>().DownloadFileAsync(URL, DestinationPath, _progress).ConfigureAwait(false);
                await Dispatcher.InvokeAsync(() =>
                {
                    Close();
                    DownloadCompleted?.Invoke();
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Close();
                    DownloadFailed?.Invoke(ex);
                });
            }
        }
    }
}
