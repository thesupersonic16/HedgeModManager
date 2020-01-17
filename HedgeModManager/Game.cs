using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{

    public static class Games
    {
        public static Game Unknown = new Game();
        public static Game SonicGenerations = new Game()
        {
            GameName = "Sonic Generations",
            ExecuteableName = "SonicGenerations.exe",
            HasCustomLoader = true,
            SupportsCPKREDIR = true,
            ModLoaderDownloadURL = Resources.URL_GCL_DL,
            ModLoaderData = Resources.DAT_GCL_DLL,
            CustomLoaderName = "Generations Code Loader",
            AppID = "71340",
            DirectXVersion = 9,
            GBProtocol = "hedgemmgens",
            Is64Bit = false,
            CodesURL = Resources.URL_GCL_CODES
        };

        public static Game SonicLostWorld = new Game()
        {
            GameName = "Sonic Lost World",
            ExecuteableName = "slw.exe",
            HasCustomLoader = true,
            SupportsCPKREDIR = true,
            ModLoaderDownloadURL = Resources.URL_LCL_DL,
            ModLoaderData = Resources.DAT_LCL_DLL,
            CustomLoaderName = "Lost Code Loader",
            AppID = "329440",
            DirectXVersion = 9,
            GBProtocol = "hedgemmlw",
            Is64Bit = false,
            CodesURL = Resources.URL_LCL_CODES
        };

        public static Game SonicForces = new Game()
        {
            GameName = "Sonic Forces",
            ExecuteableName = "Sonic Forces.exe",
            HasCustomLoader = true,
            SupportsCPKREDIR = false,
            ModLoaderDownloadURL = Resources.URL_FML_DL,
            ModLoaderData = Resources.DAT_FML_DLL,
            CustomLoaderName = "Forces Mod Loader",
            AppID = "637100",
            DirectXVersion = 11,
            GBProtocol = "hedgemmforces",
            Is64Bit = true,
            CodesURL = Resources.URL_FML_CODES
        };

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
        public string AppID = "0";
        public string GBProtocol;
        public bool Is64Bit = false;
        public string CodesURL;

        public override string ToString()
        {
            return GameName;
        }
    }
}
