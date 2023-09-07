// Uncomment the line below if you want to quickly figure out which language keys are missing
// #define THROW_MISSING_LANG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using HMMResources = HedgeModManager.Properties.Resources;
using System.Windows.Media.Animation;
using GameBananaAPI;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Markup;
using System.Windows.Resources;
using System.Xml.Serialization;
using HedgeModManager.GitHub;
using HedgeModManager.Languages;
using HedgeModManager.Misc;
using HedgeModManager.UI;
using HedgeModManager.Themes;
using HedgeModManager.UI.Models;
using HedgeModManager.Updates;
using System.Security;
using System.Windows.Interop;
using System.Windows.Shell;
using HedgeModManager.Annotations;
using static HedgeModManager.Lang;
using Microsoft.Win32;
using HedgeModManager.CLI;
using HedgeModManager.CodeCompiler;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for HedgeApp.xaml
    /// </summary>
    public partial class HedgeApp : Application
    {
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        public static Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static string StartDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string AppPath = Path.Combine(StartDirectory, AppDomain.CurrentDomain.FriendlyName);
        public static string ProgramName = "Hedge Mod Manager";
        public static string VersionString = $"{Version.Major}.{Version.Minor}-{Version.Revision}";
        public static string ModsDbPath;
        public static string ConfigPath;
        public static string CPKREDIRVersion;
        public static string[] Args;
        public static Game CurrentGame = Games.Unknown;
        public static GameInstall CurrentGameInstall;
        public static CPKREDIRConfig Config;
        public static List<GameInstall> GameInstalls = null;
        public static string PCCulture = "";
        public static NetworkConfig NetworkConfiguration = new Singleton<NetworkConfig>(new NetworkConfig());
        public static List<ModProfile> ModProfiles = new List<ModProfile>();
        public static bool AprilFools, IizukaBirthday, IsLinux = false;

        public static HttpClient HttpClient { get; private set; }
        public static string UserAgent { get; }
            = $"Mozilla/5.0 (compatible; HedgeModManager/{VersionString})";

        public const string RepoOwner = "thesupersonic16";
        public const string RepoName = "hedgemodmanager";
        public const string RepoBranch = "rewrite";
        public static string RepoCommit = HMMResources.Version.Trim();

        public static byte[] CPKREDIR = new byte[] { 0x63, 0x70, 0x6B, 0x72, 0x65, 0x64, 0x69, 0x72 };
        public static byte[] IMAGEHLP = new byte[] { 0x69, 0x6D, 0x61, 0x67, 0x65, 0x68, 0x6C, 0x70 };

        public static LanguageList SupportedCultures { get; set; }
        public static ThemeList InstalledThemes { get; set; }

        public static LangEntry CurrentCulture { get; set; }
        public static ThemeEntry CurrentTheme { get; set; }

        public static List<string> UpdateChannels { get; set; } = new() { "Release", "Development" };
        public static string CurrentChannel { get; set; } = string.IsNullOrEmpty(RepoCommit) ? "Release" : "Development";

        [STAThread]
        public static void Main(string[] args)
        {
            // Attach Console
            AttachConsole(-1);

            // Language
            PCCulture = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // Use TLSv1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);

            Singleton.SetInstance(HttpClient);
            Singleton.SetInstance<IWindowService>(new WindowServiceImplWindows());

            AprilFools      = DateTime.Now.Day == 1 && DateTime.Now.Month == 4;
            IizukaBirthday  = DateTime.Now.Day == 16 && DateTime.Now.Month == 3;

            // Check for Wine, assuming Linux
            RegistryKey key = null;
            if ((key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey("SOFTWARE\\Wine")) != null)
            {
                key.Close();
                IsLinux = true;
            }


            if (args.Length > 2 && string.Compare(args[0], "-update", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                try
                {
                    // The old pid gets passed in the CLI arguments and we use that to make sure the process is terminated before replacing it
                    int.TryParse(args[2], out int pid);
                    var process = Process.GetProcessById(pid);

                    process.WaitForExit();
                }
                catch { }

                File.Copy(AppPath, args[1], true);

                // Start a process that deletes our updater
                new Process()
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", $"/C choice /C Y /N /D Y /T 0 & Del \"{AppPath}\"")
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                }.Start();

                Process.Start(args[1]);
                Environment.Exit(0);
                return;
            }

            // Include commit hash if defined
            if (!string.IsNullOrEmpty(RepoCommit))
            {
                ProgramName += " Development";
                VersionString += $"-{RepoCommit.Substring(0, 7)}";
            }

            var application = new HedgeApp();
            application.InitializeComponent();
            application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            application.MainWindow = new MainWindow();

            Args = args;
#if !DEBUG
            // Add the exception handler when compiled in Release
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                ExceptionWindow.UnhandledExceptionEventHandler(e.ExceptionObject as Exception, e.IsTerminating);
            };
