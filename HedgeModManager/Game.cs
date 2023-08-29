using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            SaveName = "cpkredir.sav",
            SupportsCPKREDIR = true,
            SupportsSaveRedirection = true,
            Folders = new [] { "disk/bb", "disk/bb2", "disk/bb3" },
            AppID = "71340",
            GBProtocol = "hedgemmgens",
            Is64Bit = false,
            ModLoader = ModLoaders.HE1ModLoader,
            CodesURL = Resources.URL_BLUEBLUR_CODES,
            GamePath = Path.Combine("Sonic Generations", "SonicGenerations.exe")
        };

        public static Game SonicLostWorld = new Game()
        {
            GameName = "SonicLostWorld",
            ExecutableName = "slw.exe",
            SaveName = "cpkredir.sav",
            SupportsCPKREDIR = true,
            SupportsSaveRedirection = true,
            Folders = new[] { "disk/sonic2013_patch_0" },
            AppID = "329440",
            GBProtocol = "hedgemmlw",
            Is64Bit = false,
            ModLoader = ModLoaders.HE1ModLoader,
            CodesURL = Resources.URL_SONIC2013_CODES,
            GamePath = Path.Combine("Sonic Lost World", "slw.exe")
        };

        // TODO: Change SaveName to "savedata.xml" once we have code to transfer fallback saves to the new name.
        public static Game SonicForces = new Game()
        {
            GameName = "SonicForces",
            ExecutableName = "Sonic Forces.exe",
            SaveName = "cpkredir.sav",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = new[] { "disk/wars_patch" },
            AppID = "637100",
            GBProtocol = "hedgemmforces",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_WARS_CODES,
            GamePath = Path.Combine("SonicForces", "build", "main", "projects", "exec", "Sonic Forces.exe")
        };

        public static Game PuyoPuyoTetris2 = new Game()
        {
            GameName = "PuyoPuyoTetris2",
            ExecutableName = "PuyoPuyoTetris2.exe",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            Folders = new[] { "raw" },
            AppID = "1259790",
            GBProtocol = "hedgemmtenpex",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_TENPEX_CODES,
            GamePath = Path.Combine("PuyoPuyoTetris2", "PuyoPuyoTetris2.exe")
        };

        public static Game Tokyo2020 = new Game()
        {
            GameName = "Tokyo2020",
            ExecutableName = "musashi.exe",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            Folders = new[] { "musashi_0" },
            AppID = "981890",
            GBProtocol = "hedgemmmusashi",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_MUSASHI_CODES,
            GamePath = Path.Combine("Tokyo2020", "musashi.exe")
        };

        public static Game SonicColorsUltimate = new Game()
        {
            GameName = "SonicColorsUltimate",
            ExecutableName = "Sonic Colors - Ultimate.exe",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            Folders = new[] { "PCCriPak" },
            AppID = "2055290",
            EGSID = "e5071e19d08c45a6bdda5d92fbd0a03e",
            GBProtocol = "hedgemmrainbow",
            Is64Bit = true,
            ModLoader = ModLoaders.RainbowModLoader,
            CodesURL = Resources.URL_RAINBOW_CODES,
            GamePath = Path.Combine("SonicColorsUltimate", "exec", "SonicColorsUltimate.exe"),
            GamePathEGS = Path.Combine("SonicColorsUltimate", "rainbow Shipping", "Sonic Colors - Ultimate.exe")
        };

        public static Game SonicOrigins = new Game()
        {
            GameName = "SonicOrigins",
            ExecutableName = "SonicOrigins.exe",
            SaveName = "savedata",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = new[] { "raw", "Sonic1u", "Sonic2u", "Sonic3ku", "SonicCDu" },
            AppID = "1794960",
            EGSID = "5070a8e44cf74ba3b9a4ca0c0dce5cf1",
            GBProtocol = "hedgemmhite",
            Is64Bit = true,
            ModLoader = ModLoaders.HiteModLoader,
            CodesURL = Resources.URL_HITE_CODES,
            GamePath = Path.Combine("SonicOrigins", "build", "main", "projects", "exec", "SonicOrigins.exe")
        };

        public static Game SonicFrontiers = new Game()
        {
            GameName = "SonicFrontiers",
            ExecutableName = "SonicFrontiers.exe",
            SaveName = "savedata",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = new[] { "raw" },
            AppID = "1237320",
            GBProtocol = "hedgemmrangers",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_RANGERS_CODES,
            GamePath = Path.Combine("SonicFrontiers", "SonicFrontiers.exe")
        };

        public static IEnumerable<Game> GetSupportedGames()
        {
            yield return SonicGenerations;
            yield return SonicLostWorld;
            yield return SonicForces;
            yield return PuyoPuyoTetris2;
            yield return Tokyo2020;
            yield return SonicColorsUltimate;
            yield return SonicOrigins;
            yield return SonicFrontiers;
        }
    }

    internal static class EmbeddedLoaders
    {
        public static byte[] HE1ModLoader;
        public static byte[] HE2ModLoader;
        public static byte[] RainbowModLoader;
        public static byte[] HiteModLoader;

        static EmbeddedLoaders()
        {
            using (var stream = new MemoryStream(Resources.DAT_LOADERS_ZIP))
            using (var zip = new ZipArchive(stream))
            {
                HE1ModLoader = GetFile("HE1ML.dll");
                HE2ModLoader = GetFile("HE2ModLoader.dll");
                RainbowModLoader = GetFile("RainbowModLoader.dll");
                HiteModLoader = GetFile("HiteModLoader.dll");

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
        public string GameName = "NoGame";
        public string ExecutableName = string.Empty;
        public string SaveName = string.Empty;
        public ModLoader ModLoader = null;
        public bool SupportsCPKREDIR = false;
        public bool SupportsSaveRedirection = false;
        public string[] Folders = { "raw" };
        public string AppID = "0";
        public string EGSID = null;
        public string GBProtocol;
        public bool Is64Bit = false;
        public string CodesURL;
        public string GamePath = string.Empty;
        public string GamePathEGS = string.Empty;

        public override string ToString() => Localise("Game" + GameName, GameName);
    }

    public class GameInstall
    {
        public Game BaseGame;
        public string GameDirectory;
        public GameLauncher Launcher;
        public bool ShowLauncher = false;

        public string GameName { get { return Localise("Game" + BaseGame?.GameName, BaseGame?.GameName) + (ShowLauncher ? $" ({Localise("Launcher" + Launcher)})" : ""); } }
        public Uri GameImage { get { return HedgeApp.GetResourceUri($"Resources/Graphics/Games/{BaseGame?.GameName}.png"); } }

        public GameInstall(Game game, string directory, GameLauncher launcher)
        {
            BaseGame = game;
            GameDirectory = directory;
            Launcher = launcher;
        }

        public void StartGame(bool useLauncher = true, string startDirectory = null)
        {
            if (string.IsNullOrEmpty(startDirectory))
                startDirectory = HedgeApp.StartDirectory;

            if (useLauncher)
            {
                switch (Launcher)
                {
                    case GameLauncher.Steam:
                        HedgeApp.StartURL($"steam://run/{BaseGame.AppID}", true);
                        break;
                    case GameLauncher.Epic:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = $"com.epicgames.launcher://apps/{BaseGame.EGSID}?action=launch&silent=true",
                            UseShellExecute = true
                        });
                        break;
                    case GameLauncher.Heroic:
                        HedgeApp.StartURL($"heroic://launch/{BaseGame.EGSID}", true);
                        break;
                    default:
                        Process.Start(new ProcessStartInfo(Path.Combine(startDirectory, BaseGame.ExecutableName))
                        {
                            WorkingDirectory = startDirectory
                        });
                        break;
                }
            }
            else
            {
                Process.Start(new ProcessStartInfo(Path.Combine(startDirectory, BaseGame.ExecutableName))
                {
                    WorkingDirectory = startDirectory
                });
            }
        }

        public static List<GameInstall> SearchForGames(string preference = null)
        {
            var steamGames = Steam.SearchForGames();
            var epicGames = Epic.SearchForGames();

            var games = new List<GameInstall>();

            if (steamGames != null)
                games.AddRange(steamGames);

            if (epicGames != null)
                games.AddRange(epicGames);

            // Extra directories
            if (!string.IsNullOrEmpty(RegistryConfig.ExtraGameDirectories))
            {
                foreach (string path in RegistryConfig.ExtraGameDirectories.Split(';'))
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var game in Games.GetSupportedGames())
                        {
                            string fullPath = Path.Combine(path, game.ExecutableName);
                            if (File.Exists(fullPath))
                            {
                                games.Add(new GameInstall(game, Path.GetDirectoryName(fullPath), GameLauncher.None));
                            }
                        }
                    }
                }
            }

            // Check for dupes
            foreach (var game in games.GroupBy(t => t.BaseGame).Where(t => t.Count() > 1).Select(t => t.Key))
            {
                foreach (var install in games.Where(t => t.BaseGame == game))
                {
                    install.ShowLauncher = true;
                }
            }

            return !string.IsNullOrEmpty(preference)
                ? games.OrderBy(x => x.BaseGame.GameName != preference).ToList()
                : games;
        }

        public override string ToString() => Localise("Game" + BaseGame.GameName, BaseGame.GameName);
    }

    public enum GameLauncher
    {
        None,
        Steam,
        Epic,
        Heroic
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
