﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HedgeModManager
{
    public class SnapshotBuilder
    {
        /// <summary>
        /// Builds a snapshot from the user's system and returns a zip file
        /// </summary>
        /// <param name="encrypt">Should the snapshot data be encrypted</param>
        /// <param name="extraData">Extra data to be included in the snapshot</param>
        /// <returns></returns>
        public static byte[] Build(bool encrypt = true, params SnapshotFile[] extraData)
        {
            using (var stream = new MemoryStream())
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                var report = archive.CreateEntry("Report.txt");
                
                using (var file = report.Open())
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(CreateReport());
                }

                foreach (var file in extraData)
                {
                    var entry = archive.CreateEntry(file.Name);
                    using (var entryStream = entry.Open())
                    {
                        entryStream.Write(file.Data, 0, file.Data.Length);
                    }
                }

                try
                {
                    archive.CreateEntryFromFile(HedgeApp.ConfigPath, "cpkredir.ini");
                }
                catch { }

                try
                {
                    archive.CreateEntryFromFile(Path.Combine(HedgeApp.ModsDbPath, "modsdb.ini"), "ModsDB.ini");
                }
                catch { }

                try
                {
                    archive.CreateEntryFromFile(Path.Combine(HedgeApp.StartDirectory, "profiles.json"), "profiles.json");
                }
                catch { }

                try
                {
                    foreach (var file in Directory.GetFiles(HedgeApp.ModsDbPath, "mod.ini", SearchOption.AllDirectories))
                    {
                        archive.CreateEntryFromFile(file, $"Mods{Path.DirectorySeparatorChar}{GetRelativePath(file, HedgeApp.ModsDbPath)}");
                    }
                }
                catch { }

                try
                {
                    foreach (var file in Directory.GetFiles(HedgeApp.ModsDbPath, "*.ini", SearchOption.TopDirectoryOnly))
                    {
                        archive.CreateEntryFromFile(file, $"Profiles{Path.DirectorySeparatorChar}{GetRelativePath(file, HedgeApp.ModsDbPath)}");
                    }
                }
                catch { }

                return encrypt ? CryptoProvider.Encrypt(stream.ToArray()) : stream.ToArray();
            }
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string CreateReport()
        {
            StringBuilder body = new StringBuilder();
            var loaderPath = Path.Combine(HedgeApp.StartDirectory, HedgeApp.CurrentGame.ModLoader?.ModLoaderFileName);
            var cpkredirPath = Path.Combine(HedgeApp.StartDirectory, "cpkredir.dll");
            var gamePath = Path.Combine(HedgeApp.StartDirectory, HedgeApp.CurrentGame.ExecutableName);

            body.AppendLine($"Start Directory: {HedgeApp.StartDirectory}");
            body.AppendLine(File.Exists(loaderPath)
                ? $"Loader Hash: {HedgeApp.ComputeMD5Hash(loaderPath)}"
                : $"{HedgeApp.CurrentGame.ModLoader?.ModLoaderName} does not exist!");

            if (HedgeApp.CurrentGame.SupportsCPKREDIR)
            {
                body.AppendLine(File.Exists(cpkredirPath)
                    ? $"CPKREDIR Hash: {HedgeApp.ComputeMD5Hash(cpkredirPath)}"
                    : "CPKREDIR does not exist!");

                try
                {
                    if (HedgeApp.CurrentGame.SupportsCPKREDIR)
                        body.AppendLine($"CPKREDIR Installed: {HedgeApp.IsCPKREDIRInstalled(gamePath)}");
                }
                catch { }
            }

            try
            {
                body.AppendLine($"{HedgeApp.CurrentGame.ExecutableName} Hash: {HedgeApp.ComputeMD5Hash(gamePath)}");
            }
            catch(Exception e)
            {
                body.AppendLine($"Failed to compute game hash. {e.Message}");
            }


            body.AppendLine();
            body.AppendLine("Directory tree:");
            body.AppendLine(GetDirectoryTree(false));

            body.AppendLine(GetRegistryTree(Registry.LocalMachine, "SOFTWARE\\7-Zip"));
            body.AppendLine(GetRegistryTree(Registry.LocalMachine, "SOFTWARE\\WinRAR"));

            foreach (var game in Games.GetSupportedGames())
            {
                body.AppendLine(GetRegistryTree(Registry.CurrentUser, $"Software\\Classes\\{game.GBProtocol}"));
            }

            return body.ToString();
        }

        public static string GetDirectoryTree(bool recursive = true)
        {
            var body = new StringBuilder();
            int directoryLevel = 0;
            GetDirectory(new DirectoryInfo(HedgeApp.StartDirectory));
            return body.ToString();

            void GetDirectory(DirectoryInfo info)
            {
                for (int i = 0; i < directoryLevel; i++)
                    body.Append("--");

                body.AppendLine(info.Name);

                foreach (var dir in info.GetDirectories())
                {
                    if (recursive)
                    {
                        directoryLevel++;
                        GetDirectory(dir);
                        directoryLevel--;
                    }
                    else
                    {
                        for (int i = 0; i <= directoryLevel + 1; i++)
                            body.Append("--");
                        body.AppendLine(dir.Name);
                    }
                }

                try
                {
                    var files = info.GetFiles();
                    foreach (var file in files)
                    {
                        for (int i = 0; i <= directoryLevel; i++)
                            body.Append("--");

                        body.AppendLine($"{file.Name}, {file.Length:X}");
                    }
                }
                catch
                {

                }
            }
        }

        public static string GetRegistryTree(RegistryKey baseKey, string path)
        {
            var key = baseKey.OpenSubKey(path);
            if (key != null)
            {
                var report = GetRegistryTree(key);
                key.Close();
                return report;
            }

            return string.Empty;
        }

        public static string GetRegistryTree(RegistryKey key)
        {
            var body = new StringBuilder();
            int keyLevel = 0;
            GetTree(key);
            return body.ToString();

            void GetTree(RegistryKey k)
            {
                for (int i = 0; i < keyLevel; i++)
                    body.Append("--");

                body.AppendLine(k.Name);

                var values = k.GetValueNames();
                var keys = k.GetSubKeyNames();

                foreach (var value in values)
                {
                    for (int i = 0; i <= keyLevel + 1; i++)
                        body.Append("--");

                    body.AppendLine($"{(string.IsNullOrEmpty(value) ? "(default)" : value)}: {k.GetValue(value)}");
                }

                foreach (var sub in keys)
                {
                    keyLevel += 1;
                    var subKey = k.OpenSubKey(sub, false);
                    if (subKey != null)
                    {
                        GetTree(subKey);
                        subKey.Close();
                    }
                    else
                    {
                        for (int i = 0; i < keyLevel; i++)
                            body.Append("--");
                        body.AppendLine(k.Name);
                    }
                    keyLevel -= 1;
                }
            }
        }
    }

    public class SnapshotFile
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }

        public SnapshotFile(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }
    }
}
