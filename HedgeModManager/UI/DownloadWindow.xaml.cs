using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Shell;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        public string URL;
        public string DestinationPath;
        public WebClient DownloadClient;
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
            DownloadClient = new WebClient();
            DownloadClient.Headers.Add("user-agent", HedgeApp.WebRequestUserAgent);
            DownloadClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            DownloadClient.DownloadFileCompleted += WebClient_DownloadCompleted;
            DownloadClient.DownloadFileAsync(new Uri(URL), DestinationPath);
            ShowDialog();
        }

        protected void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                Progress.Maximum = args.TotalBytesToReceive;
                Progress.Value = args.BytesReceived;
                TaskbarItemInfo.ProgressValue = (float)args.BytesReceived / (float)args.TotalBytesToReceive;
            });
        }

        protected void WebClient_DownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                Close();

                if(args.Error == null)
                    DownloadCompleted?.Invoke();
                else
                    DownloadFailed?.Invoke(args.Error);
            });
        }
    }
}
