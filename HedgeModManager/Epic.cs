using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace HedgeModManager
{
    public class Epic
    {
        public class EGSInstallation
        {
            public string InstallLocation { get; set; }
            public string NamespaceId { get; set; }
            public string ItemId { get; set; }
            public string ArtifactId { get; set; }
            public string AppVersion { get; set; }
            public string AppName { get; set; }
        }

        public class LegendaryInstallation
        {
            [JsonProperty("app_name")]
            public string AppName { get; set; }
            [JsonProperty("executable")]
            public string Executable { get; set; }
            [JsonProperty("install_path")]
            public string InstallPath { get; set; }
            [JsonProperty("version")]
            public string Version { get; set; }
        }

        public class EGSLauncherInstalled
        {
            public List<EGSInstallation> InstallationList { get; set; }
        }

        public static List<GameInstall> SearchForGames()
        {
            var games = new List<GameInstall>();
            games.AddRange(SearchForGamesEGS() ?? new List<GameInstall>());
            games.AddRange(SearchForGamesHeroic() ?? new List<GameInstall>());
            return games;
        }

        public static List<GameInstall> SearchForGamesHeroic()
        {
            string home = null;
            string appdata = null;
            if (HedgeApp.IsLinux)
            {
                home = Environment.GetEnvironmentVariable("WINEHOMEDIR")?.Replace("\\??\\", "");
                // Prefix "Z:" if starts with "/"
                if (home?.StartsWith("/") == true)
                    home = $"Z:{home}";
            }
            else
            {
                home = Environment.GetEnvironmentVariable("USERPROFILE");
                appdata = Environment.GetEnvironmentVariable("APPDATA");
            }

            // Return if home folder is not found
            if (home == null || appdata == null)
                return null;

            
            string installedFilePath = Path.Combine(appdata, "heroic", "legendaryConfig", "legendary", "installed.json");
            if (!File.Exists(installedFilePath))
                installedFilePath = Path.Combine(home, ".config", "legendary", "installed.json");
            if (!File.Exists(installedFilePath))
                installedFilePath = Path.Combine(home, ".var", "app", "com.heroicgameslauncher.hgl", "config", "heroic", "legendaryConfig", "legendary", "installed.json");
            if (!File.Exists(installedFilePath))
                installedFilePath = Path.Combine(home, ".var", "app", "com.heroicgameslauncher.hgl", "config", "legendary", "installed.json");

            if (!File.Exists(installedFilePath))
                return null;

            var installations = new Dictionary<string, LegendaryInstallation>();

            try
            {
                installations = JsonConvert.DeserializeObject<Dictionary<string, LegendaryInstallation>>(File.ReadAllText(installedFilePath));
            }
            catch
            {
                return null;
            }

            if (installations == null || installations?.Count == 0)
                return null;

            var games = new List<GameInstall>();

            foreach (var game in Games.GetSupportedGames())
            {
                var installation = installations.FirstOrDefault(x => x.Key.Equals(game.EGSID, StringComparison.OrdinalIgnoreCase));

                if (installation.Value == null)
                    continue;

                string fullPath = Path.Combine(installation.Value.InstallPath, installation.Value.Executable);
                
                if (File.Exists(fullPath))
                    games.Add(new GameInstall(game, null, fullPath, GameLauncher.Heroic));
            }

            return games;
        }

        public static List<GameInstall> SearchForGamesEGS()
        {
            var eos = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Epic Games\\EOS");

            if (eos?.GetValue("ModSdkMetadataDir") is not string modSdkMetadataDir)
                return null;

            string launcherInstalledFilePath = Path.Combine(modSdkMetadataDir, 
                "..", "..", "..", "UnrealEngineLauncher", "LauncherInstalled.dat");

            EGSLauncherInstalled launcherInstalled;

            try
            {
                launcherInstalled = JsonConvert.DeserializeObject<EGSLauncherInstalled>(File.ReadAllText(launcherInstalledFilePath));
            }
            catch
            {
                return null;
            }

            if (launcherInstalled == null || launcherInstalled.InstallationList?.Count == 0)
                return null;

            var games = new List<GameInstall>();

            foreach (var game in Games.GetSupportedGames())
            {
                var installation = launcherInstalled.InstallationList
                    .FirstOrDefault(x => x.AppName.Equals(game.EGSID, StringComparison.OrdinalIgnoreCase));

                if (installation == null)
                    continue;

                foreach (string gamePath in game.GamePaths)
                {
                    string path = gamePath;
                    // Remove first folder from path
                    if (path.Contains(Path.DirectorySeparatorChar))
                        path = path.Substring(path.IndexOf(Path.DirectorySeparatorChar) + 1);

                    var fullPath = Path.Combine(installation.InstallLocation, path);
                    if (File.Exists(fullPath))
                        games.Add(new GameInstall(game, null, fullPath, GameLauncher.Epic));
                }
            }

            return games;
        }
    }
}