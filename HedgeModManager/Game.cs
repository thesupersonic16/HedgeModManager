using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HedgeModManager.Lang;

namespace HedgeModManager
{
    public static class Games
    {
        public const uint CodeLoaderMinCodeVersionStringId = 101;
        public const uint CodeLoaderMaxCodeVersionStringId = 102;
        public const string EmbeddedCPKREDIRVersion = "0.5.0.8";

        public static Game Unknown = new Game();
        public static Game SonicGenerations = new Game()
        {
            GameName = "SonicGenerations",
            ExecutableName = "SonicGenerations.exe",
            SupportsCPKREDIR = true,
            SupportsSaveRedirection = true,
            AppID = "71340",
            GBProtocol = "hedgemmgens",
            Is64Bit = false,
            ModLoader = ModLoaders.GenerationsCodeLoader,
            CodesURL = Resources.URL_GCL_CODES,
            SteamGamePath = Path.Combine("Sonic Generations", "SonicGenerations.exe")
        };

        public static Game SonicLostWorld = new Game()
        {
            GameName = "SonicLostWorld",
            ExecutableName = "slw.exe",
            SupportsCPKREDIR = true,
            SupportsSaveRedirection = true,
            AppID = "329440",
            GBProtocol = "hedgemmlw",
            Is64Bit = false,
            ModLoader = ModLoaders.LostCodeLoader,
            CodesURL = Resources.URL_LCL_CODES,
            SteamGamePath = Path.Combine("Sonic Lost World", "slw.exe")
        };

        public static Game SonicForces = new Game()
        {
            GameName = "SonicForces",
            ExecutableName = "Sonic Forces.exe",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            AppID = "637100",
            GBProtocol = "hedgemmforces",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_FML_CODES,
            SteamGamePath = Path.Combine("SonicForces", "build", "main", "projects", "exec", "Sonic Forces.exe")
        };

        public static Game PuyoPuyoTetris2 = new Game()
        {
            GameName = "PuyoPuyoTetris2",
            ExecutableName = "PuyoPuyoTetris2.exe",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            AppID = "1259790",
            GBProtocol = "hedgemmtenpex",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_TML_CODES,
            SteamGamePath = Path.Combine("PuyoPuyoTetris2", "PuyoPuyoTetris2.exe")
        };

        public static Game Tokyo2020 = new Game()
        {
            GameName = "Tokyo2020",
            ExecutableName = "musashi.exe",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            AppID = "981890",
            GBProtocol = "hedgemmmusashi",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_MML_CODES,
            SteamGamePath = Path.Combine("Tokyo2020", "musashi.exe")
        };

        public static IEnumerable<Game> GetSupportedGames()
        {
            yield return SonicGenerations;
            yield return SonicLostWorld;
            yield return SonicForces;
            yield return PuyoPuyoTetris2;
            yield return Tokyo2020;
        }
    }

    internal static class EmbeddedLoaders
    {
        public static byte[] GenerationsCodeLoader;
        public static byte[] LostCodeLoader;
        public static byte[] HE2ModLoader;

        static EmbeddedLoaders()
        {
            using (var stream = new MemoryStream(Resources.DAT_LOADERS_ZIP))
            using (var zip = new ZipArchive(stream))
            {
                GenerationsCodeLoader = GetFile("SonicGenerationsCodeLoader.dll");
                LostCodeLoader = GetFile("LostCodeLoader.dll");
                HE2ModLoader = GetFile("HE2ModLoader.dll");

                byte[] GetFile(string name)
                {
                    var entry = zip.GetEntry(name);
                    using (var file = entry.Open())
                    {
                        var buffer = new byte[entry.Length];
                        file.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
            }
        }
    }

    public class Game
    {
        public string GameName = "Unnamed Game";
        public string ExecutableName = string.Empty;
        public ModLoader ModLoader = null;
        public bool SupportsCPKREDIR = false;
        public bool SupportsSaveRedirection = false;
        public string AppID = "0";
        public string GBProtocol;
        public bool Is64Bit = false;
        public string CodesURL;
        public string SteamGamePath = string.Empty;

        public override string ToString() => Localise("Game" + GameName, GameName);
    }

    public class GameInstall
    {
        public Game BaseGame;
        public string GameDirectory;

        public GameInstall(Game game, string directory)
        {
            BaseGame = game;
            GameDirectory = directory;
        }

        public override string ToString() => Localise("Game" + BaseGame.GameName, BaseGame.GameName);
    }

    public class CodeLoaderInfo
    {
        public Version LoaderVersion { get; set; }
        public Version MinCodeVersion { get; set; }
        public Version MaxCodeVersion { get; set; }

        public CodeLoaderInfo(Version loader)
        {
            LoaderVersion = loader;
        }

        public CodeLoaderInfo(Version loader, Version minCode) : this(loader)
        {
            MinCodeVersion = minCode;
        }

        public CodeLoaderInfo(Version loader, Version minCode, Version maxCode) : this(loader, minCode)
        {
            MaxCodeVersion = maxCode;
        }
    }
}
