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
using static HedgeModManager.Lang;
using System.Net;
using System.IO.Compression;

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
        public bool AbortModUpdates = false;

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
            AbortModUpdates = true;
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

            CodesDatabase.Codes.Sort((x, y) => x.Name.CompareTo(y.Name));

            CodesDatabase.Codes.ForEach((x) =>
            {
                if (x.Enabled)
                    CodesList.Items.Insert(0, x);
                else
                    CodesList.Items.Add(x);
            });

            UpdateStatus(string.Format(Localise("StatusUILoadedMods"), ModsDatabase.Mods.Count));
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

            if (App.CurrentGame.HasCustomLoader)
            {
                //Button_CPKREDIR.IsEnabled = App.CurrentGame.SupportsCPKREDIR;
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
                loaders = Localise("CommonUINone");

            ComboBox_GameStatus.SelectedValue = App.CurrentSteamGame;
            Label_MLVersion.Content = $"{Localise("SettingsUILabelLoaders")} {loaders}";
            Button_OtherLoader.Content = Localise(hasOtherModLoader ? "SettingsUIUninstallLoader" : "SettingsUIInstallLoader");
            Button_CPKREDIR.Content = Localise(IsCPKREDIRInstalled ? "SettingsUIUninstallLoader" : "SettingsUIInstallLoader");
        }

        public bool CheckForModUpdates(ModInfo mod, bool showUpdatedDialog = true)
        {
            UpdateStatus(string.Format(Localise("StatusUICheckingModUpdates"), mod.Title));
            ModUpdate.ModUpdateInfo update;
            try
            {
                // Downloads the mod update information
                Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                update = ModUpdate.GetUpdateFromINI(mod);
                if (update == null)
                {
                    Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Arrow);
                    UpdateStatus(string.Empty);
                    return true;
                }
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Arrow);
                UpdateStatus(string.Format(Localise("StatusUIFailedToUpdate"), mod.Title, e.Message));
                return false;
            }

            if (AbortModUpdates)
            {
                Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Arrow);
                return false;
            }


            bool status = true;
            Dispatcher.Invoke(() => 
            {
                try
                {
                    UpdateStatus(string.Empty);
                    Mouse.OverrideCursor = Cursors.Arrow;

                    if (update.VersionString == mod.Version)
                    {
                        if(showUpdatedDialog)
                        {
                            var box = new HedgeMessageBox(string.Empty, string.Format(HMMResources.STR_MOD_NEWEST, mod.Title), textAlignment: TextAlignment.Center);
                            box.AddButton(Localise("CommonUIOK"), () => box.Close());
                            box.ShowDialog();
                        }
                        return;
                    }

                    var dialog = new HedgeMessageBox(string.Format(HMMResources.STR_UI_MOD_UPDATE, mod.Title, update.VersionString)
                        , update.ChangeLog, type: InputType.MarkDown);

                    dialog.AddButton(Localise("CommonUIClose"), () => dialog.Close());

                    dialog.AddButton(Localise("CommonUIUpdate"), () =>
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
                }
                catch (Exception e)
                {
                    // Mark as failed
                    status = false;
                }

            });
            return status;
        }

        public void CheckAllModsUpdates()
        {
            AbortModUpdates = false;
            int completedCount = 0;
            int failedCount = 0;
            var mods = ModsDatabase.Mods.Where(t => t.HasUpdates).ToList();
            // Filter all mods with updates
            foreach (var mod in mods)
            {
                bool status = CheckForModUpdates(mod, false);
                if (status)
                    ++completedCount;
                else
                    ++failedCount;
                if (AbortModUpdates)
                {
                    AbortModUpdates = false;
                    UpdateStatus(Localise("StatusUIModUpdateCheckAbort"));
                    return;
                }
            }
            UpdateStatus(string.Format(Localise("StatusUIModUpdateCheckFinish"), completedCount, failedCount));
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
        }

        public void StartGame()
        {
            Process.Start(new ProcessStartInfo(Path.Combine(App.StartDirectory, App.CurrentGame.ExecuteableName))
            {
                WorkingDirectory = App.StartDirectory
            });

            if (!App.Config.KeepOpen)
                Application.Current.Shutdown(0);

            UpdateStatus(string.Format(Localise("StatusUIStartingGame"), App.CurrentGame.GameName));
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
                CheckForManagerUpdates();
                if (App.Config.CheckForModUpdates)
                    CheckAllModsUpdates();
            }).Start();
        }

        public void CheckForManagerUpdates()
        {
            if (App.Config.CheckForUpdates)
            {
                UpdateStatus(Localise("StatusUICheckingForUpdates"));
                try
                {
                    var update = App.CheckForUpdates();

                    if (!update.Item1)
                    {
                        UpdateStatus(Localise("StatusUINoUpdatesFound"));
                        return;
                    }

                    Dispatcher.Invoke(() => 
                    {

                        // http://wasteaguid.info/
                        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");

                        var info = update.Item2;
                        var dialog = new HedgeMessageBox(info.Name, info.Body, HorizontalAlignment.Right, TextAlignment.Left, InputType.MarkDown);

                        dialog.AddButton(Localise("CommonUIUpdate"), () => 
                        {
                            if (info.Assets.Count > 0)
                            {
                                var asset = info.Assets[0];
                                dialog.Close();
                                var downloader = new DownloadWindow($"Downloading Hedge Mod Manager ({info.TagName})", asset.BrowserDownloadUrl.ToString(), path)
                                {
                                    DownloadCompleted = () =>
                                    {
                                        // Extract zip for compatibility for 6.x
                                        if (asset.ContentType == "application/x-zip-compressed")
                                        {
                                            // Stre old path pointing to the zip
                                            string oldPath = path;
                                            // Generate new path
                                            path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");
                                            using (var zip = ZipFile.Open(oldPath, ZipArchiveMode.Read))
                                            {
                                                var entry = zip.Entries.FirstOrDefault(t => t.Name.Contains(".exe"));
                                                entry.ExtractToFile(path);
                                            }
                                            File.Delete(oldPath);
                                        }

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
                    UpdateStatus(Localise("StatusUIFailedToCheckUpdates"));
                }
            }
        }

        protected void CheckForLoaderUpdate()
        {
            if (!App.Config.CheckLoaderUpdates)
                return;

            new Thread(() => 
            {
                UpdateStatus(string.Format(Localise("StatusUICheckingForLoaderUpdate"), App.CurrentGame.CustomLoaderName));
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
                            UpdateStatus(string.Format(Localise("StatusUILoaderUpToDate"), App.CurrentGame.CustomLoaderName));
                            return;
                        }

                        Dispatcher.Invoke(() =>
                        {
                            var dialog = new HedgeMessageBox($"{App.CurrentGame.CustomLoaderName} ({info["LoaderVersion"]})", info["LoaderChangelog"].Replace("\\n", "\n"));

                            dialog.AddButton(Localise("CommonUIIgnore"), () =>
                            {
                                dialog.Close();
                            });

                            dialog.AddButton(Localise("CommonUIUpdate"), () =>
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
                    UpdateStatus(string.Format(Localise("StatusUIFailedLoaderUpdateCheck"), App.CurrentGame.CustomLoaderName));
                }
            }).Start();
        }

        public void ShowMissingOtherLoaderWarning()
        {
            if (!App.CurrentGame.HasCustomLoader)
                return;
            bool loaderInstalled = File.Exists(Path.Combine(App.StartDirectory, $"d3d{App.CurrentGame.DirectXVersion}.dll"));
            if (loaderInstalled)
                return;
            Dispatcher.Invoke(() =>
            {
                var dialog = new HedgeMessageBox(Localise("MainUIMissingLoaderHeader"), string.Format(Localise("MainUIMissingLoaderDesc"), App.CurrentGame.GameName));

                dialog.AddButton(Localise("CommonUINo"), () =>
                {
                    dialog.Close();
                });

                dialog.AddButton(Localise("CommonUIYes"), () =>
                {
                    dialog.Close();
                    App.InstallOtherLoader(false);
                    UpdateStatus(string.Format(Localise("StatusUIInstalledLoader"), App.CurrentGame.CustomLoaderName));
                });

                dialog.ShowDialog();
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusTimer = new Timer((state) => UpdateStatus(string.Empty));
            Refresh();
            CheckForUpdates();
            ComboBox_Languages.SelectedItem = App.SupportedCultures.FirstOrDefault(t => t.Value == App.GetClosestCulture(RegistryConfig.UILanguage));
        }

        private void UI_RemoveMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ModsList.SelectedValue as ModInfo;
            if (mod == null)
                return;

            var box = new HedgeMessageBox(Localise("CommonUIWarning"), string.Format(Properties.Resources.STR_UI_DELETEMOD, mod.Title));

            box.AddButton(Localise("CommonUICancel"), () =>
            {
                box.Close();
            });

            box.AddButton(Localise("CommonUIDelete"), () =>
            {
                ModsDatabase.DeleteMod(ModsList.SelectedItem as ModInfo);
                UpdateStatus(string.Format(Localise("StatusUIDeletedMod"), ((ModInfo)ModsList.SelectedItem).Title));
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
            ShowMissingOtherLoaderWarning();
            SaveModsDB();
            Refresh();
            UpdateStatus(Localise("StatusUIModsDBSaved"));
        }

        private void UI_SaveAndPlay_Click(object sender, RoutedEventArgs e)
        {
            ShowMissingOtherLoaderWarning();
            SaveModsDB();
            Refresh();
            UpdateStatus(Localise("StatusUIModsDBSaved"));
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
            AbortModUpdates = false;
            var mod = (ModInfo)ModsList.SelectedItem;
            new Thread(() =>
            {
                CheckForModUpdates(mod);
                Dispatcher.Invoke(() => RefreshMods());
            }).Start();
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
            var choice = new OptionsWindow(Localise("MainUIInstallFormHeader"), Localise("MainUIInstallFormOptionDir"), Localise("MainUIInstallFormOptionArc"), Localise("MainUIInstallFormOptionNew")).Ask();
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

                App.ConfigPath = Path.Combine(App.StartDirectory, "cpkredir.ini");
                App.Config = new CPKREDIRConfig(App.ConfigPath);
                App.ModsDbPath = Path.Combine(App.StartDirectory, Path.GetDirectoryName(App.Config.ModsDbIni));

                // Remove old patch
                string exePath = Path.Combine(App.StartDirectory, App.CurrentGame.ExecuteableName);
                if (App.IsCPKREDIRInstalled(exePath))
                    App.InstallCPKREDIR(exePath, false);

                foreach (var watcher in ModsWatchers)
                {
                    watcher.Dispose();
                }
                ModsWatchers.Clear();
                SetupWatcher();
                Refresh();
                UpdateStatus(string.Format(Localise("StatusUIGameChange"), App.CurrentGame.GameName));
                CheckForLoaderUpdate();
            }
        }

        private void UI_Download_Codes(object sender, RoutedEventArgs e)
        {
            UpdateStatus(string.Format(Localise("StatusUIDownloadingCodes"), App.CurrentGame.GameName));
            try
            {
                var downloader = new DownloadWindow($"Downloading codes for {App.CurrentGame.GameName}", App.CurrentGame.CodesURL, CodeLoader.CodesHMMPath)
                {
                    DownloadCompleted = () =>
                    {
                        Refresh();
                        UpdateStatus(Localise("StatusUIDownloadFinished"));
                    }
                };
                downloader.Start();
            }
            catch
            {
                UpdateStatus(Localise("StatusUIDownloadFailed"));
            }
        }

        private void UI_OpenMods_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(App.ModsDbPath);
        }

        private void UI_ChangeDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Title = Localise("MainUISelectModsDBTitle");
            if (dialog.ShowDialog())
            {
                App.ModsDbPath = dialog.SelectedFolder;
                ViewModel.CPKREDIR.ModsDbIni = Path.Combine(App.ModsDbPath, "ModsDB.ini");
                if (ViewModel.CPKREDIR.ModsDbIni.StartsWith(App.StartDirectory))
                    ViewModel.CPKREDIR.ModsDbIni = ViewModel.CPKREDIR.ModsDbIni.Substring(App.StartDirectory.Length + 1);
                ViewModel.CPKREDIR.Save(Path.Combine(App.StartDirectory, "cpkredir.ini"));
                Refresh();
                UpdateStatus(Localise("StatusUIModsDBLocationChanged"));
            }
        }

        private void ComboBox_Languages_Changed(object sender, SelectionChangedEventArgs e)
        {
            var culture = ((KeyValuePair<string, string>)ComboBox_Languages.SelectedItem).Value;
            RegistryConfig.UILanguage = culture;
            RegistryConfig.Save();
            App.LoadLanaguage(culture);
            RefreshUI();
        }
    }
}
