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
            // Find home folder
            string home = null;
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
            }

            // Return if home folder is not found
            if (home == null)
                return null;

            string installedFilePath = Path.Combine(home, ".config", "legendary", "installed.json");
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
                    games.Add(new GameInstall(game, Path.GetDirectoryName(fullPath), GameLauncher.Heroic));
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
                var installation = launcherInstalled.InstallationList.FirstOrDefault(x =>
                    x.AppName.Equals(game.EGSID, StringComparison.OrdinalIgnoreCase));

                if (installation == null)
                    continue;

                string gamePath = game.GamePathEGS == String.Empty ? game.GamePath : game.GamePathEGS;

                string fullPath = Path.Combine(installation.InstallLocation, gamePath.Substring(gamePath.IndexOf('\\') + 1));

                if (File.Exists(fullPath))
                    games.Add(new GameInstall(game, Path.GetDirectoryName(fullPath), GameLauncher.Epic));
            }

            return games;
        }
    }
}