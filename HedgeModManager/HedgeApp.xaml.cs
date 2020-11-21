using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
using HedgeModManager.Github;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Serialization;
using HedgeModManager.Languages;
using HedgeModManager.Misc;
using HedgeModManager.UI;
using Newtonsoft.Json;

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
        public static SteamGame CurrentSteamGame;
        public static CPKREDIRConfig Config;
        public static List<SteamGame> SteamGames = null;
        public static bool Restart = false;
        public static string PCCulture = "";

        public const string WebRequestUserAgent =
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
        
        public const string RepoOwner = "thesupersonic16";
        public const string RepoName = "hedgemodmanager";

        public static byte[] CPKREDIR = new byte[] { 0x63, 0x70, 0x6B, 0x72, 0x65, 0x64, 0x69, 0x72 };
        public static byte[] IMAGEHLP = new byte[] { 0x69, 0x6D, 0x61, 0x67, 0x65, 0x68, 0x6C, 0x70 };

        public static LanguageList SupportedCultures { get; set; }

        public static LangEntry CurrentCulture { get; set; }


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

            if(args.Length > 2 && string.Compare(args[0], "-update", StringComparison.OrdinalIgnoreCase) >= 0)
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

            var application = new HedgeApp();
            application.InitializeComponent();
            application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            application.MainWindow = new MainWindow();
            Args = args;
#if !DEBUG
            // Enable our Crash Window if Compiled in Release
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                ExceptionWindow.UnhandledExceptionEventHandler(e.ExceptionObject as Exception, e.IsTerminating);
            };
#endif
            // Gets the embeded version
            CPKREDIRVersion = GetCPKREDIRFileVersion(true);
            RegistryConfig.Load();

            Steam.Init();
            InstallGBHandlers();
            SetupLanguages();
            CurrentCulture = GetClosestCulture(RegistryConfig.UILanguage);
            if (CurrentCulture != null)
                LoadLanguage(CurrentCulture.FileName);
            CountLanguages();
#if DEBUG
            // Find a Steam Game
            SteamGames = Steam.SearchForGames("Sonic Generations");
            var steamGame = SteamGames.FirstOrDefault();
            SelectSteamGame(steamGame);
            StartDirectory = steamGame.RootDirectory;
            if (File.Exists("key.priv.xml"))
            {
                using (var stream = File.OpenRead("key.priv.xml"))
                {
                    var serializer = new XmlSerializer(typeof(RSAParameters));
                    CryptoProvider.ImportParameters((RSAParameters) serializer.Deserialize(stream));
                }
            }
#else
            SteamGames = Steam.SearchForGames();
            if (FindAndSetLocalGame() == null)
            {
                if (!string.IsNullOrEmpty(RegistryConfig.LastGameDirectory) && CurrentGame == Games.Unknown)
                {
                    StartDirectory = RegistryConfig.LastGameDirectory;
                    FindAndSetLocalGame();
                }
            }

            if (CurrentGame == Games.Unknown)
            {
                var game = SteamGames.FirstOrDefault();
                SelectSteamGame(game);
                StartDirectory = game?.RootDirectory;
            }

            if (CurrentGame == Games.Unknown)
            {
                var dialog = new HedgeMessageBox($"No Games Found!", 
                    "Please make sure your games are properly installed on Steam or\nRun Hedge Mod Manager inside of any of the supported game's directory.");

                dialog.AddButton("Exit", () =>
                {
                    Environment.Exit(0);
                });

                dialog.ShowDialog();
            }

