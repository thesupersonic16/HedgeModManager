using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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


        }
    }
}
