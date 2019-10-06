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
using System.Net.NetworkInformation;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string StartDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string AppPath = Path.Combine(StartDirectory, AppDomain.CurrentDomain.FriendlyName);
        public static string ProgramName = "HedgeModManager";
        public static string VersionString = "7.0-dev";
        public static string ModsDbPath;
        public static string ConfigPath;
        public static string CPKREDIRVersion;
        public static string[] Args;
        public static Game CurrentGame = Games.Unknown;
        public static CPKREDIRConfig Config;
        public static List<SteamGame> SteamGames = null;
        public static bool Restart = false;

        public const string WebRequestUserAgent =
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

        public static byte[] CPKREDIR = new byte[] { 0x63, 0x70, 0x6B, 0x72, 0x65, 0x64, 0x69, 0x72 };
        public static byte[] IMAGEHLP = new byte[] { 0x69, 0x6D, 0x61, 0x67, 0x65, 0x68, 0x6C, 0x70 };

        [STAThread]
        public static void Main(string[] args)
        {
            // Language
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // Use TLSv1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var application = new App();
            application.InitializeComponent();
            application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            application.MainWindow = new MainWindow();
            Args = args;
            CPKREDIRVersion = GetCPKREDIRVersion();
#if !DEBUG
            // Enable our Crash Window if Compiled in Release
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                ExceptionWindow.UnhandledExceptionEventHandler(e.ExceptionObject as Exception, e.IsTerminating);
            };
#endif

            Steam.Init();
            InstallGBHandlers();
#if DEBUG
            // Find a Steam Game
            SteamGames = Steam.SearchForGames("Sonic Generations");
            var steamGame = SteamGames.FirstOrDefault();
            SelectSteamGame(steamGame);
            StartDirectory = steamGame.RootDirectory;
#else
            SteamGames = Steam.SearchForGames();
            foreach (var game in Games.GetSupportedGames())
            {
                if (File.Exists(Path.Combine(StartDirectory, game.ExecuteableName)))
                {
                    CurrentGame = game;
                    break;
                }
            }
#endif
            ModsDbPath = Path.Combine(StartDirectory, "Mods");
            ConfigPath = Path.Combine(StartDirectory, "cpkredir.ini");

            if (args.Length > 1 && args[0] == "-gb")
            {
                string line = args[1].Substring(args[1].IndexOf(':') + 1);
                GBAPI.ParseCommandLine(args[1]);
                return;
            }

            if (CurrentGame == Games.Unknown)
            {
                var box = new HedgeMessageBox("No Game Detected!", HMMResources.STR_MSG_NOGAME);

                box.AddButton("  Cancel  ", () =>
                {
                    box.Close();
                });
                box.AddButton("  Run Installer  ", () =>
                {
                    box.Visibility = Visibility.Collapsed;
                    new InstallWindow().ShowDialog();
                    box.Close();
                });
                box.ShowDialog();
                return;
            }
            if (CurrentGame.SupportsCPKREDIR)
            {
                if (!File.Exists(Path.Combine(StartDirectory, "cpkredir.dll")))
                {
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.dll"), HMMResources.DAT_CPKREDIR_DLL);
                    File.WriteAllBytes(Path.Combine(StartDirectory, "cpkredir.txt"), HMMResources.DAT_CPKREDIR_TXT);
                }
            }

            do
            {
                Config = new CPKREDIRConfig(ConfigPath);
                Restart = false;
                application.Run(application.MainWindow);
            }
            while (Restart);
        }

        /// <summary>
        /// Sets the Current Game to the passed Steam Game
        /// </summary>
        /// <param name="steamGame">Steam Game to select</param>
        public static void SelectSteamGame(SteamGame steamGame)
        {
            if (steamGame == null)
                return;
            StartDirectory = steamGame.RootDirectory;
            foreach(var game in Games.GetSupportedGames())
            {
                if (game.AppID == steamGame.GameID)
                {
                    CurrentGame = game;
                }
            }
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
            if (!CurrentGame.SupportsCPKREDIR)
            {
                InstallOtherLoader(install.Value);
                return;
            }
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
                buffer = install == true ? IMAGEHLP : CPKREDIR;

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
            string DLLFileName = Path.Combine(StartDirectory, $"d3d{CurrentGame.DirectXVersion}.dll");

            if (File.Exists(DLLFileName) && toggle)
            {
                File.Delete(DLLFileName);
                return;
            }

            if(HasInternet())
            {
                // Downloads the Loader
                var downloader = new DownloadWindow($"Downloading {CurrentGame.CustomLoaderName}", CurrentGame.ModLoaderDownloadURL, DLLFileName);
                downloader.Start();
            }
            else
            {
                var loader = CurrentGame.ModLoaderData;
                if (loader != null)
                    File.WriteAllBytes(DLLFileName, loader);
                else
                    throw new NotImplementedException("No Loader is available!");
            }
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

        public static bool HasInternet()
        {
            return new Ping().Send("8.8.8.8", 1000).Status == IPStatus.Success;
        }

        private static string GetCPKREDIRVersion()
        {
            var temp = Path.GetTempFileName();
            File.WriteAllBytes(temp, HMMResources.DAT_CPKREDIR_DLL);
            var info = FileVersionInfo.GetVersionInfo(temp);
            File.Delete(temp);
            return $"{info.ProductName} v{info.FileVersion}";
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
    }
}
