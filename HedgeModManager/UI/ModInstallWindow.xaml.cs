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
using Newtonsoft.Json;
using Markdig;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for GBModWindow.xaml
    /// </summary>
    public partial class ModInstallWindow : Window
    {

        private string URL;
        public ModDownload ModDownloadInfo { get; set; }

        public ModInstallWindow(string url)
        {
            URL = url;
            InitializeComponent();
        }

        private void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            var url = ((Button)sender).Tag as string;
            var popup = new PopupBox();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(url);
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

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.Visibility = Visibility.Collapsed;
            Progress.Visibility = Visibility.Visible;

            var (gameInstall, result) =
                ModInstallGameSelectorWindow.SelectGameInstall(new List<Game>() { ModDownloadInfo.Game });

            if (gameInstall == null)
            {
                DownloadButton.Visibility = Visibility.Visible;
                Progress.Visibility = Visibility.Collapsed;

                // No game
                if (result == true)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        var dialog = new HedgeMessageBox(Localise("CommonUIError"), LocaliseFormat("ModDownloaderNoGameMes", Localise($"Game{ModDownloadInfo.Game.GameName}")));
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

                if (gameInstall == null)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        var dialog = new HedgeMessageBox(Localise("CommonUIError"), LocaliseFormat("ModDownloaderNoGameMes", Localise($"Game{ModDownloadInfo.Game.GameName}")));
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

                // Load game config
                HedgeApp.Config = new CPKREDIRConfig(gameInstall);

                using (var resp = await Singleton.GetInstance<HttpClient>().GetAsync(ModDownloadInfo.DownloadURL, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    resp.EnsureSuccessStatusCode();
                    Directory.SetCurrentDirectory(Path.GetDirectoryName(HedgeApp.AppPath));

                    var destinationPath = Path.GetFileName(resp.RequestMessage.RequestUri.AbsoluteUri);
                    using (var destinationFile = File.Create(destinationPath, 8192, FileOptions.Asynchronous))
                        await resp.Content.CopyToAsync(destinationFile, progress);

                    ModsDB.InstallMod(destinationPath, Path.Combine(gameInstall.GameDirectory, Path.GetDirectoryName(HedgeApp.Config.ModsDbIni)));
                    File.Delete(destinationPath);

                    // a dialog would be nice here but i ain't adding strings
                    // I agree
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
            if (ModDownloadInfo != null)
            {
                await Dispatcher.InvokeAsync(() => PopulateUI());
            }
            else
            {
                LoadingGrid.Visibility = Visibility.Visible;

                var json = await Singleton.GetInstance<HttpClient>().GetStringAsync(URL).ConfigureAwait(false);

                if (json != null)
                {
                    ModDownloadInfo = JsonConvert.DeserializeObject<ModDownload>(json);
                    await Dispatcher.InvokeAsync(() =>
                    {
                        DataContext = ModDownloadInfo;
                        PopulateUI();
                    });
                }
                if (ModDownloadInfo == null || ModDownloadInfo.Game == null)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        // TODO: May want to make this translatable
                        HedgeApp.CreateOKMessageBox("Error", $"Invalid mod download file!").ShowDialog();
                        DialogResult = false;
                        Close();
                    });
                    return;
                }
            }
        }

        private void PopulateUI()
        {
            LoadingGrid.Visibility = Visibility.Collapsed;

            foreach (var image in ModDownloadInfo.Images)
            {
                var button = new Button()
                {
                    Margin = new Thickness(2.5),
                    Width = 96,
                    Height = 54,
                    Tag = image
                };

                button.Content = new Image()
                {
                    Source = new BitmapImage(new Uri(image)),
                    Stretch = Stretch.Fill,
                };
                button.Click += Screenshot_Click;
                Imagebar.Children.Add(button);
            }

            // TODO: Include audio

            // TODO: Fix rendering issue on Linux
            if (HedgeApp.IsLinux)
            {
                Description.Text = string.Empty;
                Description.Visibility = Visibility.Collapsed;
                DescriptionText.Text = ModDownloadInfo.Description;
                DescriptionText.Visibility = Visibility.Visible;
            }
            else
            {
                Description.Text = $"<html><body><style>{HedgeMessageBox.GetHtmlStyleSheet()}</style>" +
                    $"{Markdown.ToHtml(ModDownloadInfo.Description, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build())}</body></html>";
            }
            
            foreach (var group in ModDownloadInfo.Authors)
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

                foreach (var author in group.Value)
                {
                    var block = new TextBlock()
                    {
                        Text = author.Link == null ? author.Name: string.Empty,
                        FontSize = 14,
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        Padding = new Thickness(0, 0, 0, .5)
                    };
                    if (author.Link != null)
                    {
                        var link = new Hyperlink();
                        var run = new Run();
                        link.NavigateUri = new Uri(author.Link);
                        run.Text = author.Name;
                        link.Click += (x, y) => { Process.Start(link.NavigateUri.ToString()); };
                        link.Inlines.Add(run);
                        block.Inlines.Add(link);
                    }

                    CreditsPanel.Children.Add(block);
                    if (!string.IsNullOrEmpty(author.Description))
                        CreditsPanel.Children.Add(new TextBlock() { Text = author.Description, FontSize = 11.2, TextWrapping = TextWrapping.WrapWithOverflow, Padding = new Thickness(0, 0, 0, .5) });
                }
            }
        }
    }
}