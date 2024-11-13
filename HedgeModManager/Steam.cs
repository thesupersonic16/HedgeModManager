using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{

    public static class Steam
    {
        public static string SteamLocation;

        public static void Init()
        {
            // Not sure about OSX
            // Assume Steam is located in the home folder for Linux
            if (HedgeApp.IsLinux)
            {
                string home = Environment.GetEnvironmentVariable("WINEHOMEDIR").Replace("\\??\\", "");
                string steamPath = Path.Combine(home, ".steam/steam");
                if (Directory.Exists(steamPath))
                    SteamLocation = steamPath;
            }

            // Local Machine
            {
                var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                        .OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam");
                if (key == null || key.GetValue("InstallPath") == null)
                {
                    if (key != null)
                        key.Close();

                    key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default)
                    .OpenSubKey("SOFTWARE\\Valve\\Steam");
                }
                if (key != null)
                {
                    if (key.GetValue("InstallPath") is string steamPath && Directory.Exists(steamPath))
                        SteamLocation = steamPath;
                    key.Close();
                }
            }

            // Current User
            {
                var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                .OpenSubKey("Software\\Valve\\Steam");
                if (key != null)
                {
                    if (key.GetValue("SteamPath") is string steamPath && Directory.Exists(steamPath))
                        SteamLocation = steamPath;
                    key.Close();
                }
            }
        }

        public static string GetCachedProfileImageURLFromSID64(string SID64)
        {
            return Path.Combine(SteamLocation, "config/avatarcache/", SID64 + ".png");
        }

        public static string GetProtonPrefixPath(string gameId) =>
            Path.Combine(SteamLocation, $"steamapps\\compatdata\\{gameId}\\pfx");

        public static List<GameInstall> SearchForGames()
        {
            var paths = new List<string>();
            var games = new List<GameInstall>();

            if (string.IsNullOrEmpty(SteamLocation))
                return null;

            string vdfLocation = Path.Combine(SteamLocation, "steamapps\\libraryfolders.vdf");
            Dictionary<string, object> vdf = null;
            try
            {
                vdf = SteamVDF.Load(vdfLocation);
            }
            catch
            {
                return null;
            }

            // Adds all the custom libraries
            var container = SteamVDF.GetContainer(vdf, "LibraryFolders");
            if (container != null)
            {
                foreach (var library in container)
                {
                    if (int.TryParse(library.Key, out int index))
                    {
                        if (library.Value is Dictionary<string, object> libraryInfo)
                            paths.Add(Path.Combine(libraryInfo["path"] as string ?? string.Empty, "steamapps\\common"));
                        else
                            paths.Add(Path.Combine(library.Value as string ?? string.Empty, "steamapps\\common"));
                    }
                }
            }

            foreach (string path in paths)
            {
                string libraryPath = path;

                // Prepend "Z:" if the path starts with "/" on WINE
                if (HedgeApp.IsLinux && libraryPath.StartsWith("/"))
                    libraryPath = $"Z:{libraryPath}";

                if (Directory.Exists(path))
                {
                    foreach (var game in Games.GetSupportedGames())
                    {
                        foreach (string gamePath in game.GamePaths)
                        {
                            var fullPath = Path.Combine(libraryPath, gamePath);
                            if (File.Exists(fullPath))
                                games.Add(new GameInstall(game, null, fullPath, GameLauncher.Steam));
                        }
                    }
                }
            }

            return games;
        }

        public static bool CheckGame(string path)
        {
            return File.Exists(path) && !(
                File.Exists(Path.Combine(Path.GetDirectoryName(path), "steamclient64.dll")) ||
                File.Exists(Path.Combine(Path.GetDirectoryName(path), "steamclient.dll")));
        }

        public static bool CheckDirectory(string path)
        {
            return !(
                File.Exists(Path.Combine(path, "steamclient64.dll")) ||
                File.Exists(Path.Combine(path, "steamclient.dll")));
        }
    }


    public static class SteamVDF
    {
        // Methods
        public static string GetProperty(
            Dictionary<string, object> containers, string name)
        {
            foreach (var value in containers)
            {
                if (value.Key == name)
                {
                    return value.Value as string;
                }
            }

            return null;
        }

        public static Dictionary<string, object> GetContainer(
            Dictionary<string, object> containers, string name)
        {
            foreach (var value in containers)
            {
                if (string.Compare(value.Key, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return value.Value as Dictionary<string, object>;
                }
            }

            return null;
        }

        public static Dictionary<string, object> Load(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                return Load(fs);
            }
        }

        public static Dictionary<string, object> Load(Stream fileStream)
        {
            var defs = new Dictionary<string, object>();
            var reader = new StreamReader(fileStream, true);

            string line, str = "", nm = "";
            bool doReadString = false;
            char c;

            return ReadContainers() ?? new Dictionary<string, object>();

            // Sub-Methods
            Dictionary<string, object> ReadContainers()
            {
                List<Dictionary<string, object>> containers = new List<Dictionary<string, object>>();
                containers.Add(new Dictionary<string, object>());
                string name = "";
                nm = str = "";

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    doReadString = false;

                    for (int i = 0; i < line.Length; ++i)
                    {
                        c = line[i];
                        if (c == '"')
                        {
                            doReadString = !doReadString;

                            if (doReadString)
                            {
                                continue;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(nm))
                                {
                                    nm = str;
                                    str = "";
                                }
                                else
                                {
                                    if (containers.Count != 0)
                                        containers.Last().Add(nm, str);
                                    nm = str = "";
                                }
                            }
                        }
                        else if (c == '{')
                        {
                            var container = new Dictionary<string, object>();
                            containers.Last().Add(nm, container);
                            containers.Add(container);
                            name = nm;
                            nm = "";
                        }
                        else if (c == '}')
                        {
                            if (containers.Count != 0)
                            {
                                var container = containers.Last();
                                containers.Remove(container);
                            }
                            else
                                throw new Exception("Invalid VDF format!");
                        }
                        else if (doReadString)
                        {
                            str += c;
                        }
                    }
                }
                return containers.FirstOrDefault();
            }
        }
    }
}
