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


        public DownloadWindow(string header, string url, string destinationFile)
        {
            InitializeComponent();
            Title = header;
            Header.Text = header;
            URL = url;
            DestinationPath = destinationFile;
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
                await HedgeApp.HttpClient.DownloadFileAsync(URL, DestinationPath).ConfigureAwait(false);
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
