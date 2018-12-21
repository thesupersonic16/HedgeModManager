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

        [STAThread]
        public static void Main()
        {
            // Use TLSv1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

#if DEBUG
            Steam.Init();
            var games = Steam.SearchForGames("Sonic Forces");
            if (games.Count != 0)
                StartDirectory = games[0].RootDirectory;
#endif


            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}
