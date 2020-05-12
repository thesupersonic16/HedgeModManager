using System;
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
        /// <returns></returns>
        public static byte[] Build(bool encrypt = true)
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

                try
                {
                    archive.CreateEntryFromFile(App.ConfigPath, "cpkredir.ini");
                }
                catch { }

                try
                {
                    archive.CreateEntryFromFile(Path.Combine(App.ModsDbPath, "modsdb.ini"), "ModsDB.ini");
                }
                catch { }

                try
                {
                    foreach (var file in Directory.GetFiles(App.ModsDbPath, "mod.ini", SearchOption.AllDirectories))
                    {
                        archive.CreateEntryFromFile(file, $"Mods{Path.DirectorySeparatorChar}{GetRelativePath(file, App.ModsDbPath)}");
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
            var loaderPath = Path.Combine(App.StartDirectory, $"d3d{App.CurrentGame.DirectXVersion}.dll");
            var cpkredirPath = Path.Combine(App.StartDirectory, "cpkredir.dll");

            body.AppendLine($"Start Directory: {App.StartDirectory}");
            body.AppendLine(File.Exists(loaderPath)
                ? $"Loader Hash: {App.ComputeMD5Hash(loaderPath)}"
                : $"{App.CurrentGame.CustomLoaderName} does not exist!");

            if (App.CurrentGame.SupportsCPKREDIR)
                body.AppendLine(File.Exists(cpkredirPath)
                    ? $"CPKREDIR Hash: {App.ComputeMD5Hash(cpkredirPath)}"
                    : "CPKREDIR does not exist!");

            try
            {
                var gamePath = Path.Combine(App.StartDirectory, App.CurrentGame.ExecuteableName);
                body.AppendLine($"{App.CurrentGame.ExecuteableName} Hash: {App.ComputeMD5Hash(gamePath)}");
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
            GetDirectory(new DirectoryInfo(App.StartDirectory));
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
                    GetTree(subKey);
                    subKey.Close();
                    keyLevel -= 1;
                }
            }
        }
    }
}
