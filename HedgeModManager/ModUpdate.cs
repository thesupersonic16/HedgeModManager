using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Misc;

namespace HedgeModManager
{
    public static class ModUpdate
    {

        /// <summary>
        /// Parses a GMI mod update from a mod UpdateServer
        /// </summary>
        /// <param name="mod">Mod to be updated</param>
        /// <returns>A ModUpdateInfo containing information about the lastest update</returns>
        public static async Task<ModUpdateInfo> GetUpdateFromINIAsync(ModInfo mod)
        {
            if (string.IsNullOrEmpty(mod.UpdateServer))
                return null;

            var ini = new IniFile();
            using (var stream = await HedgeApp.HttpClient.GetStreamAsync(Path.Combine(mod.UpdateServer, "mod_version.ini")))
                ini.Read(stream);

            return await GetUpdateFromINIAsync(mod, ini);
        }


        /// <summary>
        /// Parses a GMI mod update
        /// </summary>
        /// <param name="mod">Mod to be updated</param>
        /// <param name="ini">ini of the mod_update.ini file downloaded from the server</param>
        /// <returns>A ModUpdateInfo containing information about the lastest update</returns>
        public static async Task<ModUpdateInfo> GetUpdateFromINIAsync(ModInfo mod, IniFile ini)
        {
            // Check if the ini contains all the required fields
            if (!ini.Groups.ContainsKey("Main"))
                return null;
            if (!ini["Main"].Params.ContainsKey("VersionString"))
                return null;
            if (!ini["Main"].Params.ContainsKey("DownloadSizeString"))
                return null;

            // Variables
            string modVersion = ini["Main"]["VersionString"];
            string modDLSize = ini["Main"]["DownloadSizeString"];
            string mdUrl = ini["Main"]["Markdown"];

            // Read changelog into one larger string
            string modChangeLog = "";
            int modChangeLogLineCount = int.Parse(ini["Changelog"]["StringCount"]);

            if (string.IsNullOrEmpty(mdUrl))
            {
                modChangeLog += "Changelog:<br/>";
                for (int i = 0; i < modChangeLogLineCount; ++i)
                    modChangeLog += $"- {ini["Changelog"][$"String{i}"]}<br/>";
            }
            else
            {
                modChangeLog = await HedgeApp.HttpClient.GetStringAsync(Path.Combine(mod.UpdateServer, mdUrl)).ConfigureAwait(false);
            }


            // Create a mod info
            var update = new ModUpdateInfo
            {
                Name = mod.Title,
                VersionString = modVersion,
                DownloadSizeString = modDLSize,
                ChangeLog = modChangeLog,
                Mod = mod
            };

            // Downloads and parses the list of files
            string modFiles = await HedgeApp.HttpClient.GetStringAsync(Path.Combine(mod.UpdateServer, "mod_files.txt")).ConfigureAwait(false);
            ReadUpdateFileList(modFiles);
            return update;

            // Sub-Methods
            void ReadUpdateFileList(string data)
            {

                // Adds the file name and url to the files array
                foreach (string line in data.Replace("\r", "").Split('\n'))
                {
                    // Checks if the line starts with ';' or '#' if does then continue to the next line
                    if (line.StartsWith(";") || line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var file = new ModUpdateFile()
                    {
                        FileName = line.Split(' ')[1],
                        URL = Path.Combine(mod.UpdateServer,
                        Path.Combine(Path.GetDirectoryName(line.Split(' ')[1]), Uri.EscapeDataString(Path.GetFileName(line.Split(' ')[1])))),
                        Command = line.Split(' ')[0]
                    };
                    update.Files.Add(file);
                }
            }
        }

        public static async Task DownloadAndApplyUpdateAsync(ModUpdateInfo modupdate, ModUpdateProgress progress)
        {
            for (int i = 0; i < modupdate.Files.Count; ++i)
            {
                var file = modupdate.Files[i];
                switch (file.Command.ToLower())
                {
                    case "add":
                        // Creates directories
                        var fileInfo = new FileInfo(Path.Combine(modupdate.Mod.RootDirectory, file.FileName));
                        if (!fileInfo.Directory.Exists)
                            Directory.CreateDirectory(fileInfo.Directory.FullName);

                        // Download file
                        await HedgeApp.HttpClient.DownloadFileAsync(file.URL, fileInfo.FullName);
                        break;
                    case "delete":
                        File.Delete(Path.Combine(modupdate.Mod.RootDirectory, file.FileName));
                        break;
                    default:
                        throw new Exception("Unknown Mod Update Command " + file.Command);
                }

                progress.Progress = ((float)i / modupdate.Files.Count) * 100f;
                progress.CurrentFile = Path.GetFileName(file.FileName);
            }
            progress.Completed = true;
        }

        public class ModUpdateInfo
        {
            // Information about the new update
            public string Name, VersionString, DownloadSizeString, ChangeLog;
            // The mod the update applies to
            public ModInfo Mod;
            // List of files and commands to download and process
            public List<ModUpdateFile> Files = new List<ModUpdateFile>();
        }

        public class ModUpdateProgress
        {
            public bool Completed = false;
            // Progress of the update (0f-100f)
            public float Progress = 0f;
            // The current file thats being worked on
            public string CurrentFile = "";
        }

        public class ModUpdateFile
        {
            public string FileName, URL, Command;
        }
    }
}
