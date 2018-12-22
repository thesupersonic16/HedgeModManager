using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            SteamGames = Steam.SearchForGames("Sonic Generations");
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
            var installed = BoyerMooreSearch(data, CPKREDIR) > 0;

            data = null;
            return installed;
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
            byte[] rdata = Encoding.ASCII.GetBytes(".rdata");
            byte[] buffer = install ? IMAGEHLP : CPKREDIR;
            byte[] buff = new byte[0x300 - 0x160];
            Array.Copy(data, 0x160, buff, 0, buff.Length);
            offset = BoyerMooreSearch(buff, rdata) + 0x160;

            int size = BitConverter.ToInt32(data, offset + 0x10);
            int offset_ = BitConverter.ToInt32(data, offset + 0x14);
            buff = new byte[size];
            Array.Copy(data, offset_, buff, 0, buff.Length);
            offset = BoyerMooreSearch(buff, install ? IMAGEHLP : CPKREDIR) + offset_;
            using (var stream = File.OpenWrite(executeablePath))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Write(install ? CPKREDIR : IMAGEHLP, 0, CPKREDIR.Length);
            }
        }

        public static bool CompareArray(byte[] src1, int src1Pos, byte[] src2, int src2Pos, int size)
        {
            for (int i = 0; i < size; ++i)
                if (src1[src1Pos + i] != src2[src2Pos + i])
                    return false;
            return true;
        }

        static int BoyerMooreSearch(byte[] haystack, byte[] needle)
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
    }
}
