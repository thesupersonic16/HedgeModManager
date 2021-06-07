﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{

    public static class Steam
    {
        public static string SteamLocation;

        public static void Init()
        {
            // Gets Steam's Registry Key
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam");
            // If null then try get it from the 64-bit Registry
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam");
            // Checks if the Key and Value exists.
            if (key != null && key.GetValue("InstallPath") is string steamPath)
                SteamLocation = steamPath;
        }

        public static string GetCachedProfileImageURLFromSID64(string SID64)
        {
            return Path.Combine(SteamLocation, "config/avatarcache/", SID64 + ".png");
        }

        public static List<SteamGame> SearchForGames(string preference = null)
        {
            var paths = new List<string>();
            var games = new List<SteamGame>();

            if (string.IsNullOrEmpty(SteamLocation))
                return new List<SteamGame>();

            string vdfLocation = Path.Combine(SteamLocation, "steamapps\\libraryfolders.vdf");
            Dictionary<string, object> vdf = null;
            try
            {
                vdf = SteamVDF.Load(vdfLocation);
            }
            catch (Exception ex)
            {
                return new List<SteamGame>();
            }

            // Default Common Path
            paths.Add(Path.Combine(SteamLocation, "steamapps\\common"));

            // Adds all the custom libraries
            foreach (var library in SteamVDF.GetContainer(vdf, "LibraryFolders"))
                if (int.TryParse(library.Key, out int index))
                    paths.Add(Path.Combine(library.Value as string, "steamapps\\common"));

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    string gensPath = Path.Combine(path, "Sonic Generations\\SonicGenerations.exe");
                    string lwPath = Path.Combine(path, "Sonic Lost World\\slw.exe");
                    string forcesPath = Path.Combine(path, "SonicForces\\build\\main\\projects\\exec\\Sonic Forces.exe");
                    string ppt2Path = Path.Combine(path, "PuyoPuyoTetris2\\PuyoPuyoTetris2.exe");

                    if (File.Exists(gensPath))
                        games.Add(new SteamGame(Games.SonicGenerations.GameName, gensPath, Games.SonicGenerations.AppID));
                    if (File.Exists(lwPath))
                        games.Add(new SteamGame(Games.SonicLostWorld.GameName, lwPath, Games.SonicLostWorld.AppID));
                    if (File.Exists(forcesPath))
                        games.Add(new SteamGame(Games.SonicForces.GameName, forcesPath, Games.SonicForces.AppID));
                    if (File.Exists(ppt2Path))
                        games.Add(new SteamGame(Games.PuyoPuyoTetris2.GameName, ppt2Path, Games.PuyoPuyoTetris2.AppID));
                }
            }

            if (preference != null)
                return games.OrderBy(x => x.GameName != preference).ToList();
            else return games;
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
                if (value.Key == name)
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

            ReadContainers(defs);
            return defs;

            // Sub-Methods
            void ReadContainers(Dictionary<string, object> parent)
            {
                Dictionary<string, object> container = null;
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
                                    if (container != null)
                                        container.Add(nm, str);
                                    else
                                        parent.Add(nm, str);

                                    nm = str = "";
                                }
                            }
                        }
                        else if (c == '{')
                        {
                            if (container == null)
                            {
                                container = new Dictionary<string, object>();
                                name = nm;
                            }
                            else
                            {
                                var subContainer = new Dictionary<string, object>();
                                ReadContainers(subContainer);
                                container.Add(nm, subContainer);
                            }

                            nm = "";
                        }
                        else if (c == '}')
                        {
                            if (container != null)
                            {
                                parent.Add(name, container);
                                container = null;
                            }
                        }
                        else if (doReadString)
                        {
                            str += c;
                        }
                    }
                }
            }
        }
    }

    public class SteamGame
    {
        public string GameName { get; set; }
        public string GameID { get; set; }
        public string ExeName { get; set; }
        public string RootDirectory { get; set; }
        public string ExeDirectory { get { return Path.Combine(RootDirectory, ExeName); } }
        public bool Status { get { return Steam.CheckGame(ExeDirectory); } }

        public SteamGame(string gameName, string exe, string gameID)
        {
            GameName = gameName;
            RootDirectory = Path.GetDirectoryName(exe);
            ExeName = Path.GetFileName(exe);
            GameID = gameID;
        }

        public void StartGame()
        {
            Process.Start($"steam://rungameid/{GameID}");
        }

        public override string ToString()
        {
            return GameName;
        }
    }
}
