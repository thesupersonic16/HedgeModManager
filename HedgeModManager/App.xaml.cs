using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string StartDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static Game CurrentGame = Games.Unknown;

        public static List<SteamGame> SteamGames = null;

        public static bool Restart = false;

        public static byte[] CPKREDIR = new byte[] { 0x63, 0x70, 0x6B, 0x72, 0x65, 0x64, 0x69, 0x72 };
        public static byte[] IMAGEHLP = new byte[] { 0x69, 0x6D, 0x61, 0x67, 0x65, 0x68, 0x6C, 0x70 };

        [STAThread]
        public static void Main()
        {
            // Use TLSv1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Steam.Init();
#if DEBUG
            SteamGames = Steam.SearchForGames("Sonic Forces");
#else
            SteamGames = Steam.SearchForGames();
#endif
            SelectSteamGame(SteamGames.FirstOrDefault());

            do
            {
                Restart = false;
                var application = new App();
                application.InitializeComponent();
                application.Run();
            }
            while (Restart);
        }

        public static void SelectSteamGame(SteamGame steamGame)
        {
            if (steamGame == null)
                return;
            StartDirectory = steamGame.RootDirectory;
            if (steamGame.GameID == "329440")
                CurrentGame = Games.SonicLostWorld;
            if (steamGame.GameID == "71340")
                CurrentGame = Games.SonicGenerations;
            if (steamGame.GameID == "637100")
                CurrentGame = Games.SonicForces;
        }

        public static SteamGame GetSteamGame(Game game)
        {
            return SteamGames.FirstOrDefault(t => t.GameName == game.GameName);
        }

        public static bool IsCPKREDIRInstalled(string executeablePath)
        {
            var data = File.ReadAllBytes(executeablePath);

            for(int i = 11918000; i < data.Length; i += 2)
            {
                if (CompareArray(data, i, CPKREDIR, 0, CPKREDIR.Length))
                {
                    data = null;
                    return true;
                }
            }

            data = null;
            return false;
        }

        /// <summary>
        /// Installs or Uninstalls CPKREDIR
        /// </summary>
        /// <param name="executeablePath">Path to the executeable</param>
        /// <param name="install">
        /// TRUE: Installs CPKREDIR (default)
        /// FALSE: Uninstalls CPKREDIR
        /// </param>
        public static void InstallCPKREDIR(string executeablePath, bool install = true)
        {
            File.Copy(executeablePath, $"{executeablePath}.bak", true);

            var data = File.ReadAllBytes(executeablePath);
            var offset = -1;
            for (int i = 11918000; i < data.Length; i += 2)
            {
                if (CompareArray(data, i, CPKREDIR, 0, CPKREDIR.Length) || CompareArray(data, i, IMAGEHLP, 0, IMAGEHLP.Length))
                {
                    offset = i;
                    break;
                }
            }

            if (offset > 0)
            {
                Array.Copy(install ? CPKREDIR : IMAGEHLP, 0, data, offset, CPKREDIR.Length);
                File.WriteAllBytes(executeablePath, data);
            }

            data = null;
        }

        public static bool CompareArray(byte[] src1, int src1Pos, byte[] src2, int src2Pos, int size)
        {
            for (int i = 0; i < size; ++i)
                if (src1[src1Pos + i] != src2[src2Pos + i])
                    return false;
            return true;
        }
    }
}
