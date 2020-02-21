using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameBananaAPI;
using System.Net;
using System.Windows.Controls.Primitives;
using HedgeModManager.Controls;
using System.Net.Cache;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for GBModWindow.xaml
    /// </summary>
    public partial class GBModWindow : Window
    {
        public Game Game;
        public string DownloadURL;
        public GBModWindow(GBAPIItemDataBasic mod, string dl, Game game)
        {
            DataContext = mod;
            Game = game;
            DownloadURL = dl;
            InitializeComponent();
        }

        private void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            var shot = (GBAPIScreenshotData)((Button)sender).Tag;
            var popup = new PopupBox();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(shot.URL);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            popup.Children.Add(new Border()
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.White,
                Child = new Image()
                {
                    Source = image,
                    Stretch = Stretch.Fill
                }
            });
            popup.Show(this);
        }

        private void Audio_Click(object sender, RoutedEventArgs e)
        {
            var mod = (GBAPIItemDataBasic)DataContext;
            var url = mod.SoundURL;
            var popup = new PopupBox();
            popup.Children.Add(new Border()
            {
                Child = new AudioPlayer()
                {
                    Source = url,
                }
            });
            popup.Show(this);
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            var mod = (GBAPIItemDataBasic)DataContext;
            var root = HedgeApp.GetSteamGame(Game).RootDirectory;
            var request = (HttpWebRequest)WebRequest.Create(DownloadURL);
            var response = request.GetResponse();
            var URI = response.ResponseUri.ToString();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(HedgeApp.AppPath));
            var downloader = new DownloadWindow($"Downloading {mod.ModName}", URI, Path.GetFileName(URI));
            downloader.DownloadCompleted = new Action(() =>
            {
                ModsDB.InstallMod(Path.GetFileName(URI), Path.Combine(root, "Mods"));
                File.Delete(Path.GetFileName(URI));
                Close();
            });
            downloader.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mod = (GBAPIItemDataBasic)DataContext;
            foreach (var screenshot in mod.Screenshots)
            {
                var button = new Button()
                {
                    Margin = new Thickness(2.5),
                    Width = 96,
                    Height = 54,
                    Tag = screenshot,
                };

                button.Content = new Image()
                {
                    Source = new BitmapImage(new Uri(screenshot.URLSmall)),
                    Stretch = Stretch.Fill,
                };
                button.Click += Screenshot_Click;
                Imagebar.Children.Add(button);
            }

            if(!string.IsNullOrEmpty(mod.SoundURL?.AbsoluteUri))
            {
                var button = new Button()
                {
                    Margin = new Thickness(2.5),
                    Width = 96,
                    Height = 54,
                };

                button.Content = new Image()
                {
                    Source = new BitmapImage(new Uri("/Resources/Graphics/AudioThumbnail.png", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                };
                button.Click += Audio_Click;
                Imagebar.Children.Add(button);
            }
            Description.Text = $@"
<html>
    <body>
        <style>
            {Properties.Resources.GBStyleSheet}
        </style>
        {mod.Body}
    </body>
</html>";
            foreach (var group in mod.Credits.Credits)
            {
                if (group.Value.Count < 1)
                    continue;

                CreditsPanel.Children.Add(new TextBlock()
                {
                    Text = group.Key,
                    FontSize = 12,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.LightGray,
                    Padding = new Thickness(0, 2, 0, 2)
                });

                foreach (var credit in group.Value)
                {
                    var block = new TextBlock()
                    {
                        Text = credit.MemberID == 0 ? credit.MemberName : string.Empty,
                        FontSize = 14,
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        Padding = new Thickness(0, 0, 0, .5)
                    };
                    if (credit.MemberID != 0)
                    {
                        var link = new Hyperlink();
                        var run = new Run();
                        link.NavigateUri = new Uri($"https://gamebanana.com/members/{credit.MemberID}");
                        run.Text = credit.MemberName;
                        link.Click += (x, y) => { System.Diagnostics.Process.Start(link.NavigateUri.ToString()); };
                        link.Inlines.Add(run);
                        block.Inlines.Add(link);
                    }

                    CreditsPanel.Children.Add(block);
                    if (!string.IsNullOrEmpty(credit.Role))
                        CreditsPanel.Children.Add(new TextBlock() { Text = credit.Role, FontSize = 11.2, TextWrapping = TextWrapping.WrapWithOverflow, Padding = new Thickness(0, 0, 0, .5) });
                }
            }
        }
    }
}