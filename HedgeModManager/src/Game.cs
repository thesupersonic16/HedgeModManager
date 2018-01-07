using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{

    public class Games
    {
        public static Game Unknown          = new Game();
        public static Game SonicGenerations = new Game("Sonic Generations"  , true  , 9 , Resources.SonicGenerationsCodeLoaderURL, Resources.SonicGenerationsCodeLoaderHashURL, Resources.SonicGenerationsCodeLoader);
        public static Game SonicLostWorld   = new Game("Sonic Lost World"   , false , 9 , null                                   , null                                       , null);
        public static Game SonicForces      = new Game("Sonic Forces"       , true  , 11, Resources.ForcesModLoaderURL           , Resources.ForcesModLoaderHashURL           , Resources.ForcesModLoader);
    }

    public class Game
    {
        public string GameName = "Unnamed Game";
        public string LoaderDownloadURL = "";
        public string LoaderHashURL = "";
        public byte[] LoaderFile = null;
        public byte[] Hash = null;
        public byte DirectXVersion = 0;
        public bool HasCodes = false;

        public Game()
        {

        }

        public Game(string gameName, bool hasCodes, byte directXVersion, string loaderDownloadURL, string loaderHashURL, byte[] loaderFile)
        {
            GameName = gameName;
            HasCodes = hasCodes;
            DirectXVersion = directXVersion;
            LoaderDownloadURL = loaderDownloadURL;
            LoaderHashURL = loaderHashURL;
            LoaderFile = loaderFile;
        }

        public override string ToString()
        {
            return GameName;
        }

    }
}