#endif
            if (string.IsNullOrEmpty(ModsDbPath))
                ModsDbPath = Path.Combine(StartDirectory, "Mods");
            ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "-h")
                {
                    ShowHelp();
                    return;
                }

                if (args[0].ToLower() == "-gb" && args.Length > 1)
                {
                    GBAPI.ParseCommandLine(args[1]);
                    return;
                }
                
                if (args[0].ToLower() == "-encrypt")
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Insufficient arguments.");
                        return;
                    }

                    var filename = args[1] + ".bytes";
                    if (args.Length > 2)
                        filename = args[2];

                    using (var file = File.OpenRead(args[1]))
                    using (var encrypted = File.Create(filename))
                    {
                        CryptoProvider.Encrypt(file, encrypted);
                        Console.WriteLine($"Successfully encrypted {filename}");
                    }
                    return;
                }

                if (args[0].ToLower() == "-decrypt")
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Insufficient arguments.");
                        return;
                    }

                    var filename = Path.ChangeExtension(args[1], string.Empty);
                    if (args.Length > 2)
                        filename = args[2];

                    using (var encrypted = File.OpenRead(args[1]))
                    using (var decrypted = File.Create(filename))
                    {
                        CryptoProvider.Decrypt(encrypted, decrypted);
                        Console.WriteLine($"Successfully decrypted {filename}");
                    }
                    return;
                }

                if (args[0].ToLower() == "-decrypt64")
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Insufficient arguments.");
                        return;
                    }

                    var filename = Path.ChangeExtension(args[1], string.Empty);
                    if (args.Length > 2)
                        filename = args[2];


                    byte[] data = Convert.FromBase64String(File.ReadAllText(args[1]));

                    using (var encrypted = new MemoryStream(data))
                    using (var decrypted = File.Create(filename))
                    {
                        CryptoProvider.Decrypt(encrypted, decrypted);
                        Console.WriteLine($"Successfully decrypted {filename}");
                    }
                    return;
                }
            }

            if (CurrentGame.SupportsCPKREDIR)
            {
                if (!File.Exists(Path.Combine(StartDirectory, "cpkredir.dll")))
                {
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.dll"), HMMResources.DAT_CPKREDIR_DLL);
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.txt"), HMMResources.DAT_CPKREDIR_TXT);
                }
            }

            // Try to remove old patch
            try
            {
                string exePath = Path.Combine(StartDirectory, CurrentGame.ExecuteableName);
                if (IsCPKREDIRInstalled(exePath))
                    InstallCPKREDIR(exePath, false);
            }
            catch{ }

            CodeProvider.TryLoadRoslyn();
            do
            {
                Config = new CPKREDIRConfig(ConfigPath);
                Restart = false;
                application.Run(application.MainWindow);
            }
            while (Restart);
        }

        public static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine($"HedgeModManager {VersionString}\n");
            
            Console.WriteLine("Commands:");

            Console.WriteLine("    -encrypt");
            Console.WriteLine("        Usage: filename [output]");

            Console.WriteLine("    -decrypt");
            Console.WriteLine("        Usage: filename [output]");
        }

        public static SteamGame FindAndSetLocalGame()
        {
            foreach (var game in Games.GetSupportedGames())
            {
                if (File.Exists(Path.Combine(StartDirectory, game.ExecuteableName)))
                {
                    var steamGame = SteamGames.FirstOrDefault(x => x.GameID == game.AppID);
                    if (steamGame == null)
                    {
                        steamGame = new SteamGame(game.GameName, Path.Combine(StartDirectory, game.ExecuteableName), game.AppID);
                        SteamGames.Add(steamGame);
                    }
                    CurrentGame = game;
                    CurrentSteamGame = steamGame;
                    RegistryConfig.LastGameDirectory = StartDirectory;
                    RegistryConfig.Save();
                    ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");
                    Config = new CPKREDIRConfig(ConfigPath);
                    ModsDbPath = Path.Combine(StartDirectory, Path.GetDirectoryName(Config.ModsDbIni));
                    return steamGame;
                }
            }
            return null;
        }

        public static void CountLanguages()
        {
            foreach (var entry in SupportedCultures)
            {
                var langDict = new ResourceDictionary();
                langDict.Source = new Uri($"Languages/{entry.FileName}.xaml", UriKind.Relative);
                entry.Lines = langDict.Count;
                if (entry.Lines > LanguageList.TotalLines)
                    LanguageList.TotalLines = entry.Lines;
            }
        }

        public static void LoadLanguage(string culture)
        {
            var langDict = new ResourceDictionary();
            langDict.Source = new Uri($"Languages/{culture}.xaml", UriKind.Relative);
            while (Current.Resources.MergedDictionaries.Count > 2)
                Current.Resources.MergedDictionaries.RemoveAt(2);
            // No need to load the fallback language on top
            if (culture == "en-AU")
                return;
            Current.Resources.MergedDictionaries.Add(langDict);
        }

        public static LangEntry GetClosestCulture(string culture)
        {
            // Check if the culture exists
            var cultureEntry = SupportedCultures.FirstOrDefault(t => t.FileName == culture);
            if (cultureEntry != null)
                return cultureEntry;
            // Find anouther culture based off language
            string language = culture.Split('-')[0];
            cultureEntry = SupportedCultures.FirstOrDefault(t => t.FileName.Split('-')[0] == language);
            cultureEntry = cultureEntry ?? SupportedCultures.First();
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

        /// <summary>
        /// Sets the Current Game to the passed Steam Game
        /// </summary>
        /// <param name="steamGame">Steam Game to select</param>
        public static void SelectSteamGame(SteamGame steamGame)
        {
            if (steamGame == null)
                return;

            foreach(var game in Games.GetSupportedGames())
            {
                if (game.AppID == steamGame.GameID)
                {
                    CurrentGame = game;
                    CurrentSteamGame = steamGame;
                    StartDirectory = steamGame.RootDirectory;
                    RegistryConfig.LastGameDirectory = StartDirectory;
                    RegistryConfig.Save();
                }
            }

            ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");
            Config = new CPKREDIRConfig(ConfigPath);
            ModsDbPath = Path.Combine(StartDirectory, Path.GetDirectoryName(Config.ModsDbIni));
        }

        public static void InstallGBHandlers()
        {
            foreach(var game in Games.GetSupportedGames())
            {
                GBAPI.InstallGBHandler(game);
            }
        }

        /// <summary>
        /// Finds and returns an instance of SteamGame from a HMM Game
        /// </summary>
        /// <param name="game">HMM Game</param>
        /// <returns>Steam Game</returns>
        public static SteamGame GetSteamGame(Game game)
        {
            return SteamGames.FirstOrDefault(t => t.GameName == game.GameName);
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
            box.AddButton("OK", () => box.Close());
            return box;
        }

        public static void InstallOtherLoader(bool toggle = true)
        {
            if (CurrentGame.SupportsCPKREDIR)
            {
                if (!File.Exists(Path.Combine(StartDirectory, "cpkredir.dll")))
                {
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.dll"), HMMResources.DAT_CPKREDIR_DLL);
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.txt"), HMMResources.DAT_CPKREDIR_TXT);
                }
            }

            string DLLFileName = Path.Combine(StartDirectory, CurrentGame.CustomLoaderFileName);

            if (File.Exists(DLLFileName) && toggle)
            {
                File.Delete(DLLFileName);
                return;
            }

            // Downloads the loader
            var downloader = new DownloadWindow($"Downloading {CurrentGame.CustomLoaderName}",
                CurrentGame.ModLoaderDownloadURL, DLLFileName);
           
            downloader.DownloadFailed += (ex) =>
            {
                var loader = CurrentGame.ModLoaderData;
                if (loader != null)
                    File.WriteAllBytes(DLLFileName, loader);
                else
                    throw new NotImplementedException("No Loader is available!");
            };

            downloader.Start();
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

        public static (bool, ReleaseInfo) CheckForUpdates()
        {
            var info = GithubAPI.GetLatestRelease(RepoOwner, RepoName);
            var version = info == null ? Version : info.GetVersion();
            bool hasUpdate = version > Version;

            return (hasUpdate, info);
        }

        public static string GetCPKREDIRVersionString()
        {
            var temp = Path.Combine(StartDirectory, "cpkredir.dll");
            FileVersionInfo info = null;
            if(!File.Exists(temp))
            {
                temp = Path.GetTempFileName();
                File.WriteAllBytes(temp, HMMResources.DAT_CPKREDIR_DLL);
                info = FileVersionInfo.GetVersionInfo(temp);
                File.Delete(temp);
            }

            info = info ?? FileVersionInfo.GetVersionInfo(temp);
            return $"{info.ProductName} v{info.FileVersion}";
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

        /// <summary>
        /// Checks the current version of CPKREDIR with the embeded one and updates it if the current is older
        /// </summary>
        public static void UpdateCPKREDIR()
        {
            if (GetCPKREDIRFileVersion(false) is string currentVersionString)
            {
                if (int.TryParse(CPKREDIRVersion.Replace(".", ""), out int packedVersion) &&
                    int.TryParse(currentVersionString.Replace(".", ""), out int currentVersion) &&
                    packedVersion > currentVersion)
                {
                    // Write embeded CPKREDIR
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.dll"), HMMResources.DAT_CPKREDIR_DLL);
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.txt"), HMMResources.DAT_CPKREDIR_TXT);
                }
            }
        }

        public static string GetCodeLoaderVersion(Game game)
        {
            try
            {
                var loaderPath = Path.Combine(StartDirectory, game.CustomLoaderFileName);

                if (!game.HasCustomLoader)
                    return null;

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

        public static (Version LoaderVersion, Version MinCodeVersion) GetCodeLoaderInfo(Game game)
        {
            try
            {
                var minCodeVersion = "0.1";
                var loaderVersion = GetCodeLoaderVersion(game);

                if (loaderVersion != minCodeVersion)
                {
                    using (var res = new DllResource(Path.Combine(StartDirectory, game.CustomLoaderFileName)))
                    {
                        minCodeVersion = res.GetString(Games.CodeLoaderMinCodeVersionStringId);
                    }
                }

                return (loaderVersion != "0.1" ? new Version(loaderVersion) : null, new Version(string.IsNullOrEmpty(minCodeVersion) ? "9999.9999" : minCodeVersion));
            }
            catch
            {
                return (new Version("0.1"), new Version("9999.9999"));
            }
        }

        public static string ComputeMD5Hash(string path)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
            {
                var hash = md5.ComputeHash(stream);
                var builder = new StringBuilder();
                foreach (var b in hash)
                {
                    builder.Append($"{b:X2}");
                }

                return builder.ToString();
            }
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
            var window = Window.GetWindow((DependencyObject)sender);
            var minbtn = (Button)window.Template.FindName("MinBtn", window);
            var maxbtn = (Button)window.Template.FindName("MaxBtn", window);
            maxbtn.IsEnabled = window.ResizeMode == ResizeMode.CanResizeWithGrip || window.ResizeMode == ResizeMode.CanResize;
            minbtn.IsEnabled = window.ResizeMode != ResizeMode.NoResize;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count < 1 || !(e.RemovedItems[0] is FrameworkElement))
                return;

            var oldControl = (FrameworkElement)((TabItem)e.RemovedItems[0]).Content;
            var control = (TabControl)sender;
            var tempArea = (System.Windows.Shapes.Shape)control.Template.FindName("PART_TempArea", (FrameworkElement)sender);
            var presenter = (ContentPresenter)control.Template.FindName("PART_Presenter", (FrameworkElement)sender);
            var target = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            target.Render(oldControl);
            tempArea.HorizontalAlignment = HorizontalAlignment.Stretch;
            tempArea.Fill = new ImageBrush(target);
            tempArea.RenderTransform = new TranslateTransform();
            presenter.RenderTransform = new TranslateTransform();
            presenter.RenderTransform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(control.ActualWidth, 0));
            tempArea.RenderTransform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(0, -control.ActualWidth, (x, y) => { tempArea.HorizontalAlignment = HorizontalAlignment.Left; }));
            tempArea.Fill.BeginAnimation(Brush.OpacityProperty, CreateAnimation(1, 0));
            

            AnimationTimeline CreateAnimation(double from, double to,
                          EventHandler whenDone = null)
            {
                IEasingFunction ease = new BackEase
                { Amplitude = 0.5, EasingMode = EasingMode.EaseOut };
                var duration = new Duration(TimeSpan.FromSeconds(0.4));
                var anim = new DoubleAnimation(from, to, duration)
                { EasingFunction = ease };
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
