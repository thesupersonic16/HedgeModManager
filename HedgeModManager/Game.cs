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
        //                                                     Game Name         Custom  CPKREDIR DX ModLoader Download URL    ModLoader Data
        public static Game Unknown              = new Game();
        public static Game SonicGenerations     = new Game("Sonic Generations"  , true  , true  , 9 , Resources.URL_GCL_DL, Resources.DAT_GCL_DLL);
        public static Game SonicLostWorld       = new Game("Sonic Lost World"   , false , true  , 9 , null, null);
        public static Game SonicForces          = new Game("Sonic Forces"       , true  , false , 11, Resources.URL_FML_DL, Resources.DAT_FML_DLL);
    }

    public class Game
    {
        public string GameName = "Unnamed Game";
        public string ModLoaderDownloadURL = "";
        public byte[] ModLoaderData = null;
        public int DirectXVersion = 0;
        public bool HasCustomLoader = false;
        public bool SupportsCPKREDIR = false;

        public Game()
        {

        }

        public Game(string gameName, bool hasCustomLoader, bool supportsCPKREDIR, int directXVersion, string modLoaderDownloadURL, byte[] modLoaderData)
        {
            GameName               = gameName;
            HasCustomLoader        = hasCustomLoader;
            SupportsCPKREDIR       = supportsCPKREDIR;
            DirectXVersion         = directXVersion;
            ModLoaderDownloadURL   = modLoaderDownloadURL;
            ModLoaderData          = modLoaderData;
        }

        public override string ToString()
        {
            return GameName;
        }

    }
}
