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
using System.Net.Http;
using PropertyTools.Wpf;
using static HedgeModManager.Lang;
using PopupBox = HedgeModManager.Controls.PopupBox;
using System.Diagnostics;
using System.Windows.Shell;
using HedgeModManager.Misc;
using HedgeModManager.Exceptions;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for GBModWindow.xaml
    /// </summary>
    public partial class GBModWindow : Window
    {
        public Game Game;
        public string ItemType;
        public int ItemId;
        public string DownloadURL;
        public string Protocol;

        public GBModWindow(string itemType, int itemId, string dl, string protocol)
        {
            //DataContext = mod;
            //Game = game;

            Game = Games.Unknown;
            ItemType = itemType;
            ItemId = itemId;
            Protocol = protocol;
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
                BorderBrush = HedgeApp.GetThemeBrush("HMM.Window.ForegroundBrush"),
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

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.Visibility = Visibility.Collapsed;
            Progress.Visibility = Visibility.Visible;

            var compatibleGames = Games.GetSupportedGames()
                .Where(t => t.GBProtocol == Protocol)
                .ToList();

            var (gameInstall, result) = 
                ModInstallGameSelectorWindow.SelectGameInstall(compatibleGames);

            if (gameInstall == null)
            {
                DownloadButton.Visibility = Visibility.Visible;
                Progress.Visibility = Visibility.Collapsed;

                // No game
                if (result == true)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        var dialog = new HedgeMessageBox(Localise("CommonUIError"), LocaliseFormat("ModDownloaderNoGameMes", Localise($"Game{Game.GameName}")));
                        dialog.AddButton(Localise("CommonUIClose"), () =>
                        {
                            dialog.Close();
                            DialogResult = false;
                            Close();
                        });
                        dialog.ShowDialog();
                    });
                    return;
                }

                // Cancelled
                return;
            }

            try
            {
                var progress = new Progress<double?>((v) =>
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

                HedgeApp.Config = new CPKREDIRConfig(gameInstall);
                var mod = (GBAPIItemDataBasic)DataContext;

                using (var resp = await Singleton.GetInstance<HttpClient>().GetAsync(DownloadURL, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    resp.EnsureSuccessStatusCode();
                    Directory.SetCurrentDirectory(Path.GetDirectoryName(HedgeApp.AppPath));

                    var destinationPath = Path.GetFileName(resp.RequestMessage.RequestUri.AbsoluteUri);
                    using (var destinationFile = File.Create(destinationPath, 8192, FileOptions.Asynchronous))
                        await resp.Content.CopyToAsync(destinationFile, progress);
                    
                    ModsDB.InstallMod(destinationPath, Path.Combine(gameInstall.GameDirectory, Path.GetDirectoryName(HedgeApp.Config.ModsDbIni)));
                    File.Delete(destinationPath);

                    // a dialog would be nice here but i ain't adding strings
                    await Dispatcher.InvokeAsync(() =>
                    {
                        DialogResult = true;
                        Close();
                    });
                }
            }
            catch (ModInstallException)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    var dialog = new HedgeMessageBox(Localise("CommonUIError"), Localise("DialogUINoDecompressor"));
                    dialog.AddButton(Localise("CommonUIClose"), () =>
                    {
                        dialog.Close();
                        DialogResult = false;
                        Close();
                    });
                    dialog.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    var dialog = new HedgeMessageBox(Localise("ModDownloaderFailed"),
                        string.Format(Localise("ModDownloaderWebError"), ex.Message),
                        HorizontalAlignment.Right, TextAlignment.Center, InputType.MarkDown);
                    dialog.Owner = this;

                    dialog.AddButton(Localise("CommonUIRetry"), () =>
                    {
                        dialog.Close();
                        Download_Click(sender, e);
                    });

                    dialog.AddButton(Localise("CommonUICancel"), () =>
                    {
                        dialog.Close();
                        DialogResult = false;
                        Close();
                    });

                    dialog.ShowDialog();
                });
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GBAPIItemDataBasic mod)
            {
                PopulateUI(mod);
            }
            else
            {
                LoadingGrid.Visibility = Visibility.Visible;

                var item = new GBAPIItemDataBasic(ItemType, ItemId);
                item = await GBAPI.PopulateItemDataAsync(item).ConfigureAwait(false);

                var game = Games.Unknown;
                foreach (var gam in Games.GetSupportedGames())
                {
                    if (gam.GBProtocol == Protocol)
                    {
                        game = gam;
                        break;
                    }
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    if (game == Games.Unknown || item.ModName == null)
                    {
                        HedgeApp.CreateOKMessageBox("Error", $"Invalid GameBanana item!").ShowDialog();
                        DialogResult = false;
                        Close();
                        return;
                    }

                    Game = game;
                    DataContext = item;
                    PopulateUI(item);
                });
            }
        }

        private void PopulateUI(GBAPIItemDataBasic mod)
        {
            LoadingGrid.Visibility = Visibility.Collapsed;

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

            if (!string.IsNullOrEmpty(mod.SoundURL?.AbsoluteUri))
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

            if (HedgeApp.IsLinux)
            {
                Description.Text = string.Empty;
                Description.Visibility = Visibility.Collapsed;
                DescriptionText.Text = mod.Body;
                DescriptionText.Visibility = Visibility.Visible;
            }
            else
            {
                Description.Text = $@"
<html>
    <body>
        <style>
            {HedgeMessageBox.GetHtmlStyleSheet()}
        </style>
        {mod.Body}
    </body>
</html>";
            }
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
                    Foreground = HedgeApp.GetThemeBrush("HMM.Window.LightForegroundBrush"),
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