#endif

            SplashScreen splashScreen = null;
            if (AprilFools || IizukaBirthday)
            {
                splashScreen = new ("Resources/Graphics/splash.png");
                splashScreen.Show(false, true);
            }

            // Gets the embeded version
            CPKREDIRVersion = GetCPKREDIRFileVersion(true);
            RegistryConfig.Load();
            _ = LoadNetworkConfigAsync();

            Steam.Init();
            InstallGBHandlers();
            InstallOneClickHandler();
            SetupLanguages();
            LoadLanguageFolder();
            SetupThemes();
            CurrentTheme = InstalledThemes.FirstOrDefault(t => t.FileName == RegistryConfig.UITheme);
            LoadTheme(CurrentTheme?.FileName ?? InstalledThemes.First().FileName);
            CurrentCulture = GetClosestCulture(RegistryConfig.UILanguage);
            if (CurrentCulture != null)
                LoadLanguage(CurrentCulture.FileName);
            CountLanguages();
#if DEBUG
            // Find a Steam Game
            GameInstalls = GameInstall.SearchForGames("SonicGenerations");
            var steamGame = GameInstalls.FirstOrDefault();
            SelectGameInstall(steamGame);
            StartDirectory = steamGame?.GameDirectory ?? StartDirectory;
            if (File.Exists("key.priv.xml"))
            {
                using (var stream = File.OpenRead("key.priv.xml"))
                {
                    var serializer = new XmlSerializer(typeof(RSAParameters));
                    CryptoProvider.ImportParameters((RSAParameters)serializer.Deserialize(stream));
                }
            }
#else
            GameInstalls = GameInstall.SearchForGames();
            if (FindAndSetLocalGame() == null)
            {
                if (!string.IsNullOrEmpty(RegistryConfig.LastGameDirectory) && CurrentGame == Games.Unknown)
                {
                    StartDirectory = RegistryConfig.LastGameDirectory;
                    FindAndSetLocalGame();
                }
            }

