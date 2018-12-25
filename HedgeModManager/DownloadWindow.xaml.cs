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
        
        public DownloadWindow(string header, string url, string destinationFile)
        {
            InitializeComponent();
            Header.Text = header;
            URL = url;
            DestinationPath = destinationFile;
        }

        public void Start()
        {
            Show();
            DownloadClient = new WebClient();
            DownloadClient.Headers.Add("user-agent", App.WebRequestUserAgent);
            DownloadClient.DownloadProgressChanged += Client_Download_Progress_Changed;
            DownloadClient.DownloadFileCompleted += Client_Download_Completed;
            DownloadClient.DownloadFileAsync(new Uri(URL), DestinationPath);
        }

        protected void Client_Download_Progress_Changed(object sender, DownloadProgressChangedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                Progress.Maximum = args.TotalBytesToReceive;
                Progress.Value = args.BytesReceived;
                TaskbarItemInfo.ProgressValue = (float)args.BytesReceived / (float)args.TotalBytesToReceive;
            });
        }

        protected void Client_Download_Completed(object sender, AsyncCompletedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                Close();
                DownloadCompleted.Invoke();
            });
        }
    }
}
