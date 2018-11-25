using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using GameBananaAPI.Core;
using Path = System.IO.Path;
using mshtml;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for GBModDownloadWindow.xaml
    /// </summary>
    public partial class GameBananaModDownloadWindow : Window
    {
        public string URL;
        public GBAPIItemDataBasic Item;
        public WebClient WebClient = new WebClient();

        public GameBananaModDownloadWindow(GBAPIItemDataBasic item, string url)
        {
            InitializeComponent();
            URL = url;
            Item = item;
            ModTitleLabel.Content = item.ModName;
            ModSubtitleLabel.Content = item.Subtitle;
            SubmitterLabel.Content = new TextBlock() { Text = ProcessCredits(item.Credits), TextWrapping = TextWrapping.Wrap };

            // Description
            DescriptionViewer.LoadCompleted += DescriptionViewer_LoadCompleted;
            DescriptionViewer.NavigateToString(item.Body);

            WebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            WebClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
            
            // Loads an Image if the submission contains one
            if (item.ScreenshotURL != null)
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(item.ScreenshotURL);
                imageSource.EndInit();
                SubmissionImage.Source = imageSource;
            }

        }

        public static string ProcessCredits(GBAPICredit[] credits)
        {
            string s = "";
            for (int i = 0; i < credits.Length; ++i)
            {
                s += $"- {credits[i].MemberName}\n   {credits[i].Role}\n";
            }
            return s;
        }

        private void DescriptionViewer_LoadCompleted(object sender, EventArgs e)
        {
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            ProgressGrid.Visibility = Visibility.Visible;
            PrograssI.Width = (PrograssO.ActualWidth - 2d);
            ResizeMode = ResizeMode.NoResize;
            new Thread(() =>
            {
                // Gets URL to file
                Dispatcher.Invoke(new Action(() => PrograssL1.Content = "Requesting Download Link..."));
                var request = (HttpWebRequest)WebRequest.Create(URL);
                var response = request.GetResponse();
                var URI = response.ResponseUri;
                response.Close();
                Dispatcher.Invoke(new Action(() => PrograssL1.Content = "Starting Download..."));
                WebClient.DownloadDataAsync(URI);
            }).Start();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.TotalBytesToReceive != -1)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    double w = (PrograssO.ActualWidth - 2d);
                    PrograssI.Width = w - (e.BytesReceived / (float)e.TotalBytesToReceive) * w;
                    PrograssL1.Content = (int)((float)e.BytesReceived / e.TotalBytesToReceive * 100f) + "%";
                }));

            }
        }

        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => PrograssL1.Content = "Installing..."));
            var bytes = e.Result;
            new Thread(() => 
            {
                string tempFolder = Path.Combine(Program.StartDirectory, "downloads");
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);
                string filePath = Path.Combine(tempFolder, "download.bin");
                File.WriteAllBytes(filePath, bytes);
                if (bytes[0] == 'P')
                    AddModForm.InstallFromZip(filePath); // Install from a zip
                else
                    AddModForm.InstallFrom7zArchive(filePath); // Use 7-Zip or WinRAR if its not a Zip
                Dispatcher.Invoke(new Action(() => Close()));
            }).Start();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Closes the Window
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
        }
    }
}
