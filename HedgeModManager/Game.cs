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
        //                                                     Game Name          Executeable Name       Custom  CPKREDIR DX ModLoader Download URL    ModLoader Data
        public static Game Unknown              = new Game();
        public static Game SonicGenerations     = new Game("Sonic Generations"  , "SonicGenerations.exe", true  , true  , 9 , Resources.URL_GCL_DL, Resources.DAT_GCL_DLL, "Generations Code Loader");
        public static Game SonicLostWorld       = new Game("Sonic Lost World"   , "slw.exe"             , false , true  , 9 , null, null, "None");
        public static Game SonicForces          = new Game("Sonic Forces"       , "Sonic Forces.exe"    , true  , false , 11, Resources.URL_FML_DL, Resources.DAT_FML_DLL, "Forces Mod Loader");

        public static IEnumerable<Game> GetSupportedGames()
        {
            yield return SonicGenerations;
            yield return SonicLostWorld;
            yield return SonicForces;
        }
    }

    public class Game
    {
        public string GameName = "Unnamed Game";
        public string ExecuteableName = string.Empty;
        public string ModLoaderDownloadURL = "";
        public byte[] ModLoaderData = null;
        public int DirectXVersion = 0;
        public bool HasCustomLoader = false;
        public bool SupportsCPKREDIR = false;
        public string CustomLoaderName = "None";
        public Game()
        {

        }

        public Game(string gameName, string executeableName, bool hasCustomLoader, bool supportsCPKREDIR, int directXVersion, string modLoaderDownloadURL, byte[] modLoaderData, string modLoaderName)
        {
            GameName               = gameName;
            ExecuteableName        = executeableName;
            HasCustomLoader        = hasCustomLoader;
            SupportsCPKREDIR       = supportsCPKREDIR;
            DirectXVersion         = directXVersion;
            ModLoaderDownloadURL   = modLoaderDownloadURL;
            ModLoaderData          = modLoaderData;
            CustomLoaderName = modLoaderName;
        }

        public override string ToString()
        {
            return GameName;
        }

    }
}
