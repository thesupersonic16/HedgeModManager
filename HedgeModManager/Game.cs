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
        
        // For GUI use
        public static Game AddGame { get; set; } = new Game()
        {
            GameName = "AddGame",
        };

        public static Game SonicGenerations = new Game()
        {
            GameName = "SonicGenerations",
            SaveName = "cpkredir.sav",
            SupportsCPKREDIR = true,
            SupportsSaveRedirection = true,
            Folders = ["disk/bb", "disk/bb2", "disk/bb3"],
            AppID = "71340",
            GBProtocol = "hedgemmgens",
            Is64Bit = false,
            ModLoader = ModLoaders.HE1ModLoader,
            CodesURL = Resources.URL_BLUEBLUR_CODES,
            GamePaths = [Path.Combine("Sonic Generations", "SonicGenerations.exe")],
            Timestamps = [0x4ED631A1]
        };

        public static Game SonicLostWorld = new Game()
        {
            GameName = "SonicLostWorld",
            SaveName = "cpkredir.sav",
            SupportsCPKREDIR = true,
            SupportsSaveRedirection = true,
            Folders = ["disk/sonic2013_patch_0"],
            AppID = "329440",
            GBProtocol = "hedgemmlw",
            Is64Bit = false,
            ModLoader = ModLoaders.HE1ModLoader,
            CodesURL = Resources.URL_SONIC2013_CODES,
            GamePaths = [Path.Combine("Sonic Lost World", "slw.exe")],
            Timestamps = [0x5677710B]
        };

        // TODO: Change SaveName to "savedata.xml" once we have code to transfer fallback saves to the new name.
        public static Game SonicForces = new Game()
        {
            GameName = "SonicForces",
            SaveName = "cpkredir.sav",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = ["disk/wars_patch"],
            AppID = "637100",
            GBProtocol = "hedgemmforces",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoaderD3D11,
            CodesURL = Resources.URL_WARS_CODES,
            GamePaths = [Path.Combine("SonicForces", "build", "main", "projects", "exec", "Sonic Forces.exe")]
        };

        public static Game PuyoPuyoTetris2 = new Game()
        {
            GameName = "PuyoPuyoTetris2",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            Folders = ["raw"],
            AppID = "1259790",
            GBProtocol = "hedgemmtenpex",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_TENPEX_CODES,
            GamePaths = [Path.Combine("PuyoPuyoTetris2", "PuyoPuyoTetris2.exe")]
        };

        public static Game Tokyo2020 = new Game()
        {
            GameName = "Tokyo2020",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            Folders = ["musashi_0"],
            AppID = "981890",
            GBProtocol = "hedgemmmusashi",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_MUSASHI_CODES,
            GamePaths = [Path.Combine("Tokyo2020", "musashi.exe")]
        };

        public static Game SonicColorsUltimate = new Game()
        {
            GameName = "SonicColorsUltimate",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = false,
            Folders = ["PCCriPak"],
            AppID = "2055290",
            EGSID = "e5071e19d08c45a6bdda5d92fbd0a03e",
            GBProtocol = "hedgemmrainbow",
            Is64Bit = true,
            ModLoader = ModLoaders.RainbowModLoader,
            CodesURL = Resources.URL_RAINBOW_CODES,
            GamePaths = [
                Path.Combine("SonicColorsUltimate", "exec", "SonicColorsUltimate.exe"),
                Path.Combine("SonicColorsUltimate", "rainbow Shipping", "Sonic Colors - Ultimate.exe")
            ]
        };

        public static Game SonicOrigins = new Game()
        {
            GameName = "SonicOrigins",
            SaveName = "savedata",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = ["raw", "Sonic1u", "Sonic2u", "Sonic3ku", "SonicCDu"],
            AppID = "1794960",
            EGSID = "5070a8e44cf74ba3b9a4ca0c0dce5cf1",
            GBProtocol = "hedgemmhite",
            Is64Bit = true,
            ModLoader = ModLoaders.HiteModLoader,
            CodesURL = Resources.URL_HITE_CODES,
            GamePaths = [Path.Combine("SonicOrigins", "build", "main", "projects", "exec", "SonicOrigins.exe")],
            Timestamps = [0x65041AFB]
        };

        public static Game SonicFrontiers = new Game()
        {
            GameName = "SonicFrontiers",
            SaveName = "savedata",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = ["raw"],
            AppID = "1237320",
            EGSID = "c5ca98fa240c4eb796835f97126df8e7",
            GBProtocol = "hedgemmrangers",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader,
            CodesURL = Resources.URL_RANGERS_CODES,
            GamePaths = [Path.Combine("SonicFrontiers", "SonicFrontiers.exe")],
            Timestamps = [0x65510C9C]
        };

        // TODO: implement loader.
        public static Game SonicGenerations2024 = new Game()
        {
            GameName = "SonicGenerations2024",
            ModsDirectoryName = "mods_sonic",
            SaveName = "savedata",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = ["raw"],
            AppID = "2513280",
            EGSID = "a88805d3fbec4ca9bfc248105f6adb0a",
            GBProtocol = "hedgemmmillersonic",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoader, // TODO: use HE1ML?
            CodesURL = Resources.URL_MILLER_SONIC_CODES,
            GamePaths = [Path.Combine("SONIC_X_SHADOW_GENERATIONS", "SONIC_GENERATIONS.exe")],
            Timestamps = [0x66F6109A]
        };

        public static Game ShadowGenerations = new Game()
        {
            GameName = "ShadowGenerations",
            ModsDirectoryName = "mods_shadow",
            SaveName = "savedata",
            SupportsCPKREDIR = false,
            SupportsSaveRedirection = true,
            Folders = ["raw"],
            AppID = "2513280",
            EGSID = "a88805d3fbec4ca9bfc248105f6adb0a",
            GBProtocol = "hedgemmmillershadow",
            Is64Bit = true,
            ModLoader = ModLoaders.HE2ModLoaderD3D11,
            CodesURL = Resources.URL_MILLER_SHADOW_CODES,
            GamePaths = [Path.Combine("SONIC_X_SHADOW_GENERATIONS", "SONIC_X_SHADOW_GENERATIONS.exe")],
            Timestamps = [0x66F609C2, 0x66F55857]
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
            // yield return SonicGenerations2024;
            yield return ShadowGenerations;
        }
    }

    internal static class EmbeddedLoaders
    {
        public static byte[] HE1ModLoader;
        public static byte[] HE2ModLoader;
        public static byte[] RainbowModLoader;

        static EmbeddedLoaders()
        {
            using (var stream = new MemoryStream(Resources.DAT_LOADERS_ZIP))
            using (var zip = new ZipArchive(stream))
            {
                HE1ModLoader = GetFile("HE1ML.dll");
                HE2ModLoader = GetFile("HE2ModLoader.dll");
                RainbowModLoader = GetFile("RainbowModLoader.dll");

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
        public string SaveName = string.Empty;
        public string ModsDirectoryName = "mods";
        public ModLoader ModLoader = null;
        public bool SupportsCPKREDIR = false;
        public bool SupportsSaveRedirection = false;
        public string[] Folders = { "raw" };
        public string AppID = "0";
        public string EGSID = null;
        public string GBProtocol;
        public bool Is64Bit = false;
        public string CodesURL;
        public string[] GamePaths = [];
        public uint[] Timestamps = null;

        public override string ToString() => Localise("Game" + GameName, GameName);
    }

    public class GameInstall
    {
        public static GameInstall Unknown = new GameInstall(Games.Unknown, null, null, GameLauncher.None);

        public Game Game;
        public string GameDirectory;
        public string ExecutablePath;
        public GameLauncher Launcher;
        public bool ShowLauncher = false;
        public bool IsCustom { get; set; } = false;

        public string GameName => GetGameTitle();
        public Uri GameImage { get { return HedgeApp.GetResourceUri($"Resources/Graphics/Games/{Game?.GameName}.png"); } }
        public bool IsAddGame => Game == Games.AddGame;

        public GameInstall(Game game, string directory, string executablePath, GameLauncher launcher, bool custom = false)
        {
            Game = game;
            if (string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(executablePath))
                GameDirectory = Path.GetDirectoryName(executablePath);
            else
                GameDirectory = directory;
            ExecutablePath = executablePath;
            Launcher = launcher;
            IsCustom = custom;
        }

        public void StartGame(bool useLauncher = true, string startDirectory = null)
        {
            if (string.IsNullOrEmpty(startDirectory))
                startDirectory = GameDirectory;

            if (useLauncher)
            {
                switch (Launcher)
                {
                    case GameLauncher.Steam:
                        HedgeApp.StartURL($"steam://run/{Game.AppID}", true);
                        break;
                    case GameLauncher.Epic:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = $"com.epicgames.launcher://apps/{Game.EGSID}?action=launch&silent=true",
                            UseShellExecute = true
                        });
                        break;
                    case GameLauncher.Heroic:
                        HedgeApp.StartURL($"heroic://launch/{Game.EGSID}", true);
                        break;
                    default:
                        Process.Start(new ProcessStartInfo(ExecutablePath)
                        {
                            WorkingDirectory = startDirectory
                        });
                        break;
                }
            }
            else
            {
                Process.Start(new ProcessStartInfo(ExecutablePath)
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
            if (!string.IsNullOrEmpty(RegistryConfig.CustomGames))
            {
                foreach (string entry in RegistryConfig.CustomGames.Split(';'))
                {
                    string[] split = entry.Split('|');
                    // Should follow the format below, can be a directory at this point for compatibility
                    // e.g. "A:\Games\Sonic Generations\SonicGenerations.exe|SonicGenerations|Steam"
                    string path = split[0];
                    var game = Games.Unknown;
                    var launcher = GameLauncher.None;

                    if (split.Length > 1)
                        game = Games.GetSupportedGames().FirstOrDefault(x => x.GameName == split[1]);

                    if (split.Length > 2)
                        Enum.TryParse(split[2], out launcher);

                    // Guess game for compatibility
                    if (game == Games.Unknown && Directory.Exists(path))
                    {
                        game = Games.GetSupportedGames().FirstOrDefault(x =>
                        {
                            foreach (string gamePath in x.GamePaths)
                            {
                                if (File.Exists(Path.Combine(path, gamePath)))
                                    return true;
                            }
                            return false;
                        });
                        path = Path.Combine(path, game.GamePaths[0]);
                    }

                    if (File.Exists(path))
                        games.Add(new GameInstall(game, null, path, launcher, true));
                }
            }

            HandleGameInstallDuplicates(games);

            return !string.IsNullOrEmpty(preference)
                ? games.OrderBy(x => x.Game.GameName != preference).ToList()
                : games;
        }

        public static void HandleGameInstallDuplicates(List<GameInstall> games)
        {
            // Reset launcher visibility
            games.ForEach(game => game.ShowLauncher = false);

            // Show launcher for duplicates
            foreach (var game in games.GroupBy(t => t.Game).Where(t => t.Count() > 1).Select(t => t.Key))
            {
                foreach (var install in games.Where(t => t.Game == game))
                {
                    install.ShowLauncher = true;
                }
            }

            // Remove duplicates with same path
            foreach (var game in games.GroupBy(t => t.ExecutablePath).Where(t => t.Count() > 1).Select(t => t.Key))
            {
                var installs = games.Where(t => t.ExecutablePath == game).ToList();
                for (int i = installs.Count - 1; i > 0; i--)
                    games.Remove(installs[i]);
            }
        }

        public string GetGameTitle()
        {
            string title = Localise("Game" + Game.GameName, Game.GameName);

            if (ShowLauncher)
            {
                if (Launcher == GameLauncher.None)
                    title += $" ({Path.GetFileNameWithoutExtension(ExecutablePath)})";
                else
                    title += $" ({Localise("Launcher" + Launcher)})";
            }

            return title;
        }

        public override string ToString() => Localise("Game" + Game.GameName, Game.GameName);
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
