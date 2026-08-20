using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using File = System.IO.File;
using Path = System.IO.Path;
using static HedgeModManager.Lang;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window, INotifyPropertyChanged
    {

        private Exception Exception;
        private string ExtraInfo;
        private GitHub.ReleaseInfo ReleaseInfo = null;
        // This should be kept in English as its used in creating reports
        public string UpdateStatus { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string ReportRepository { get; set; } = "https://github.com/thesupersonic16/HedgeModManager";

        public ExceptionWindow(Exception exception, string extraInfo = "") : this()
        {
            Exception = exception;
            ExtraInfo = extraInfo;
        }

        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public string GetReport(bool useMarkdown = false, bool showInstructions = false)
        {
            var body = new StringBuilder();

            if (showInstructions)
            {
                body.AppendLine("<!-- Please explain below any relevant information which may have caused this crash like steps to reproduce the issue -->");
                body.AppendLine();
                body.AppendLine();
                body.AppendLine();

                body.AppendLine("<!-- Please drag the snapshot file (text file) below -->");
                body.AppendLine();
                body.AppendLine();
                body.AppendLine();

                body.AppendLine("<!-- Please do not modify the contents below -->");
                body.AppendLine();
            }

            body.AppendLine($"HMM Info:");

            if (useMarkdown) body.AppendLine("```");
            body.AppendLine($"    Version: {HedgeApp.VersionString}");
            body.AppendLine($"    Args: {string.Join(" ", HedgeApp.Args)}");
            body.AppendLine($"    StartDir: {HedgeApp.StartDirectory}");
            body.AppendLine($"    Process Level: " + (HedgeApp.RunningAsAdmin() ? "Administrator" : "User"));
            try
            {
                body.AppendLine($"    GameInstall: {HedgeApp.CurrentGameInstall}");
            } catch { }
            if (UpdateStatus != null) body.AppendLine($"    Update Status: {UpdateStatus}");
            if (useMarkdown) body.AppendLine("```");

            body.AppendLine("");

            if (!string.IsNullOrEmpty(ExtraInfo))
            {
                body.AppendLine($"Extra Information: {ExtraInfo}");
            }

            if (Exception != null)
            {
                body.AppendLine($"Exception:");
                if (useMarkdown) body.AppendLine("```");

                if (Exception.GetType() != typeof(Exception))
                {
                    body.AppendLine($"    Type: {Exception.GetType().Name}");
                }

                body.AppendLine($"    Message: {Exception.Message}");

                if (!string.IsNullOrEmpty(Exception.Source))
                {
                    body.AppendLine($"    Source: {Exception.Source}");
                }

                if (Exception.TargetSite != null)
                {
                    body.AppendLine($"    Function: {Exception.TargetSite}");
                }

                if(Exception.StackTrace != null)
                    body.AppendLine($"    StackTrace: \n    {Exception.StackTrace.Replace("\n", "\n    ")}");

                if (Exception.InnerException != null)
                {
                    body.AppendLine($"    InnerException: {Exception.InnerException}");
                }

                if (useMarkdown) body.AppendLine("```");
                body.AppendLine("");
            }

            return body.ToString();
        }

        public static void UnhandledExceptionEventHandler(Exception e, bool unhandled = false)
        {
            var window = new ExceptionWindow(e);
            Application.Current.MainWindow = window;
            if (unhandled)
                window.Header.Content = Localise("ExceptionWindowUnhandled");
            window.ShowDialog();
        }

        private void Button_CopyLog_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetReport(true));
        }

        private void Button_Ignore_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Report_Click(object sender, RoutedEventArgs e)
        {
            string url = $"{ReportRepository}/issues/new";
            url += $"?title=[{HedgeApp.CurrentGameInstall.Game?.GameName}] ";
            url += $"&body={Uri.EscapeDataString(GetReport(true, true))}";
            Process.Start(url);

            try
            {
                var time = DateTime.Now;
                var path =
                    $"HMM_Snapshot_{time.Date:00}{time.Month:00}{time.Year:0000}{time.Hour:00}{time.Minute:00}{time.Second:00}.txt";
#if DEBUG
                File.WriteAllBytes(path, SnapshotBuilder.Build(false,
                        new SnapshotFile("Exception.txt", Encoding.UTF8.GetBytes(Exception.ToString()))));
#else
                File.WriteAllText(path,
                    Convert.ToBase64String(SnapshotBuilder.Build(true,
                        new SnapshotFile("Exception.txt", Encoding.UTF8.GetBytes(Exception.ToString())))));
#endif

                Process.Start($"explorer.exe", $"/select,\"{Path.GetFullPath(path)}\"");
                HedgeApp.CreateOKMessageBox("Hedge Mod Manager", $"Please attach the file\n{path}\nto the issue.").ShowDialog();
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_ExceptionInfo.Text = GetReport();
            DataContext = this;
            Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    UpdateStatus = "Checking For Updates";
                    var update = await HedgeApp.CheckForUpdatesAsync();
                    UpdateStatus = "Parsing";
                    if (!update.Item1)
                        UpdateStatus = "Up To Date";
                    else
                    {
                        UpdateStatus = "Update Required";
                        ReleaseInfo = update.Item2;
                        var window = new HedgeMessageBox(
                            Localise("ExceptionWindowNewUpdateTitle"), Localise("ExceptionWindowNewUpdateText"));
                        window.AddButton(Localise("CommonUIUpdate"), () =>
                        {
                            window.Close();
                            if (ReleaseInfo.Assets.Count > 0)
                            {
                                // http://wasteaguid.info/
                                var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");

                                var asset = ReleaseInfo.Assets[0];
                                var downloader = new DownloadWindow($"Downloading Hedge Mod Manager ({ReleaseInfo.TagName})", asset.BrowserDownloadUrl.ToString(), path)
                                {
                                    DownloadCompleted = () =>
                                    {
                                        try
                                        {
                                            HedgeApp.PerformUpdate(path, asset.ContentType);
                                        }catch
                                        {
                                            // Try clean up
                                            try { File.Delete(path); } catch { }
                                            UpdateStatus = "Update Install Failed";
                                            var dialog = new HedgeMessageBox(Localise("ExceptionWindowUpdateErrorTitle"),
                                                Localise("ExceptionWindowUpdateErrorText"));
                                            dialog.AddButton(Localise("CommonUIOK"), () =>
                                            {
                                                Process.Start($"https://github.com/{HedgeApp.RepoOwner}/{HedgeApp.RepoName}/releases");
                                                dialog.Close();
                                            });
                                            dialog.ShowDialog();
                                        }
                                    }
                                };

                                downloader.Start();
                            }
                        });

                        window.AddButton(Localise("CommonUIIgnore"), () =>
                        {
                            UpdateStatus = "Update Ignored";
                            window.Close();
                        });

                        window.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus = "Failed: " + ex.ToString();
                }
            });
        }
    }
}
