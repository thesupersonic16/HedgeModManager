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
        public class Installation
        {
            public string InstallLocation { get; set; }
            public string NamespaceId { get; set; }
            public string ItemId { get; set; }
            public string ArtifactId { get; set; }
            public string AppVersion { get; set; }
            public string AppName { get; set; }
        }

        public class LauncherInstalled
        {
            public List<Installation> InstallationList { get; set; }
        }

        public static List<GameInstall> SearchForGames()
        {
            var eos = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Epic Games\\EOS");

            if (eos?.GetValue("ModSdkMetadataDir") is not string modSdkMetadataDir)
                return null;

            string launcherInstalledFilePath = Path.Combine(modSdkMetadataDir, 
                "..", "..", "..", "UnrealEngineLauncher", "LauncherInstalled.dat");

            LauncherInstalled launcherInstalled;

            try
            {
                launcherInstalled = JsonConvert.DeserializeObject<LauncherInstalled>(File.ReadAllText(launcherInstalledFilePath));
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
                    x.AppName.Equals(game.AppID, StringComparison.OrdinalIgnoreCase));

                if (installation == null)
                    continue;

                string fullPath = Path.Combine(installation.InstallLocation, game.GamePath);

                if (File.Exists(fullPath))
                    games.Add(new GameInstall(game, Path.GetDirectoryName(fullPath)));
            }

            return games;
        }
    }
}