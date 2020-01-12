using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using HedgeModManager.UI;
using System.Collections.ObjectModel;

using HMMResources = HedgeModManager.Properties.Resources;
using System.Net;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsCPKREDIRInstalled = false;
        public static ModsDB ModsDatabase;
        public static CodeList CodesDatabase = new CodeList();
        public static List<FileSystemWatcher> ModsWatchers = new List<FileSystemWatcher>();
        public MainWindowViewModel ViewModel = new MainWindowViewModel();

        protected Timer StatusTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            RefreshMods();
            RefreshUI();
        }

        public void RefreshMods()
        {
            CodesList.Items.Clear();

            ModsDatabase = new ModsDB(App.ModsDbPath);
            ModsDatabase.DetectMods();
            ModsDatabase.GetEnabledMods();
            ModsDatabase.Mods.Sort((x, y) => x.Title.CompareTo(y.Title));

            // Re-arrange the mods
            for (int i = 0; i < ModsDatabase.ActiveModCount; i++)
            {
                for (int i2 = 0; i2 < ModsDatabase.Mods.Count; i2++)
                {
                    var mod = ModsDatabase.Mods[i2];
                    if (Path.GetFileName(mod.RootDirectory) == ModsDatabase.ActiveMods[i])
                    {
                        ModsDatabase.Mods.Remove(mod);
                        ModsDatabase.Mods.Insert(i, mod);
                    }
                }
            }

            CodesDatabase = CodeLoader.LoadAllCodes();
            ModsDatabase.Codes.ForEach((x) =>
            {
                var code = CodesDatabase.Codes.Find((y) => { return y.Name == x; });
                if(code != null)
                code.Enabled = true;
            });

            CodesDatabase.Codes.ForEach((x) =>
            {
                if (x.Enabled)
                    CodesList.Items.Insert(0, x);
                else
                    CodesList.Items.Add(x);
            });

            UpdateStatus($"Loaded {ModsDatabase.Mods.Count} mods");
        }

        public void RefreshUI()
        {
            // Sets the DataContext for all the Components
            ViewModel = new MainWindowViewModel
            {
                CPKREDIR = App.Config,
                ModsDB = ModsDatabase,
                Games = App.SteamGames,
                Mods = new ObservableCollection<ModInfo>(ModsDatabase.Mods)
            };
            DataContext = ViewModel;

            Title = $"{App.ProgramName} ({App.VersionString}) - {App.CurrentGame.GameName}";

            if (App.CurrentGame.HasCustomLoader && !App.CurrentGame.SupportsCPKREDIR)
            {
                Button_CPKREDIR.IsEnabled = true;
                Button_OtherLoader.IsEnabled = false;
            }
            else
            {
                Button_CPKREDIR.IsEnabled = App.CurrentGame.SupportsCPKREDIR;
                Button_OtherLoader.IsEnabled = App.CurrentGame.HasCustomLoader;
                Button_DownloadCodes.IsEnabled = App.CurrentGame.HasCustomLoader && !string.IsNullOrEmpty(App.CurrentGame.CodesURL);
            }

            var exeDir = App.StartDirectory;
            bool hasOtherModLoader = App.CurrentGame.HasCustomLoader ? File.Exists(Path.Combine(exeDir, $"d3d{App.CurrentGame.DirectXVersion}.dll")) : false;
            IsCPKREDIRInstalled = App.CurrentGame.SupportsCPKREDIR ? App.IsCPKREDIRInstalled(Path.Combine(exeDir, App.CurrentGame.ExecuteableName)) : hasOtherModLoader;
            string loaders = (IsCPKREDIRInstalled && App.CurrentGame.SupportsCPKREDIR ? App.CPKREDIRVersion : "");

            if (hasOtherModLoader)
            {
                if (string.IsNullOrEmpty(loaders))
                    loaders = $"{App.CurrentGame.CustomLoaderName}";
                else
                    loaders += $" & {App.CurrentGame.CustomLoaderName}";
            }

            if (string.IsNullOrEmpty(loaders))
                loaders = "None";

            ComboBox_GameStatus.SelectedValue = App.CurrentSteamGame;
            Label_MLVersion.Content = $"Loaders: {loaders}";
            Button_OtherLoader.Content = hasOtherModLoader && App.CurrentGame.SupportsCPKREDIR ? "Uninstall Code Loader" : "Install Code Loader";
            Button_CPKREDIR.Content = $"{(IsCPKREDIRInstalled ? "Uninstall" : "Install")} Mod Loader";
        }

        public void CheckForModUpdates(ModInfo mod, bool showUpdatedDialog = true)
        {
            new Thread(() => 
            {
                UpdateStatus($"Checking for {mod.Title} updates");

                // Downloads the mod update information
                Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                var update = ModUpdate.GetUpdateFromINI(mod);
                if (update == null)
                {
                    Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Arrow);
                    UpdateStatus(string.Empty);
                    return;
                }

                Dispatcher.Invoke(() => 
                {
                    UpdateStatus(string.Empty);
                    Mouse.OverrideCursor = Cursors.Arrow;
                    if (update.VersionString == mod.Version)
                    {
                        if(showUpdatedDialog)
                        {
                            var box = new HedgeMessageBox(string.Empty, string.Format(HMMResources.STR_MOD_NEWEST, mod.Title), textAlignment: TextAlignment.Center);
                            box.AddButton("OK", () => box.Close());
                            box.ShowDialog();
                        }
                        return;
                    }

                    var dialog = new HedgeMessageBox(string.Format(HMMResources.STR_UI_MOD_UPDATE, mod.Title, update.VersionString)
                        , update.ChangeLog, type: InputType.MarkDown);

                    dialog.AddButton("Close", () => dialog.Close());

                    dialog.AddButton("Update", () =>
                    {
                        var updater = new ModUpdateWindow(update);
                        dialog.Close();

                        updater.DownloadCompleted = () =>
                        {
                            Refresh();
                        };
                        updater.Start();
                    });

                    dialog.ShowDialog();
                });
            }).Start();
        }

        public void SaveModsDB()
        {
            App.Config.Save(App.ConfigPath);
            ModsDatabase.Mods.Clear();
            ModsDatabase.Codes.Clear();

            foreach (var mod in ViewModel.Mods)
            {
                ModsDatabase.Mods.Add(mod as ModInfo);
            }

            CodesDatabase.Codes.ForEach((x) => 
            {
                if(x.Enabled)
                {
                    ModsDatabase.Codes.Add(x.Name);
                }
            });

            ModsDatabase.SaveDB();
            UpdateStatus("Saved mods database");
        }

        public void StartGame()
        {
            Process.Start(new ProcessStartInfo(Path.Combine(App.StartDirectory, App.CurrentGame.ExecuteableName))
            {
                WorkingDirectory = App.StartDirectory
            });

            if (!App.Config.KeepOpen)
                Application.Current.Shutdown(0);

            UpdateStatus($"Starting {App.CurrentGame.GameName}");
        }

        private void SetupWatcher()
        {
            if (!Directory.Exists(App.ModsDbPath))
                return;

            var watcher = new FileSystemWatcher(App.ModsDbPath)
            {
                NotifyFilter = NotifyFilters.DirectoryName
            };

            watcher.Deleted += WatcherEvent;
            watcher.Created += WatcherEvent;
            watcher.EnableRaisingEvents = true;
            ModsWatchers.Add(watcher);

            foreach(var directory in Directory.GetDirectories(App.ModsDbPath))
            {
                var watch = new FileSystemWatcher(directory);
                watch.Changed += WatcherModEvent;
                watch.Deleted += WatcherModEvent;
                watch.Created += WatcherModEvent;
                watch.Renamed += WatcherModEvent;
                watch.EnableRaisingEvents = true;
                ModsWatchers.Add(watch);
            }

            void WatcherModEvent(object sender, FileSystemEventArgs e)
            {
                if(e.Name == "mod.ini")
                {
                    Dispatcher.Invoke(() =>
                    {
                        try { Refresh(); } catch { }
                    });
                }
            }

            void WatcherEvent(object sender, FileSystemEventArgs e)
            {
                if(Directory.Exists(e.FullPath))
                {
                    var watch = new FileSystemWatcher(e.FullPath);
                    watch.Changed += WatcherModEvent;
                    watch.Deleted += WatcherModEvent;
                    watch.Created += WatcherModEvent;
                    watch.Renamed += WatcherModEvent;
                    watch.EnableRaisingEvents = true;
                    ModsWatchers.Add(watch);
                }
                Dispatcher.Invoke(() =>
                {
                    try { Refresh(); } catch { }
                });
            }
        }

        public void CheckForUpdates()
        {
            new Thread(() => 
            {
                if (App.Config.CheckForUpdates)
                {
                    UpdateStatus("Checking for updates");
                    try
                    {
                        var update = App.CheckForUpdates();

                        if (!update.Item1)
                        {
                            UpdateStatus("No updates found");
                            return;
                        }

                        Dispatcher.Invoke(() => 
                        {

                            // http://wasteaguid.info/
                            var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");

                            var info = update.Item2;
                            var dialog = new HedgeMessageBox(info.Name, info.Body, HorizontalAlignment.Right, TextAlignment.Left, InputType.MarkDown);

                            dialog.AddButton("Update", () => 
                            {
                                if (info.Assets.Count > 0)
                                {
                                    var asset = info.Assets[0];
                                    dialog.Close();
                                    var downloader = new DownloadWindow($"Downloading Hedge Mod Manager ({info.TagName})", asset.BrowserDownloadUrl.ToString(), path)
                                    {
                                        DownloadCompleted = () =>
                                        {
                                            Process.Start(path, $"-update \"{App.AppPath}\" {Process.GetCurrentProcess().Id}");
                                            Application.Current.Shutdown();
                                        }
                                    };

                                    downloader.Start();
                                }
                            });
                            
                            dialog.ShowDialog();
                        });

                        UpdateStatus(string.Empty);
                    }
                    catch
                    {
                        UpdateStatus("Failed to check for updates");
                    }
                }
            }).Start();
        }

        protected void CheckForLoaderUpdate()
        {
            if (!App.Config.CheckLoaderUpdates)
                return;

            new Thread(() => 
            {
                UpdateStatus($"Checking for {App.CurrentGame.CustomLoaderName} updates");
                try
                {
                    using (var stream = WebRequest.Create(HMMResources.URL_LOADERS_INI).GetResponse().GetResponseStream())
                    {
                        string loaderVersion = App.GetCodeLoaderVersion(App.CurrentGame);
                        // Check if there is a loader version, if not return
                        if (string.IsNullOrEmpty(loaderVersion))
                            return;

                        var ini = new IniFile(stream);
                        var info = ini[App.CurrentGame.GameName];
                        var version = new Version(loaderVersion);
                        var newVersion = new Version(info["LoaderVersion"]);

                        if (newVersion <= version)
                        {
                            UpdateStatus($"{App.CurrentGame.CustomLoaderName} is up to date");
                            return;
                        }

                        Dispatcher.Invoke(() =>
                        {
                            var dialog = new HedgeMessageBox($"{App.CurrentGame.CustomLoaderName} ({info["LoaderVersion"]})", info["LoaderChangeLog"]);

                            dialog.AddButton("Ignore", () =>
                            {
                                dialog.Close();
                            });

                            dialog.AddButton("Update", () =>
                            {
                                dialog.Close();
                                App.InstallOtherLoader(false);
                                UpdateStatus($"Updated {App.CurrentGame.CustomLoaderName} to {info["LoaderVersion"]}");
                            });

                            dialog.ShowDialog();
                        });
                    }
                }
                catch
                {
                    UpdateStatus($"Failed to check for {App.CurrentGame.CustomLoaderName} updates");
                }
            }).Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusTimer = new Timer((state) => UpdateStatus(string.Empty));
            Refresh();
            CheckForUpdates();
        }

        private void UI_RemoveMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ModsList.SelectedValue as ModInfo;
            if (mod == null)
                return;

            var box = new HedgeMessageBox("WARNING", string.Format(Properties.Resources.STR_UI_DELETEMOD, mod.Title));

            box.AddButton("  Cancel  ", () =>
            {
                box.Close();
            });

            box.AddButton("  Delete  ", () =>
            {
                ModsDatabase.DeleteMod(ModsList.SelectedItem as ModInfo);
                UpdateStatus($"Deleted {((ModInfo)ModsList.SelectedItem).Title}");
                Refresh();
                box.Close();
            });

            box.ShowDialog();
        }

        private void UI_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void UI_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveModsDB();
            Refresh();
        }

        private void UI_SaveAndPlay_Click(object sender, RoutedEventArgs e)
        {
            SaveModsDB();
            Refresh();
            StartGame();
        }

        private void UI_Play_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void UI_CPKREDIR_Click(object sender, RoutedEventArgs e)
        {
            App.InstallCPKREDIR(Path.Combine(App.StartDirectory, App.CurrentGame.ExecuteableName), IsCPKREDIRInstalled);
            RefreshUI();
        }

        private void UI_About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void UI_ModsList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                // Try Install mods from all files
                files.ToList().ForEach(t => ModsDatabase.InstallMod(t));
                Refresh();
            }
        }

        private void UI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ListViewItem)sender;
            var dialog = new AboutModWindow((ModInfo)item.Content);
            dialog.ShowDialog();
        }

        private void UI_OtherLoader_Click(object sender, RoutedEventArgs e)
        {
            App.InstallOtherLoader(true);
            RefreshUI();
        }

        private void UI_Update_Mod(object sender, RoutedEventArgs e)
        {
            var mod = (ModInfo)ModsList.SelectedItem;
            CheckForModUpdates(mod);
            RefreshMods();
        }

        private void UI_Edit_Mod(object sender, RoutedEventArgs e)
        {
            var mod = (ModInfo)ModsList.SelectedItem;
            var window = new EditModWindow(mod);
            if (window.ShowDialog().Value == true)
            {
                mod.Save();
            }
            RefreshMods();
        }

        private void UI_Description_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutModWindow((ModInfo)ModsList.SelectedItem);
            dialog.ShowDialog();
        }

        private void UI_Open_Folder(object sender, RoutedEventArgs e)
        {
            var mod = (ModInfo)ModsList.SelectedItem;
            Process.Start(mod.RootDirectory);
        }

        private void UI_Install_Mod(object sender, RoutedEventArgs e)
        {
            var choice = new OptionsWindow("Install a mod by...", "Installing from a folder", "Installing from an archive", "Making one (for developers!)").Ask();
            if (choice == 0)
            {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog())
                {
                    ModsDatabase.InstallMod(dialog.SelectedFolder);
                }
            }
            else if (choice == 1)
            {
                var dialog = new FileOpenDialog();
                var filters = new COMDLG_FILTERSPEC[3];
                filters[0] = new COMDLG_FILTERSPEC() { pszName = "Zip archive", pszSpec = "*.zip" };
                filters[1] = new COMDLG_FILTERSPEC() { pszName = "7z archive", pszSpec = "*.7z" };
                filters[2] = new COMDLG_FILTERSPEC() { pszName = "Rar archive", pszSpec = "*.rar" };
                dialog.SetFileTypes((uint)filters.Length, filters);
                if (dialog.Show(new WindowInteropHelper(this).Handle) == 0)
                {
                    dialog.GetResult(out var item);
                    item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var path);
                    ModsDatabase.InstallMod(path);
                }
            }
            else if (choice == 2)
            {
                var mod = new ModInfo
                {
                    Title = GenerateModTitle(),
                    Date = DateTime.Now.ToString(),
                    Version = "1.0",
                    Author = Environment.UserName
                };

                mod.IncludeDirs.Add(".");
                var editor = new EditModWindow(mod);
                if (editor.ShowDialog().Value)
                {
                    ModsDatabase.CreateMod(mod, true);
                    RefreshMods();
                }
            }
        }

        protected string GenerateModTitle()
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                var title = $"{App.CurrentGame.GameName} Mod {i}";
                if (!Directory.Exists(Path.Combine(ModsDatabase.RootDirectory, title)))
                    return title;
            }
            return string.Empty;
        }

        public void UpdateStatus(string str)
        {
            Dispatcher.Invoke(() => 
            {
                StatusLbl.Content = str;
            });
            StatusTimer.Change(4000, Timeout.Infinite);
        }

        private void Game_Changed(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBox_GameStatus.SelectedItem != null)
            {
                App.SelectSteamGame((SteamGame)ComboBox_GameStatus.SelectedItem);
                App.ModsDbPath = Path.Combine(App.StartDirectory, "Mods");
                App.ConfigPath = Path.Combine(App.StartDirectory, "cpkredir.ini");
                App.Config = new CPKREDIRConfig(App.ConfigPath);
                foreach (var watcher in ModsWatchers)
                {
                    watcher.Dispose();
                }
                ModsWatchers.Clear();
                SetupWatcher();
                Refresh();
                UpdateStatus($"Changed game to {App.CurrentGame.GameName}");
                CheckForLoaderUpdate();
            }
        }

        private void UI_Download_Codes(object sender, RoutedEventArgs e)
        {
            UpdateStatus($"Downloading codes for {App.CurrentGame.GameName}");
            try
            {
                var downloader = new DownloadWindow($"Downloading codes for {App.CurrentGame.GameName}", App.CurrentGame.CodesURL, CodeLoader.CodesHMMPath)
                {
                    DownloadCompleted = () =>
                    {
                        Refresh();
                        UpdateStatus("Download finished");
                    }
                };
                downloader.Start();
            }
            catch
            {
                UpdateStatus("Download failed");
            }
        }
    }
}
