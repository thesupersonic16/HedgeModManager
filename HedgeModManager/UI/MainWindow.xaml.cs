using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using HedgeModManager.UI;
using System.Collections.ObjectModel;
using System.Net;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MenuItem = System.Windows.Controls.MenuItem;
using Timer = System.Threading.Timer;
using HMMResources = HedgeModManager.Properties.Resources;
using static HedgeModManager.Lang;
using HedgeModManager.Languages;
using Newtonsoft.Json;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsCPKREDIRInstalled = false;
        public static ModsDB ModsDatabase;

        public static CodeFile CodesDatabase
        {
            get => ModsDatabase?.CodesDatabase;
            set => ModsDatabase.CodesDatabase = value;
        }

        public static List<FileSystemWatcher> ModsWatchers = new List<FileSystemWatcher>();
        public MainWindowViewModel ViewModel = new MainWindowViewModel();
        public List<string> CheckedModUpdates = new List<string>();
        public bool PauseModUpdates = false;
        public ModProfile SelectedModProfile = null;

        protected Timer StatusTimer;

        private bool CodesOutdated = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            RefreshMods();
            RefreshUI();
        }

        public void RefreshProfiles()
        {
            HedgeApp.ModProfiles.Clear();
            try
            {
                string profilePath = Path.Combine(HedgeApp.StartDirectory, "profiles.json");
                if (File.Exists(profilePath))
                    HedgeApp.ModProfiles = JsonConvert.DeserializeObject<List<ModProfile>>(File.ReadAllText(profilePath));

                HedgeApp.ModProfiles ??= new List<ModProfile>();

                // Remove profiles that don't exist
                HedgeApp.ModProfiles.RemoveAll(profile =>
                    !File.Exists(Path.Combine(HedgeApp.ModsDbPath, profile.ModDBPath)));
                
                // Create new profile set if needed
                if (HedgeApp.ModProfiles.Count == 0)
                    HedgeApp.ModProfiles.Add(new ModProfile("Default", "ModsDB.ini"));

                SelectedModProfile = HedgeApp.ModProfiles.FirstOrDefault(t => t.Name == HedgeApp.Config.ModProfile)
                    ?? HedgeApp.ModProfiles.First();
            }
            catch (Exception e)
            {
                new ExceptionWindow(e).ShowDialog();
                HedgeApp.ModProfiles ??= new List<ModProfile>();
                if (HedgeApp.ModProfiles.Count == 0)
                    HedgeApp.ModProfiles.Add(new ModProfile("Default", "ModsDB.ini"));
                SelectedModProfile = HedgeApp.ModProfiles.First();
            }
        }

        public void LoadDatabase()
        {
            ModsDatabase = new ModsDB(HedgeApp.ModsDbPath, SelectedModProfile.ModDBPath);
            if (!Directory.Exists(HedgeApp.ModsDbPath))
            {
                Application.Current?.MainWindow?.Hide();
                var box = new HedgeMessageBox("No Mods Found", Properties.Resources.STR_UI_NO_MODS);

                box.AddButton("Yes", () =>
                {
                    ModsDatabase.SetupFirstTime();
                    box.Close();
                });

                box.AddButton("No", () => Environment.Exit(0));

                box.ShowDialog();
                Application.Current?.MainWindow?.Show();
            }
        }

        public void RefreshMods()
        {
            PauseModUpdates = true;
            CodesList.Items.Clear();

            LoadDatabase();
            ModsDatabase.DetectMods();
            ModsDatabase.GetEnabledMods();
            ModsDatabase.Mods.Sort((x, y) => x.Title.CompareTo(y.Title));

            CodesDatabase = CodeFile.FromFiles(CodeProvider.CodesTextPath, CodeProvider.ExtraCodesTextPath);
            ModsDatabase.Codes.ForEach((x) =>
            {
                var code = CodesDatabase.Codes.Find((y) => { return y.Name == x; });
                if (code != null)
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

            UpdateStatus(Localise("StatusUILoadedMods", ModsDatabase.Mods.Count));
            CheckCodeCompatibility();
            var invalid = ModsDatabase.GetInvalidMods();
            if (invalid.Count > 0)
            {
                var messageBuilder = new StringBuilder();

                foreach (var mod in invalid)
                {
                    messageBuilder.AppendLine($"· {mod.Title}");
                }

                var box = new HedgeMessageBox(Localise("ModsUIInvalidIncludeDirs"), messageBuilder.ToString(), textAlignment: TextAlignment.Left);
                box.AddButton(Localise("CommonUIOK"), () =>
                {
                    foreach (var mod in invalid)
                    {
                        mod.FixIncludeDirectories();
                        mod.Save();
                    }

                    box.Close();
                });

                box.AddButton(Localise("CommonUICancel"), box.Close);
                box.ShowDialog();
            }

            PauseModUpdates = false;
        }

        public void RefreshUI()
        {
            // Re-arrange the mods
            for (int i = 0; i < ModsDatabase.ActiveMods.Count; i++)
            {
                var mod = ModsDatabase.GetModFromActiveGUID(ModsDatabase.ActiveMods[i]);

                if (mod != null)
                {
                    ModsDatabase.Mods.Remove(mod);
                    ModsDatabase.Mods.Insert(i, mod);
                }
            }

            for (int i = 0; i < ModsDatabase.Mods.Count; i++)
            {
                var mod = ModsDatabase.Mods[i];
                if (mod.Favorite && !mod.Enabled)
                {
                    ModsDatabase.Mods.Remove(mod);
                    ModsDatabase.Mods.Insert(ModsDatabase.ActiveMods.Count, mod);
                }
            }

            // Sets the DataContext for all the Components
            ViewModel = new MainWindowViewModel
            {
                CPKREDIR = HedgeApp.Config,
                ModsDB = ModsDatabase,
                Games = HedgeApp.SteamGames,
                Mods = new ObservableCollection<ModInfo>(ModsDatabase.Mods),
                Profiles = new ObservableCollection<ModProfile>(HedgeApp.ModProfiles),
                SelectedModProfile = SelectedModProfile
            };
            DataContext = ViewModel;

            Title = $"{HedgeApp.ProgramName} ({HedgeApp.VersionString}) - {HedgeApp.CurrentGame.GameName} ({SelectedModProfile?.Name})";

            if (HedgeApp.CurrentGame.HasCustomLoader)
            {
                Button_OtherLoader.IsEnabled = HedgeApp.CurrentGame.HasCustomLoader;
                Button_DownloadCodes.IsEnabled = HedgeApp.CurrentGame.HasCustomLoader && !string.IsNullOrEmpty(HedgeApp.CurrentGame.CodesURL);
            }

            var exeDir = HedgeApp.StartDirectory;
            bool hasOtherModLoader = File.Exists(Path.Combine(exeDir, HedgeApp.CurrentGame.CustomLoaderFileName));
            IsCPKREDIRInstalled = HedgeApp.CurrentGame.SupportsCPKREDIR ? HedgeApp.IsCPKREDIRInstalled(Path.Combine(exeDir, HedgeApp.CurrentGame.ExecuteableName)) : hasOtherModLoader;
            string loaders = (IsCPKREDIRInstalled && HedgeApp.CurrentGame.SupportsCPKREDIR ? HedgeApp.GetCPKREDIRVersionString() : "");

            if (hasOtherModLoader)
            {
                if (string.IsNullOrEmpty(loaders))
                    loaders = $"{HedgeApp.CurrentGame.CustomLoaderName}";
                else
                    loaders += $" & {HedgeApp.CurrentGame.CustomLoaderName}";
            }

            if (string.IsNullOrEmpty(loaders))
                loaders = Localise("CommonUINone");

            ComboBox_GameStatus.SelectedValue = HedgeApp.CurrentSteamGame;
            Button_OtherLoader.Content = Localise(hasOtherModLoader ? "SettingsUIUninstallLoader" : "SettingsUIInstallLoader");
        }

        private void UI_CodesTab_Click(object sender, RoutedEventArgs e)
        {
            // Display update alert.
            UpdateStatus(Localise(CodesOutdated ? "StatusUICodeUpdatesAvailable" : "StatusUINoCodeUpdatesFound"));
        }

        public async Task CheckForCodeUpdates()
        {
            if (!File.Exists(CodeProvider.CodesTextPath))
                return;

            try
            {
                // Codes from disk.
                string localCodes = File.ReadAllText(CodeProvider.CodesTextPath);
                string repoCodes = await HedgeApp.HttpClient.GetStringAsync(HedgeApp.CurrentGame.CodesURL);

                if (localCodes == repoCodes)
                {
                    CodesOutdated = false;

                    // Codes are the same, so use default text.
                    Button_DownloadCodes.Content = Localise("CodesUIDownload");
                }
                else
                {
                    CodesOutdated = true;

                    // Codes are different, report update possibility.
                    Button_DownloadCodes.Content = Localise("CodesUIUpdate");
                }
            }
            catch (WebException) { /* do nothing for web exceptions */ }
        }

        public async Task<bool> CheckForModUpdatesAsync(ModInfo mod, bool showUpdatedDialog = true)
        {
            // Cancel update check if URL is blocked
            if (HedgeApp.NetworkConfiguration.URLBlockList.Any(t => mod.UpdateServer.ToLowerInvariant().Contains(t)))
                return false;

            UpdateStatus(string.Format(Localise("StatusUICheckingModUpdates"), mod.Title));
            ModUpdate.ModUpdateInfo update;
            try
            {
                // Downloads the mod update information
                Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                update = await ModUpdate.GetUpdateFromINIAsync(mod);
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
                UpdateStatus(Localise("StatusUIFailedToUpdate", mod.Title, e.Message));
                return false;
            }

            if (PauseModUpdates)
            {
                Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Arrow);
                return false;
            }

            bool status = true;
            await Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    UpdateStatus(string.Empty);
                    Mouse.OverrideCursor = Cursors.Arrow;

                    if (update.VersionString == mod.Version)
                    {
                        if (showUpdatedDialog)
                        {
                            var box = new HedgeMessageBox(string.Empty, string.Format(HMMResources.STR_MOD_NEWEST, mod.Title));
                            box.AddButton(Localise("CommonUIOK"), () => box.Close());
                            box.ShowDialog();
                        }
                        return;
                    }

                    var dialog = new HedgeMessageBox(string.Format(HMMResources.STR_UI_MOD_UPDATE, mod.Title, update.VersionString)
                        , update.ChangeLog, type: InputType.MarkDown);

                    dialog.AddButton(Localise("CommonUIUpdate"), () =>
                    {
                        var updater = new ModUpdateWindow(update);
                        dialog.Close();

                        updater.DownloadCompleted = Refresh;
                        updater.Start();
                    });

                    dialog.AddButton(Localise("CommonUIClose"), () => dialog.Close());

                    dialog.ShowDialog();
                }
                catch (Exception)
                {
                    // Mark as failed
                    status = false;
                }

            });
            return status;
        }

        public async Task CheckAllModsUpdatesAsync()
        {
            PauseModUpdates = false;
            int completedCount = 0;
            int failedCount = 0;
            var mods = ModsDatabase.Mods.Where(t => t.HasUpdates).ToList();
            // Filter all mods with updates
            foreach (var mod in mods)
            {
                if (CheckedModUpdates.Any(t => t == mod.RootDirectory))
                    continue;
                bool status = await CheckForModUpdatesAsync(mod, false);
                if (status)
                    ++completedCount;
                else
                    ++failedCount;
                CheckedModUpdates.Add(mod.RootDirectory);
                if (PauseModUpdates)
                {
                    while (PauseModUpdates)
                        await Task.Delay(200);
                    await CheckAllModsUpdatesAsync();
                    return;
                }
            }
            CheckedModUpdates.Clear();

            // Language Workaround
            string g_completed = completedCount == 1 ? Localise("StatusUIUpdateCompletedSingular") : Localise("StatusUIUpdateCompletedPlural");
            string g_failed = failedCount == 1 ? Localise("StatusUIUpdateFailedSingular") : Localise("StatusUIUpdateFailedPlural");
            string text = string.Format(Localise("StatusUIModUpdateCheckFinish"), completedCount, failedCount, g_completed, g_failed);
            UpdateStatus(text);
        }

        public async Task SaveModsDB()
        {
            HedgeApp.Config.ModsDbIni = Path.Combine(HedgeApp.ModsDbPath, SelectedModProfile.ModDBPath);
            HedgeApp.Config.Save(HedgeApp.ConfigPath);
            ModsDatabase.Mods.Clear();
            ModsDatabase.Codes.Clear();

            foreach (var mod in ViewModel.Mods)
            {
                ModsDatabase.Mods.Add(mod);
            }

            foreach (var code in CodesDatabase.Codes)
            {
                if (code.Enabled)
                {
                    ModsDatabase.Codes.Add(code.Name);
                }
            }

            await ModsDatabase.SaveDB();
        }

        public Task StartGame()
        {
            Process.Start(new ProcessStartInfo(Path.Combine(HedgeApp.StartDirectory, HedgeApp.CurrentGame.ExecuteableName))
            {
                WorkingDirectory = HedgeApp.StartDirectory
            });

            if (!HedgeApp.Config.KeepOpen)
                Dispatcher.Invoke(() => Close());

            UpdateStatus(string.Format(Localise("StatusUIStartingGame"), HedgeApp.CurrentGame.GameName));
            return Task.CompletedTask;
        }

        private void SetupWatcher()
        {
            if (!Directory.Exists(HedgeApp.ModsDbPath))
                return;

            var watcher = new FileSystemWatcher(HedgeApp.ModsDbPath)
            {
                NotifyFilter = NotifyFilters.DirectoryName
            };

            watcher.Deleted += WatcherEvent;
            watcher.Created += WatcherEvent;
            watcher.EnableRaisingEvents = true;
            ModsWatchers.Add(watcher);

            foreach (var directory in Directory.GetDirectories(HedgeApp.ModsDbPath))
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
                if (e.Name == "mod.ini")
                {
                    Dispatcher.Invoke(() =>
                    {
                        try { Refresh(); } catch { }
                    });
                }
            }

            void WatcherEvent(object sender, FileSystemEventArgs e)
            {
                if (Directory.Exists(e.FullPath))
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

        private void ResetWatchers()
        {
            foreach (var watcher in ModsWatchers)
            {
                watcher.Dispose();
            }
            ModsWatchers.Clear();
            SetupWatcher();
        }

        public async Task CheckForUpdatesAsync()
        {
            await CheckForManagerUpdatesAsync();
            if (HedgeApp.Config.CheckForModUpdates)
                await CheckAllModsUpdatesAsync();
            await CheckForCodeUpdates();
        }

        public async Task CheckForManagerUpdatesAsync()
        {
            if (!HedgeApp.Config.CheckForUpdates)
                return;

            UpdateStatus(Localise("StatusUICheckingForUpdates"));
            try
            {
                var update = await HedgeApp.CheckForUpdatesAsync();

                if (!update.Item1)
                {
                    UpdateStatus(Localise("StatusUINoUpdatesFound"));
                    return;
                }

                await Dispatcher.InvokeAsync(() =>
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
                                    // TODO: literally extracting a ZIP on the UI thread I couldn't make this up if I *tried*

                                    // Extract zip for compatibility for 6.x
                                    if (asset.ContentType == "application/x-zip-compressed" || asset.ContentType == "application/zip")
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

                                    Process.Start(path, $"-update \"{HedgeApp.AppPath}\" {Process.GetCurrentProcess().Id}");
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

        protected async Task CheckForLoaderUpdateAsync()
        {
            if (!HedgeApp.Config.CheckLoaderUpdates)
                return;

            await Task.Yield();

            UpdateStatus(string.Format(Localise("StatusUICheckingForLoaderUpdate"), HedgeApp.CurrentGame.CustomLoaderName));
            try
            {
                using (var stream = await HedgeApp.HttpClient.GetStreamAsync(HMMResources.URL_LOADERS_INI))
                {
                    var loaderInfo = HedgeApp.GetCodeLoaderInfo(HedgeApp.CurrentGame);
                    // Check if there is a loader version, if not return
                    if (loaderInfo.LoaderVersion == null)
                        return;

                    var ini = new IniFile(stream);
                    var info = ini[HedgeApp.CurrentGame.GameName];
                    var newVersion = new Version(info["LoaderVersion"]);

                    if (newVersion <= loaderInfo.LoaderVersion)
                    {
                        UpdateStatus(string.Format(Localise("StatusUILoaderUpToDate"), HedgeApp.CurrentGame.CustomLoaderName));
                        return;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        var dialog = new HedgeMessageBox($"{HedgeApp.CurrentGame.CustomLoaderName} ({info["LoaderVersion"]})", info["LoaderChangelog"].Replace("\\n", "\n"), textAlignment: TextAlignment.Left);

                        dialog.AddButton(Localise("CommonUIIgnore"), () =>
                        {
                            dialog.Close();
                        });

                        dialog.AddButton(Localise("CommonUIUpdate"), () =>
                        {
                            var dialog = new HedgeMessageBox($"{HedgeApp.CurrentGame.CustomLoaderName} ({info["LoaderVersion"]})", info["LoaderChangelog"].Replace("\\n", "\n"), textAlignment: TextAlignment.Left);

                            dialog.AddButton(Localise("CommonUIUpdate"), () =>
                            {
                                dialog.Close();
                                HedgeApp.InstallOtherLoader(false);
                                UpdateStatus($"Updated {HedgeApp.CurrentGame.CustomLoaderName} to {info["LoaderVersion"]}");
                            });

                            dialog.AddButton(Localise("CommonUIIgnore"), () =>
                            {
                                dialog.Close();
                            });

                            dialog.ShowDialog();
                        });

                        dialog.ShowDialog();
                    });
                }
            }
            catch
            {
                UpdateStatus(string.Format(Localise("StatusUIFailedLoaderUpdateCheck"), HedgeApp.CurrentGame.CustomLoaderName));
            }
        }

        protected void CheckCodeCompatibility()
        {
            var info = HedgeApp.GetCodeLoaderInfo(HedgeApp.CurrentGame);
            if (CodesDatabase.Codes.Count == 0)
                return;

            if (CodesDatabase.FileVersion >= info.MinCodeVersion && CodesDatabase.FileVersion <= info.MaxCodeVersion)
                return;

            var dialog = new HedgeMessageBox(Localise("CommonUIWarning"), Localise("CodesUIVersionIncompatible"));
            dialog.AddButton(Localise("CommonUIUpdate"), () =>
            {
                HedgeApp.InstallOtherLoader(false);
                UI_Download_Codes(null, null);
                dialog.Close();
            });
            dialog.AddButton(Localise("CommonUICancel"), dialog.Close);
            dialog.ShowDialog();
        }

        public void EnableSaveRedirIfUsed()
        {
            if (HedgeApp.Config.EnableFallbackSaveRedirection)
            {
                HedgeApp.Config.EnableSaveFileRedirection = true;
                return;
            }

            HedgeApp.Config.EnableSaveFileRedirection = false;
            foreach (var mod in ModsDatabase.Mods)
            {
                if (mod.SupportsSave && mod.Enabled)
                {
                    HedgeApp.Config.EnableSaveFileRedirection = true;
                    break;
                }
            }
        }

        public void ShowMissingOtherLoaderWarning()
        {
            if (!HedgeApp.CurrentGame.HasCustomLoader)
                return;
            bool loaderInstalled = File.Exists(Path.Combine(HedgeApp.StartDirectory, HedgeApp.CurrentGame.CustomLoaderFileName));
            if (loaderInstalled)
                return;

            Dispatcher.Invoke(() =>
            {
                var dialog = new HedgeMessageBox(Localise("MainUIMissingLoaderHeader"), string.Format(Localise("MainUIMissingLoaderDesc"), HedgeApp.CurrentGame.GameName));

                dialog.AddButton(Localise("CommonUIYes"), () =>
                {
                    dialog.Close();
                    HedgeApp.InstallOtherLoader(false);
                    UpdateStatus(string.Format(Localise("StatusUIInstalledLoader"), HedgeApp.CurrentGame.CustomLoaderName));
                });

                dialog.AddButton(Localise("CommonUINo"), () =>
                {
                    dialog.Close();
                });

                dialog.ShowDialog();
            });
        }

        public async Task SaveConfig(bool startGame = false)
        {
            string profilePath = Path.Combine(HedgeApp.StartDirectory, "profiles.json");
            File.WriteAllText(profilePath, JsonConvert.SerializeObject(HedgeApp.ModProfiles));
            ShowMissingOtherLoaderWarning();
            EnableSaveRedirIfUsed();
            try
            {
                await SaveModsDB();
                Refresh();
                UpdateStatus(Localise("StatusUIModsDBSaved"));
                if (startGame)
                    await StartGame();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => new ExceptionWindow(ex).ShowDialog());
            }
        }

        public bool CheckModDepends()
        {
            bool abort = false;
            var report = ModsDatabase.ResolveDepends();

            if (report.HasErrors)
            {
                var box = new HedgeMessageBox(Localise("MainUIMissingDependsHeader"), report.BuildMarkdown(), textAlignment: TextAlignment.Left, type: InputType.MarkDown);
                box.AddButton(Localise("CommonUIIgnore"), () => box.Close());
                box.AddButton(Localise("CommonUICancel"), () =>
                {
                    box.Close();
                    abort = true;
                });
                box.ShowDialog();
            }

            return !abort;
        }

        public bool CheckDepends()
        {
            bool abort = false;

            if (!abort)
                abort = DependsHandler.AskToInstallRuntime(Games.SonicGenerations.AppID, DependTypes.VS2019x86);
            if (!abort)
                abort = DependsHandler.AskToInstallRuntime(Games.SonicLostWorld.AppID, DependTypes.VS2019x86);
            if (!abort)
                abort = DependsHandler.AskToInstallRuntime(Games.SonicForces.AppID, DependTypes.VS2019x64);
            if (!abort)
                abort = DependsHandler.AskToInstallRuntime(Games.PuyoPuyoTetris2.AppID, DependTypes.VS2019x64);
            return !abort;
        }

        public bool CheckDepend(string id, string filePath, string dependName, string downloadURL, string fileName)
        {
            bool abort = false;
            if (HedgeApp.CurrentGame.AppID == id && !File.Exists(Path.Combine(Environment.GetEnvironmentVariable("windir"), filePath)))
            {
                var dialog = new HedgeMessageBox(Localise("MainUIRuntimeMissingTitle"), string.Format(Localise("MainUIRuntimeMissingMsg"), HedgeApp.CurrentGame.GameName, dependName));

                dialog.AddButton(Localise("CommonUIYes"), () =>
                {
                    DownloadWindow window = new DownloadWindow($"Downloading {dependName}...", downloadURL, fileName);
                    window.Start();
                    if (File.Exists(fileName))
                    {
                        // For VC++
                        Process.Start(fileName, "/passive /norestart").WaitForExit(30000);
                        File.Delete(fileName);
                    }
                    dialog.Close();
                });

                dialog.AddButton(Localise("CommonUINo"), () =>
                {
                    abort = true;
                    dialog.Close();
                });

                dialog.ShowDialog();
            }
            return abort;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusTimer = new Timer((state) => UpdateStatus(string.Empty));

            // Update CPKREDIR if needed
            if (HedgeApp.CurrentGame.SupportsCPKREDIR)
                HedgeApp.UpdateCPKREDIR();

            RefreshProfiles();
            Refresh();
            await CheckForUpdatesAsync();
        }

        private void UI_RemoveMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ModsList.SelectedValue as ModInfo;
            if (mod == null)
                return;

            var box = new HedgeMessageBox(Localise("CommonUIWarning"), string.Format(Localise("DialogUIDeleteMod"), mod.Title));

            box.AddButton(Localise("CommonUIDelete"), () =>
            {
                foreach (var watcher in ModsWatchers)
                {
                    watcher.Dispose();
                }
                ModsWatchers.Clear();
                ModsDatabase.DeleteMod(ViewModel.SelectedMod);
                UpdateStatus(string.Format(Localise("StatusUIDeletedMod"), ViewModel.SelectedMod.Title));
                Refresh();
                SetupWatcher();
                box.Close();
            });

            box.AddButton(Localise("CommonUICancel"), () =>
            {
                box.Close();
            });

            box.ShowDialog();
        }

        private void UI_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void UI_Save_Click(object sender, RoutedEventArgs e)
        {
            if (CheckModDepends())
                await SaveConfig();
        }

        private async void UI_SaveAndPlay_Click(object sender, RoutedEventArgs e)
        {
            await SaveConfig(CheckDepends() && CheckModDepends());
        }

        private void UI_Play_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDepends())
                StartGame();
        }

        private void UI_About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void UI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ListViewItem)sender;
            var dialog = new AboutModWindow((ModInfo)item.Content);
            dialog.ShowDialog();
        }

        private void UI_OtherLoader_Click(object sender, RoutedEventArgs e)
        {
            HedgeApp.InstallOtherLoader(true);
            RefreshUI();
        }

        private async void UI_Update_Mod(object sender, RoutedEventArgs e)
        {
            PauseModUpdates = false;
            if (await CheckForModUpdatesAsync(ViewModel.SelectedMod).ConfigureAwait(false))
                Dispatcher.Invoke(RefreshMods);
        }

        private void UI_Edit_Mod(object sender, RoutedEventArgs e)
        {
            var mod = ViewModel.SelectedMod;
            var window = new EditModWindow(mod);
            if (window.ShowDialog().Value)
            {
                mod.Save();
            }
            RefreshMods();
        }

        private void UI_Description_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutModWindow(ViewModel.SelectedMod);
            dialog.ShowDialog();
        }

        private void UI_Favorite_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedMod.Favorite = !ViewModel.SelectedMod.Favorite;
            RefreshUI();
        }

        private void UI_Open_Folder(object sender, RoutedEventArgs e)
        {
            Process.Start(ViewModel.SelectedMod.RootDirectory);
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
                    var modDir = HedgeApp.CurrentGame == Games.PuyoPuyoTetris2 ? "raw" : "disk";
                    ModsDatabase.CreateMod(mod, modDir, true);
                    RefreshMods();
                }
            }
        }

        protected string GenerateModTitle()
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                var title = $"{HedgeApp.CurrentGame.GameName} Mod {i}";
                if (!Directory.Exists(Path.Combine(ModsDatabase.RootDirectory, title)))
                    return title;
            }
            return string.Empty;
        }

        public void UpdateStatus(string str)
        {
            Dispatcher.Invoke(() => StatusLbl.Text = str);
            StatusTimer.Change(4000, Timeout.Infinite);
        }

        private async void Game_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_GameStatus.SelectedItem != null && ComboBox_GameStatus.SelectedItem != HedgeApp.CurrentSteamGame)
            {
                HedgeApp.SelectSteamGame((SteamGame)ComboBox_GameStatus.SelectedItem);

                if (HedgeApp.CurrentGame.SupportsCPKREDIR)
                {
                    // Remove old patch
                    string exePath = Path.Combine(HedgeApp.StartDirectory, HedgeApp.CurrentGame.ExecuteableName);
                    if (HedgeApp.IsCPKREDIRInstalled(exePath))
                        HedgeApp.InstallCPKREDIR(exePath, false);

                    // Update CPKREDIR if needed
                    HedgeApp.UpdateCPKREDIR();
                }

                ResetWatchers();
                RefreshProfiles();
                Refresh();
                UpdateStatus(string.Format(Localise("StatusUIGameChange"), HedgeApp.CurrentGame.GameName));
                CheckForLoaderUpdateAsync();

                // Schedule checking for code updates if available.
                if (Button_DownloadCodes.IsEnabled)
                    await CheckForCodeUpdates();
            }
        }

        private void UI_Download_Codes(object sender, RoutedEventArgs e)
        {
            UpdateStatus(string.Format(Localise("StatusUIDownloadingCodes"), HedgeApp.CurrentGame.GameName));
            try
            {
                var downloader = new DownloadWindow($"Downloading codes for {HedgeApp.CurrentGame.GameName}", HedgeApp.CurrentGame.CodesURL, CodeProvider.CodesTextPath)
                {
                    DownloadCompleted = () =>
                    {
                        Refresh();
                        UpdateStatus(Localise("StatusUIDownloadFinished"));

                        // Update button visual.
                        {
                            CodesOutdated = false;

                            // Reset button text if there was an update that was just downloaded.
                            Button_DownloadCodes.Content = Localise("CodesUIDownload");
                        }
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
            Process.Start(HedgeApp.ModsDbPath);
        }

        private void UI_ChangeDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                Title = Localise("MainUISelectModsDBTitle")
            };

            if (dialog.ShowDialog())
            {
                HedgeApp.ModsDbPath = dialog.SelectedFolder;
                ViewModel.CPKREDIR.ModsDbIni = Path.Combine(HedgeApp.ModsDbPath, SelectedModProfile.ModDBPath);
                if (ViewModel.CPKREDIR.ModsDbIni.StartsWith(HedgeApp.StartDirectory))
                    ViewModel.CPKREDIR.ModsDbIni = ViewModel.CPKREDIR.ModsDbIni.Substring(HedgeApp.StartDirectory.Length + 1);
                ViewModel.CPKREDIR.Save(Path.Combine(HedgeApp.StartDirectory, "cpkredir.ini"));
                Refresh();
                UpdateStatus(Localise("StatusUIModsDBLocationChanged"));
            }
        }

        private void ComboBox_Languages_Changed(object sender, SelectionChangedEventArgs e)
        {
            HedgeApp.ChangeLanguage();
            RefreshUI();
        }

        private void ComboBox_Languages_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Languages.GetBindingExpression(ComboBox.ItemsSourceProperty).UpdateTarget();
            ComboBox_Languages.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
        }

        private void ComboBox_Themes_Changed(object sender, SelectionChangedEventArgs e)
        {
            HedgeApp.UpdateTheme();
            RefreshUI();
        }

        private void ComboBox_Themes_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Themes.GetBindingExpression(ComboBox.ItemsSourceProperty).UpdateTarget();
            ComboBox_Themes.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
        }

        private void UI_ConfigureMod_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.SelectedMod.HasSchema)
                return;

            var window = new ModConfigWindow(ViewModel.SelectedMod);
            window.ShowDialog();
        }

        private void UI_ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            if (!(sender is ListViewItem listItem))
                return;

            var item = HedgeApp.FindChild<MenuItem>(listItem.ContextMenu, "ContextMenuItemConfigure");
            if (item == null)
                return;

            item.IsEnabled = ViewModel.SelectedMod.HasSchema;
        }

        private void ModsList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ViewModel.SelectedMod == null)
                return;

            var mod = ViewModel.SelectedMod;
            var ctrlKey = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var altKey = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

            if (Keyboard.IsKeyDown(Key.Space))
                mod.Enabled = !mod.Enabled;

            if (ctrlKey)
            {
                if (Keyboard.IsKeyDown(Key.C))
                    UI_ConfigureMod_Click(null, null);
                else if (Keyboard.IsKeyDown(Key.D))
                    UI_Description_Click(null, null);
                else if (Keyboard.IsKeyDown(Key.E))
                    UI_Edit_Mod(null, null);
                else if (Keyboard.IsKeyDown(Key.O))
                    UI_Open_Folder(null, null);
                else if (Keyboard.IsKeyDown(Key.U))
                    UI_Update_Mod(null, null);

                e.Handled = true;
            }
            else if (altKey)
            {
                if (Keyboard.IsKeyDown(Key.F))
                    UI_Favorite_Click(null, null);

                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.Delete))
            {
                UI_RemoveMod_Click(null, null);
                e.Handled = true;
            }
        }

        private void CodesList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ViewModel.SelectedCode == null)
                return;

            var code = ViewModel.SelectedCode;

            if (Keyboard.IsKeyDown(Key.Space))
            {
                code.Enabled = !code.Enabled;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var shiftKey = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftKey && Keyboard.IsKeyDown(Key.F1))
            {
                try
                {
                    var time = DateTime.Now;
                    var path =
                        $"HMM_Snapshot_{time.Date:00}{time.Month:00}{time.Year:0000}{time.Hour:00}{time.Minute:00}{time.Second:00}.txt";

                    File.WriteAllText(path, Convert.ToBase64String(SnapshotBuilder.Build()));
                    Process.Start($"explorer.exe", $"/select,\"{System.IO.Path.GetFullPath(path)}\"");
                    HedgeApp.CreateOKMessageBox("Hedge Mod Manager", $"Please attach the file\n{path}\nto the issue.").ShowDialog();
                }
                catch { }

            }
        }

        private async void ComboBox_ModProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ignore event when combobox is initalising 
            if (ComboBox_ModProfile.SelectedItem == null || ComboBox_ModProfile.SelectedItem == SelectedModProfile)
                return;

            // Save profile
            try
            {
                await SaveModsDB();
            }
            catch (Exception ex)
            {
                new ExceptionWindow(ex).ShowDialog();
            }
            SelectedModProfile.Enabled = false;
            SelectedModProfile = ComboBox_ModProfile.SelectedItem as ModProfile ?? HedgeApp.ModProfiles.First();
            SelectedModProfile.Enabled = true;
            HedgeApp.Config.ModProfile = SelectedModProfile.Name;
            string profilePath = Path.Combine(HedgeApp.StartDirectory, "profiles.json");
            HedgeApp.Config.Save(HedgeApp.ConfigPath);
            File.WriteAllText(profilePath, JsonConvert.SerializeObject(HedgeApp.ModProfiles));
            Refresh();
        }

        private void UI_ManageProfile_Click(object sender, RoutedEventArgs e)
        {
            var manager = new ProfileManagerWindow();
            manager.DataContext = DataContext;
            manager.ShowDialog();
            // Update profiles
            HedgeApp.ModProfiles.Clear();
            HedgeApp.ModProfiles.AddRange(ViewModel.Profiles);
            // Save profiles
            string profilePath = Path.Combine(HedgeApp.StartDirectory, "profiles.json");
            File.WriteAllText(profilePath, JsonConvert.SerializeObject(HedgeApp.ModProfiles));
            Refresh();
        }
    }
}