#endif

            if (GameInstalls.Count == 0)
                GameInstalls.Add(new GameInstall(Games.Unknown, null, GameLauncher.None));

            bool modsDbValidPath = !string.IsNullOrEmpty(ModsDbPath) && Directory.Exists(ModsDbPath);
            if (!modsDbValidPath && !string.IsNullOrEmpty(StartDirectory))
                ModsDbPath = Path.Combine(StartDirectory, "Mods");
            if (!string.IsNullOrEmpty(StartDirectory))
                ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");
            

            // Try to remove old patch
            try
            {
                if (CurrentGame.SupportsCPKREDIR)
                {
                    string exePath = Path.Combine(StartDirectory, CurrentGame.ExecutableName);
                    if (IsCPKREDIRInstalled(exePath))
                        InstallCPKREDIR(exePath, false);

                    CurrentGame.ModLoader.MakeCompatible(StartDirectory);
                }
            }
            catch { }

            if (AprilFools)
            {
                var random = new Random();
                if (random.Next(10) == 0)
                {
                    var langDict = new ResourceDictionary { Source = new Uri("Languages/en-UW.xaml", UriKind.Relative) };
                    Current.Resources.MergedDictionaries.RemoveAt(3);
                    Current.Resources.MergedDictionaries.Insert(3, langDict);
                }
            }

            CodeProvider.TryLoadRoslyn();

            if (splashScreen != null)
                splashScreen.Close(TimeSpan.FromSeconds(0.5));

            application.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // URL command
            if (e.Args.Length >= 1 && e.Args[0].ToLowerInvariant().StartsWith("hedgemm://"))
            {
                string arg = e.Args[0];
                if (arg.StartsWith("hedgemm://install/", StringComparison.InvariantCultureIgnoreCase))
                {
                    string url = arg.Substring("hedgemm://install/".Length);
                    new ModInstallWindow(url).ShowDialog();
                }
                Shutdown();
            }

            var args = CommandLine.ParseArguments(e.Args);
            CommandLine.ExecuteArguments(args);

            base.OnStartup(e);
            MainWindow.Show();
        }

        private static async Task LoadNetworkConfigAsync()
        {
            Singleton.SetInstance(await NetworkConfig.LoadConfig(
                $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/{RepoBranch}/HMMNetworkConfig.json"));
        }

        public static Uri GetResourceUri(string path)
        {
            return new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/" + path, UriKind.Absolute);
        }

        public static Dictionary<string, string> ParseArguments(string[] args)
        {
            var dict = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].StartsWith("-") && i + 1 != args.Length && !args[i + 1].StartsWith("-"))
                    dict[args[i]] = args[++i];
                else
                    dict[args[i]] = null;
            }
            return dict;
        }

        public static GameInstall FindAndSetLocalGame()
        {
            foreach (var game in Games.GetSupportedGames())
            {
                if (File.Exists(Path.Combine(StartDirectory, game.ExecutableName)))
                {
                    var steamGame = GameInstalls.FirstOrDefault(x => x.BaseGame == game);
                    if (steamGame == null)
                    {
                        steamGame = new GameInstall(game, StartDirectory, GameLauncher.None);
                        GameInstalls.Add(steamGame);
                    }
                    CurrentGame = game;
                    CurrentGameInstall = steamGame;
                    try
                    {
                        RegistryConfig.LastGameDirectory = StartDirectory;
                        RegistryConfig.Save();
                        ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");
                        Config = new CPKREDIRConfig(ConfigPath);
                        ModsDbPath = Path.Combine(StartDirectory, Path.GetDirectoryName(Config.ModsDbIni));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Current.MainWindow = CreateOKMessageBox(Localise("CommonUIError"),
                            string.Format(Localise("DialogUINoGameDirAccess"), StartDirectory));
                        Current.MainWindow.ShowDialog();
                        Environment.Exit(-1);
                    }
                    return steamGame;
                }
            }
            return null;
        }

        public static async void DumpLanguage(string culture)
        {
            string url = $"https://raw.githubusercontent.com/{RepoOwner}/{RepoName}/{RepoBranch}/HedgeModManager/Languages/en-AU.xaml";
            var message = await Singleton.GetInstance<HttpClient>().GetStringAsync(url);
            var lines = message.Replace("\r", "").Split('\n');

            var langDict = GetLanguageResource(culture);

            foreach (DictionaryEntry baseEntry in langDict)
            {
                for (int i = 0; i < lines.Length; ++i)
                {
                    if (baseEntry.Key is string key && lines[i].Contains($"\"{key}\""))
                        lines[i] = lines[i].Substring(0, lines[i].IndexOf(">") + 1)
                            + SecurityElement.Escape(baseEntry.Value as string).Replace("\n", "&#x0a;")
                            + lines[i].Substring(lines[i].IndexOf("</"));
                }
            }
            if (!Directory.Exists("Languages"))
                Directory.CreateDirectory("Languages");
            string path = $"Languages\\{CurrentCulture.FileName}.xaml";
            File.WriteAllText(path, string.Join("\r\n", lines));
            Process.Start($"explorer.exe", $"/select,\"{path}\"");
        }

        public static ResourceDictionary GetLanguageResource(string culture)
        {
            var entry = GetClosestCulture(culture);
            if (entry == null)
                return null;

            try
            {
                if (entry.Local)
                {
                    ResourceDictionary langDict;
                        using var stream = File.OpenRead($"Languages/{entry.FileName}.xaml");
                        langDict = XamlReader.Load(stream) as ResourceDictionary;
                        return langDict;
                }
                else
                {
                    var langDict = new ResourceDictionary();
                    langDict.Source = new Uri($"Languages/{entry.FileName}.xaml", UriKind.Relative);
                    return langDict;
                }
            }
            catch (Exception e)
            {
                throw new Exceptions.LanguageLoadException(entry.FileName, e);
            }
        }

        public static void CountLanguages()
        {
            // Just to make sure this somehow doesn't get shipped accidentally
#if THROW_MISSING_LANG && DEBUG
            var baseDict = new ResourceDictionary {Source = new Uri("Languages/en-AU.xaml", UriKind.Relative)};
            var builder = new StringBuilder();
            builder.AppendLine();
#endif

            foreach (var entry in SupportedCultures)
            {
                var langDict = GetLanguageResource(entry.FileName);
                entry.Lines = langDict.Count;

#if THROW_MISSING_LANG && DEBUG
                builder.AppendLine(entry.FileName);
                foreach (DictionaryEntry baseEntry in baseDict)
                {
                    if (!langDict.Contains(baseEntry.Key))
                    {
                        builder.AppendLine(baseEntry.Key.ToString());
                    }
                }

                builder.AppendLine();
#endif

                if (entry.Lines > LanguageList.TotalLines)
                    LanguageList.TotalLines = entry.Lines;
            }

#if THROW_MISSING_LANG && DEBUG
            new ExceptionWindow(new Exception(builder.ToString())).ShowDialog();
#endif
        }

        public static void LoadLanguage(string culture)
        {
            var langDict = GetLanguageResource(culture);

            // Fallback if language is not loaded
            langDict ??= GetLanguageResource("en-AU");

            while (Current.Resources.MergedDictionaries.Count > 5)
                Current.Resources.MergedDictionaries.RemoveAt(5);
            // No need to load the fallback language on top
            if (culture == "en-AU")
                return;
            Current.Resources.MergedDictionaries.Add(langDict);
        }

        public static void LoadLanguageFolder()
        {
            if (Directory.Exists("Languages"))
            {
                foreach (string path in Directory.EnumerateFiles("Languages").Where(t => t.EndsWith(".xaml")))
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    if (SupportedCultures.Any(t => t.FileName == fileName))
                    {
                        SupportedCultures.FirstOrDefault(t => t.FileName == fileName).Local = true;
                    }
                    else
                    {
                        SupportedCultures.Add(new LangEntry()
                        {
                            FileName = fileName,
                            Name = fileName,
                            Local = true
                        });
                    }
                }
            }
        }

        public static LangEntry GetClosestCulture(string culture)
        {
            // Check if the culture exists
            var cultureEntry = SupportedCultures.FirstOrDefault(t => t.FileName == culture);
            if (cultureEntry != null)
                return cultureEntry;
            // Find another culture based off language
            string language = culture.Split('-')[0];
            cultureEntry = SupportedCultures.FirstOrDefault(t => t.FileName.Split('-')[0] == language);
            cultureEntry ??= SupportedCultures.First();
            return cultureEntry;
        }

        public static void SetupLanguages()
        {
            var resource = Current.TryFindResource("Languages");
            if (resource is LanguageList langs)
                SupportedCultures = langs;
        }

        public static void ChangeLanguage()
        {
            RegistryConfig.UILanguage = CurrentCulture.FileName;
            RegistryConfig.Save();
            LoadLanguage(CurrentCulture.FileName);
        }

        public static void FindMissingLanguageEntries(string culture)
        {
            var entry = GetClosestCulture(culture);
            var baseDict = new ResourceDictionary { Source = new Uri("Languages/en-AU.xaml", UriKind.Relative) };
            var builder = new StringBuilder();
            builder.AppendLine();

            var langDict = GetLanguageResource(entry.FileName);

            builder.AppendLine("Missing Entries:");
            foreach (DictionaryEntry baseEntry in baseDict)
                if (!langDict.Contains(baseEntry.Key))
                    builder.AppendLine(baseEntry.Key.ToString());

            builder.AppendLine();
            new ExceptionWindow(new Exception(builder.ToString())).ShowDialog();
        }

        public static void SetupThemes()
        {
            var resource = Current.TryFindResource("Themes");
            if (resource is ThemeList themes)
                InstalledThemes = themes;
        }

        public static void UpdateTheme()
        {
            RegistryConfig.UITheme = CurrentTheme.FileName;
            RegistryConfig.Save();
            LoadTheme(CurrentTheme.FileName);
        }

        public static void LoadTheme(string themeName)
        {
            var themeDict = new ResourceDictionary();
            themeDict.Source = new Uri($"Themes/{themeName}.xaml", UriKind.Relative);

            Current.Resources.MergedDictionaries.RemoveAt(2);
            Current.Resources.MergedDictionaries.Insert(2, themeDict);
        }

        /// <summary>
        /// Sets the CurrentGame to the passed GameInstall
        /// </summary>
        /// <param name="gameinstall">Game to select</param>
        public static void SelectGameInstall(GameInstall gameinstall)
        {
            if (gameinstall == null)
                return;

            foreach (var game in Games.GetSupportedGames())
            {
                if (game == gameinstall.BaseGame)
                {
                    CurrentGame = game;
                    CurrentGameInstall = gameinstall;
                    StartDirectory = gameinstall.GameDirectory;
                    RegistryConfig.LastGameDirectory = StartDirectory;
                    RegistryConfig.Save();
                }
            }
            try
            {
                if (HedgeApp.CurrentGame != Games.Unknown)
                {
                    ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");
                    Config = new CPKREDIRConfig(ConfigPath);

                    ModsDbPath = Path.Combine(StartDirectory, Path.GetDirectoryName(Config.ModsDbIni) ?? "Mods");
                    if (!Directory.Exists(ModsDbPath))
                    {
                        ModsDbPath = Path.Combine(StartDirectory, "Mods");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Current.MainWindow = CreateOKMessageBox(Localise("CommonUIError"),
                    string.Format(Localise("DialogUINoGameDirAccess"), StartDirectory));
                Current.MainWindow.ShowDialog();
                Environment.Exit(-1);
            }
        }

        public static void InstallGBHandlers()
        {
            foreach (var game in Games.GetSupportedGames())
            {
                GBAPI.InstallGBHandler(game);
            }
        }

        /// <summary>
        /// Finds and returns an instance of GameInstall from a HMM Game
        /// </summary>
        /// <param name="game">HMM Game</param>
        /// <returns>GameInstall thats linked to the passed Game</returns>
        public static GameInstall GetGameInstall(Game game)
        {
            return GameInstalls.FirstOrDefault(t => t.BaseGame == game);
        }

        /// <summary>
        /// Checks if CPKREDIR is currently Installed
        /// </summary>
        /// <param name="executablePath">Path to the executable</param>
        /// <returns>
        /// TRUE: CPKREDIR is installed
        /// FALSE: CPKREDIR is not Installed
        /// </returns>
        public static bool IsCPKREDIRInstalled(string executablePath)
        {
            var data = File.ReadAllBytes(executablePath);
            var installed = BoyerMooreSearch(data, CPKREDIR) > 0;

            data = null;
            return installed;
        }

        /// <summary>
        /// Installs or Uninstalls CPKREDIR
        /// </summary>
        /// <param name="executablePath">Path to the executable</param>
        /// <param name="install">
        /// TRUE: Installs CPKREDIR (default)
        /// FALSE: Uninstalls CPKREDIR
        /// NULL: Toggle
        /// </param>
        public static void InstallCPKREDIR(string executablePath, bool? install = true)
        {
            // Backup Executable
            File.Copy(executablePath, $"{executablePath}.bak", true);

            // Read Executable
            var data = File.ReadAllBytes(executablePath);
            var offset = -1;

            // Search for the .rdata entry
            byte[] rdata = Encoding.ASCII.GetBytes(".rdata");
            byte[] buff = new byte[0x300 - 0x160];
            Array.Copy(data, 0x160, buff, 0, buff.Length);
            offset = BoyerMooreSearch(buff, rdata) + 0x160;

            // Read Segment Entry Data
            int size = BitConverter.ToInt32(data, offset + 0x10);
            int offset_ = BitConverter.ToInt32(data, offset + 0x14);

            // Read Segment
            buff = new byte[size];
            Array.Copy(data, offset_, buff, 0, buff.Length);

            bool IsCPKREDIR = false;
            offset = BoyerMooreSearch(buff, IMAGEHLP);
            IsCPKREDIR = offset == -1;
            if (offset == -1)
                offset = BoyerMooreSearch(buff, CPKREDIR);
            offset += offset_;
            byte[] buffer = null;
            // Toggle
            if (install == null)
                buffer = IsCPKREDIR ? IMAGEHLP : CPKREDIR;
            else
                buffer = install == false ? IMAGEHLP : CPKREDIR;

            // Write Patch to file
            using (var stream = File.OpenWrite(executablePath))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Write(buffer, 0, CPKREDIR.Length);
            }
        }

        public static HedgeMessageBox CreateOKMessageBox(string header, string message)
        {
            var box = new HedgeMessageBox(header, message);
            box.AddButton(Lang.Localise("CommonUIOK"), () => box.Close());
            return box;
        }

        public static bool UnInstallOtherLoader()
        {
            if (CurrentGame?.ModLoader == null)
            {
                return false;
            }

            try
            {
                var path = Path.Combine(StartDirectory, CurrentGame.ModLoader.ModLoaderFileName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool InstallOtherLoader(bool toggle = true)
        {
            bool installed = false;
            try
            {
                // Do not attempt if no loader exists
                if (CurrentGame.ModLoader == null)
                    return false;

                string DLLFileName = Path.Combine(StartDirectory, CurrentGame.ModLoader.ModLoaderFileName);
                CurrentGame.ModLoader.MakeCompatible(StartDirectory);

                if (File.Exists(DLLFileName) && toggle)
                {
                    installed = true;
                    File.Delete(DLLFileName);
                    return true;
                }

                // Downloads the loader
                var downloader = new DownloadWindow($"Downloading {CurrentGame.ModLoader.ModLoaderName}",
                    CurrentGame.ModLoader.ModLoaderDownloadURL, DLLFileName);

                downloader.DownloadFailed += (ex) =>
                {
                    var loader = CurrentGame.ModLoader.ModLoaderData;
                    if (loader != null)
                        File.WriteAllBytes(DLLFileName, loader);
                    else
                    {
                        CreateOKMessageBox("Hedge Mod Manager", Lang.Localise("MainUIMLDownloadFail")).ShowDialog();
                        if (File.Exists(DLLFileName))
                        {
                            try
                            {
                                File.Delete(DLLFileName);
                            }
                            catch { }
                        }
                    }
                };

                downloader.Start();
            }
            catch (Exception e)
            {
                CreateOKMessageBox("Hedge Mod Manager",
                    installed ? Lang.Localise("MainUIMLUninstallFail") : Lang.Localise("MainUIMLInstallFail")).ShowDialog();
                return false;
            }
            return true;
        }

        public static int BoyerMooreSearch(byte[] haystack, byte[] needle)
        {
            int[] lookup = new int[256];
            for (int i = 0; i < lookup.Length; i++) { lookup[i] = needle.Length; }

            for (int i = 0; i < needle.Length; i++)
            {
                lookup[needle[i]] = needle.Length - i - 1;
            }

            int index = needle.Length - 1;
            var lastByte = needle.Last();
            while (index < haystack.Length)
            {
                var checkByte = haystack[index];
                if (haystack[index] == lastByte)
                {
                    bool found = true;
                    for (int j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack[index - needle.Length + j + 1] != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return index - needle.Length + 1;
                    else
                        index++;
                }
                else
                {
                    index += lookup[checkByte];
                }
            }
            return -1;
        }

        // https://stackoverflow.com/questions/11660184/c-sharp-check-if-run-as-administrator
        public static bool RunningAsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static async Task<(bool, ReleaseInfo)> CheckForUpdatesAsync()
        {
            var info = await GitHubAPI.GetLatestRelease(RepoOwner, RepoName);
            var version = info == null ? Version : info.GetVersion();
            bool hasUpdate = version > Version;

            return (hasUpdate, info);
        }

        public static async Task<(bool, WorkflowRunInfo, ArtifactInfo)> CheckForUpdatesDevAsync()
        {
            var runs = await GitHubAPI.GetAllRuns(RepoOwner, RepoName, "build.yml");
            var workflow = runs.Runs.FirstOrDefault();
            if (workflow == null)
                return (false, null, null);

            bool hasUpdate = RepoCommit != workflow.HeadSHA;
            ArtifactInfo info = null;
            if (hasUpdate)
            {
                var list = await GitHubAPI.GetObject<ArtifactInfo.ArtifactList>(workflow.ArtifactsURL.ToString());
                info = list.Artifacts.FirstOrDefault(t => t.Expired == false && t.Name.Contains("Release"));
                // Just in case there is a configuration issue
                hasUpdate = info != null;
            }
            return (hasUpdate, workflow, info);
        }

        public static async Task<string> GetGitChangeLog(string hash)
        {
            var info = await GitHubAPI.GetAllCommits(RepoOwner, RepoName, hash);
            string text = "";
            int limit = info.ToList().FindIndex(t => t.SHA == RepoCommit);
            if (limit == -1)
                limit = info.Length;

            for (int i = 0; i < limit; ++i)
            {
                if (info[i].Commit.IsSkipCI()) continue;

                string message = info[i].Commit.Message.Replace("\r", "");
                if (message.Contains("\n"))
                    message = message.Substring(0, message.IndexOf("\n", StringComparison.Ordinal));

                text += $" - {info[i].SHA.Substring(0, 7)} - {message}\n";
            }
            return text;
        }

        public static void PerformUpdate(string path, string contentType)
        {
            // TODO: literally extracting a ZIP on the UI thread I couldn't make this up if I *tried*

            // Extract zip for compatibility for 6.x
            if (contentType == "application/x-zip-compressed" || contentType == "application/zip")
            {
                // Store old path pointing to the zip
                string oldPath = path;
                // Generate new path
                path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");
                using (var zip = ZipFile.Open(oldPath, ZipArchiveMode.Read))
                {
                    var entry = zip.Entries.FirstOrDefault(t => t.Name.EndsWith(".exe"));
                    entry.ExtractToFile(path);
                }
                File.Delete(oldPath);
            }

            Process.Start(path, $"-update \"{AppPath}\" {Process.GetCurrentProcess().Id}");
            Current.Shutdown();
        }

        /// <summary>
        /// Installs the one-click install handler
        /// </summary>
        public static bool InstallOneClickHandler()
        {
            try
            {
                if (IsLinux)
                    return Linux.GenerateDesktop();
                else
                {
                    var reg = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\hedgemm");
                    reg.SetValue("", $"URL:HedgeModManager");
                    reg.SetValue("URL Protocol", "");
                    reg = reg.CreateSubKey("shell\\open\\command");
                    reg.SetValue("", $"\"{HedgeApp.AppPath}\" \"%1\"");
                    reg.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes an URL. xdg-open is used on Linux
        /// </summary>
        /// <param name="url">URL to execute</param>
        /// <param name="useShellExecute">ProcessStartInfo.UseShellExecute</param>
        public static void StartURL(string url, bool useShellExecute = true)
        {
            if (IsLinux)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"start",
                    Arguments = $"/b /unix /usr/bin/xdg-open {url}",
                    UseShellExecute = useShellExecute
                });
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"{url}",
                    UseShellExecute = useShellExecute
                });
            }
        }

        private static string GetCPKREDIRFileVersion(bool? packed = null)
        {
            string version = null;
            var temp = Path.Combine(StartDirectory, "cpkredir.dll");
            if (File.Exists(temp) && packed != true)
                version = FileVersionInfo.GetVersionInfo(temp).FileVersion;

            if (version == null && packed != false)
            {
                version = Games.EmbeddedCPKREDIRVersion;
            }

            return version;
        }

        public static Version ExpandVersion(Version version)
        {
            var build = version.Build;
            var revision = version.Revision;

            return new Version(version.Major, version.Minor, build == -1 ? 0 : build, revision == -1 ? 0 : revision);
        }

        public static string GetCodeLoaderVersion(Game game)
        {
            try
            {
                if (game.ModLoader == null)
                    return null;

                var loaderPath = Path.Combine(StartDirectory, game.ModLoader.ModLoaderFileName);

                if (!File.Exists(loaderPath))
                    return null;

                var info = FileVersionInfo.GetVersionInfo(loaderPath);
                return info.ProductVersion ?? "0.1";
            }
            catch
            {
                return "0.1";
            }
        }

        public static string GetCodeLoaderName(Game game)
        {
            try
            {
                if (game.ModLoader == null)
                    return null;

                var loaderPath = Path.Combine(StartDirectory, game.ModLoader.ModLoaderFileName);

                if (!File.Exists(loaderPath))
                    return null;

                var info = FileVersionInfo.GetVersionInfo(loaderPath);
                return info.ProductName;
            }
            catch
            {
                return "null";
            }
        }

        [CanBeNull]
        public static CodeLoaderInfo GetCodeLoaderInfo(Game game)
        {
            try
            {
                var minCodeVersion = "0.1";
                var maxCodeVersion = minCodeVersion;
                var loaderVersion = GetCodeLoaderVersion(game);

                if (loaderVersion != minCodeVersion)
                {
                    using (var res = new DllResource(Path.Combine(StartDirectory, game.ModLoader.ModLoaderFileName)))
                    {
                        minCodeVersion = res.GetString(Games.CodeLoaderMinCodeVersionStringId);
                        maxCodeVersion = res.GetString(Games.CodeLoaderMaxCodeVersionStringId);
                    }
                }

                var lowCodeVersion = new Version(string.IsNullOrEmpty(minCodeVersion) ? "9999.9999" : minCodeVersion);
                return new CodeLoaderInfo(loaderVersion != "0.1" ? new Version(loaderVersion) : null,
                    lowCodeVersion,
                    string.IsNullOrEmpty(maxCodeVersion) ? lowCodeVersion : new Version(maxCodeVersion));
            }
            catch
            {
                return null;
            }
        }

        public static string ComputeMD5Hash(string path)
        {
            using var md5 = MD5.Create();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 12000);

            byte[] hash = md5.ComputeHash(stream);
            var builder = new StringBuilder();
            foreach (var b in hash)
            {
                builder.Append($"{b:X2}");
            }

            return builder.ToString();
        }

        public static string ComputeSHA1Hash(string path)
        {
            using (var sha1 = SHA1.Create())
            using (var stream = File.OpenRead(path))
            {
                var hash = sha1.ComputeHash(stream);
                var builder = new StringBuilder();
                foreach (var b in hash)
                {
                    builder.Append($"{b:X2}");
                }

                return builder.ToString();
            }
        }

        public static Guid GenerateSeededGuid(int seed)
        {
            var random = new Random(seed);
            var guid = new byte[16];

            random.NextBytes(guid);
            return new Guid(guid);
        }

        public static Color GetThemeColor(string key)
        {
            var resource = Current.TryFindResource(key);
            if (resource is null)
                return Colors.Black;

            if (resource is SolidColorBrush brush)
                return brush.Color;
            else if (resource is Color color)
                return color;

            return Colors.Black;
        }

        public static Brush GetThemeBrush(string key)
        {
            return Current.TryFindResource(key) as Brush;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow((DependencyObject)sender);
            window.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow((DependencyObject)sender);
            window.Close();
        }

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow((DependencyObject)sender);
            window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
            {
                return;
            }

            var minbtn = (Button)window.Template.FindName("MinBtn", window);
            var maxbtn = (Button)window.Template.FindName("MaxBtn", window);
            maxbtn.IsEnabled = window.ResizeMode is ResizeMode.CanResizeWithGrip or ResizeMode.CanResize;
            minbtn.IsEnabled = window.ResizeMode is not ResizeMode.NoResize;

            var handle = new WindowInteropHelper(window).Handle;
            int cornerPreference = 0;
            if (Win32.DwmGetWindowAttribute(handle, Win32.DwmWindowAttribute.WindowCornerPreference,
                    ref cornerPreference, sizeof(int)) == 0 && cornerPreference != 1) // DWMWCP_DONOTROUND
            {
                var outlineBorder = (Border)window.Template.FindName("OutlineBorder", window);

                // Push the window in a bit
                outlineBorder.BorderThickness = new Thickness(outlineBorder.BorderThickness.Left + 1,
                    outlineBorder.BorderThickness.Top + 1, outlineBorder.BorderThickness.Right + 1, outlineBorder.BorderThickness.Bottom + 1);

                outlineBorder.CornerRadius = (CornerRadius)window.FindResource("HedgeWindowRoundedCornerRadius");
                
                // Cursed
                Unsafe.Unbox<Thickness>(window.FindResource("HedgeWindowGridMargin")) = new Thickness(2);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count < 1 || e.AddedItems.Count < 1 || !(e.RemovedItems[0] is FrameworkElement))
                return;

            var oldControl = (FrameworkElement)((TabItem)e.RemovedItems[0]).Content;
            var control = (TabControl)sender;

            var isLeft = control.Items.IndexOf(e.RemovedItems[0]) < control.Items.IndexOf(e.AddedItems[0]);
            var tempArea = (System.Windows.Shapes.Shape)control.Template.FindName("PART_TempArea", (FrameworkElement)sender);
            var presenter = (ContentPresenter)control.Template.FindName("PART_Presenter", (FrameworkElement)sender);
            var target = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            target.Render(oldControl);

            tempArea.HorizontalAlignment = HorizontalAlignment.Stretch;
            tempArea.VerticalAlignment = VerticalAlignment.Stretch;
            tempArea.Fill = new ImageBrush(target);
            tempArea.Width = target.Width;
            tempArea.Height = target.Height;
            tempArea.RenderTransform = new TranslateTransform();
            presenter.RenderTransform = new TranslateTransform();

            presenter.RenderTransform.BeginAnimation(TranslateTransform.XProperty, isLeft ? CreateAnimation(control.ActualWidth, 0) : CreateAnimation(-control.ActualWidth, 0));
            tempArea.RenderTransform.BeginAnimation(TranslateTransform.XProperty, isLeft ?
                CreateAnimation(0, -control.ActualWidth, (x, y) => { tempArea.HorizontalAlignment = HorizontalAlignment.Left; }) :
                CreateAnimation(0, control.ActualWidth, (x, y) => { tempArea.HorizontalAlignment = HorizontalAlignment.Left; })
                );

            tempArea.BeginAnimation(UIElement.OpacityProperty, CreateAnimation(1, 0));


            AnimationTimeline CreateAnimation(double from, double to, EventHandler whenDone = null)
            {
                var ease = new ExponentialEase { Exponent = 7, EasingMode = EasingMode.EaseOut };
                var duration = new Duration(TimeSpan.FromSeconds(1.0 / 3.0));
                var anim = new DoubleAnimation(from, to, duration) { EasingFunction = ease };
                if (whenDone != null)
                    anim.Completed += whenDone;
                anim.Freeze();
                return anim;
            }
        }

        public static T FindChild<T>(ContextMenu parent, string childName)
            where T : FrameworkElement
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;


            foreach (FrameworkElement item in parent.Items)
            {
                if (item.Name == childName)
                    return (T)item;
            }

            return null;
        }
    }
}
