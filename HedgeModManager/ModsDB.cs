using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameBananaAPI;
using HedgeModManager.Exceptions;
using HedgeModManager.Serialization;
using HedgeModManager.UI;
using Newtonsoft.Json;
using static HedgeModManager.Lang;
using HedgeModManager.CodeCompiler;

namespace HedgeModManager
{
    public class ModsDB : IEnumerable<ModInfo>
    {
        public CodeFile CodesDatabase = new CodeFile();
        public List<ModInfo> Mods = new List<ModInfo>();

        [IniField("Main", "ActiveMod")]
        public List<string> ActiveMods = new List<string>();

        [IniField("Main", "ReverseLoadOrder")]
        public bool ReverseLoadOrder { get; set; }

        [IniField("Main", "FavoriteMod")]
        public List<string> FavoriteMods = new List<string>();

        [IniField("Mods")]
        private Dictionary<string, string> mMods = new Dictionary<string, string>();

        [IniField("Codes", "Code")]
        public List<string> Codes = new List<string>();

        public string RootDirectory { get; set; }
        public string FileName { get; set; } = "ModsDB.ini";
        public int ModCount => Mods.Count;

        public ModsDB()
        {
        }

        public ModsDB(string modsDirectiory, string fileName = "ModsDB.ini")
        {
            RootDirectory = modsDirectiory;
            FileName = fileName;
            string iniPath = Path.Combine(RootDirectory, fileName);
            if (File.Exists(iniPath))
            {
                try
                {
                    using (var stream = File.OpenRead(iniPath))
                        IniSerializer.Deserialize(this, stream);
                    
                    // Force load order to bottom to top
                    ReverseLoadOrder = false;
                }
                catch
                {
                    DetectMods();
                }
            }

            DetectMods();
            GetEnabledMods();
        }

        public void SetupFirstTime()
        {
            Directory.CreateDirectory(RootDirectory);
            SaveDBSync();
        }

        public void DetectMods()
        {
            Mods.Clear();
            if (!Directory.Exists(RootDirectory))
                return;

            foreach (string folder in Directory.GetDirectories(RootDirectory))
            {
                if (File.Exists(Path.Combine(folder, "mod.ini")))
                {
                    try
                    {
                        var mod = new ModInfo(folder);
                        Mods.Add(mod);
                    }
                    catch (Exceptions.ModLoadException) { }
                }
            }
        }

        public void GetEnabledMods()
        {
            for (int i = 0; i < ActiveMods.Count; i++)
            {
                var mod = GetModFromActiveGUID(ActiveMods[i]);
                if (mod != null)
                    mod.Enabled = true;
                else
                    ActiveMods.RemoveAt(i--);
            }

            for (int i = 0; i < FavoriteMods.Count; i++)
            {
                var mod = GetModFromActiveGUID(FavoriteMods[i]);
                if (mod != null)
                    mod.Favorite = true;
                else
                    FavoriteMods.RemoveAt(i--);
            }
        }

        public DependencyReport ResolveDepends()
        {
            var report = new DependencyReport();

            var enabledMods = new List<ModInfo>();
            List<ModInfo> newMods = null;
            newMods = CheckDepends(Mods, enabledMods);
            while (newMods != null)
                newMods = CheckDepends(Mods, enabledMods);

            return report;

            List<ModInfo> CheckDepends(IEnumerable<ModInfo> mods, List<ModInfo> enabledMods)
            {
                List<ModInfo> result = null;
                foreach (var mod in mods)
                {
                    if (mod.Enabled)
                    {
                        DependencyReport.ErrorInfo info = null;
                        foreach (var depend in mod.DependsOn)
                        {
                            var resolvedMod = Mods.FirstOrDefault(m => m.ID == depend.ID);
                            if (resolvedMod == null)
                            {
                                info ??= new DependencyReport.ErrorInfo { Mod = mod };
                                info.UnresolvedDepends.Add(depend);
                                continue;
                            }

                            if (depend.ModVersion != null)
                            {
                                if (!Version.TryParse(resolvedMod.Version, out var modVersion) || modVersion < depend.ModVersion)
                                {
                                    info ??= new DependencyReport.ErrorInfo { Mod = mod };
                                    info.UnresolvedDepends.Add(depend);
                                    continue;
                                }
                            }

                            resolvedMod.Enabled = true;
                            if (resolvedMod.DependsOn.Count > 0 && !enabledMods.Contains(resolvedMod))
                            {
                                result ??= new List<ModInfo>();
                                result.Add(resolvedMod);
                                enabledMods.Add(resolvedMod);
                            }
                        }

                        if (info != null)
                            report.Errors.Add(info);
                    }
                }

                return result;
            }
        }

        public void SaveDBSync(bool compileCodes = true)
        {
            SaveDB(compileCodes).GetAwaiter().GetResult();
        }

        public async Task SaveDB(bool compileCodes = true)
        {
            ActiveMods.Clear();
            FavoriteMods.Clear();
            mMods.Clear();

            foreach (var mod in Mods)
            {
                var id = HedgeApp.GenerateSeededGuid(mod.RootDirectory.GetHashCode()).ToString();

                if (mod.Enabled)
                    ActiveMods.Add(id);

                if (mod.Favorite)
                    FavoriteMods.Add(id);

                // ReSharper disable once AssignNullToNotNullAttribute
                mMods.Add(id, $"{mod.RootDirectory}{Path.DirectorySeparatorChar}mod.ini");
            }
            using (var stream = File.Create(Path.Combine(RootDirectory, FileName)))
            {
                IniSerializer.Serialize(this, stream);
            }

            if (compileCodes)
            {
                var codes = new List<Code>();

                foreach (var code in CodesDatabase.Codes)
                {
                    if (code.Enabled || code.Type == CodeType.Library)
                        codes.Add(code);
                }

                foreach (var mod in Mods)
                {
                    if (mod.Enabled && mod.Codes != null)
                        codes.AddRange(mod.Codes.Codes);
                }

                await CodeProvider.CompileCodes(codes, CodeProvider.CompiledCodesPath);
            }
        }

        public ModInfo GetModFromActiveGUID(string id)
        {
            var modPair = mMods.FirstOrDefault(t => t.Key == id);

            if (modPair.Key == null)
                return null;

            string modPath = Path.GetDirectoryName(modPair.Value);

            // If the path doesn't exist, check RootDirectory if the game was moved elsewhere.
            if (!Directory.Exists(modPath))
                modPath = Path.Combine(RootDirectory, Path.GetFileName(modPath));

            return Mods.FirstOrDefault(t => modPath.Equals(t.RootDirectory, StringComparison.OrdinalIgnoreCase));
        }

        public void DeleteMod(ModInfo mod)
        {
            ActiveMods.Clear();
            Mods.Remove(mod);
            Directory.Delete(mod.RootDirectory, true);
        }

        public void DisableAllMods()
        {
            Mods.Where(mod => mod.Enabled).ToList().ForEach(t => t.Enabled = false);
        }

        public void InstallMod(string path)
        {
            InstallMod(path, RootDirectory);
        }

        public static void InstallMod(string path, string root)
        {
            if (File.Exists(path))
                InstallModArchive(path, root);
            else if (Directory.Exists(path))
                InstallModDirectory(path, root);
        }

        public static void InstallModArchive(string path, string root)
        {
            if (Path.GetExtension(path) == ".zip")
            {
                InstallModArchiveUsingZipFile(path, root);
                return;
            }
            if (!InstallModArchiveUsing7Zip(path, root))
                if (!InstallModArchiveUsingWinRAR(path, root))
                    throw new ModInstallException("Could not install mod, neither 7-Zip or WinRAR was found!");
        }

        public static void InstallModArchiveUsingZipFile(string path, string root)
        {
            // Path to the install temp folder
            string tempDirectory = Path.Combine(HedgeApp.StartDirectory, "temp_install", Guid.NewGuid().ToString());

            // Deletes the temp Directory if it exists
            if (Directory.Exists(tempDirectory))
                DeleteReadOnlyDirectory(tempDirectory);

            // Extracts all contents inside of the zip file
            ZipFile.ExtractToDirectory(path, tempDirectory);

            // Install mods from the temp folder
            InstallModDirectory(tempDirectory, root);

            // Deletes the temp folder with all of its contents
            DeleteReadOnlyDirectory(tempDirectory);
        }

        public static bool InstallModArchiveUsing7Zip(string path, string root)
        {
            string exePath = null;
            // Check if file exists next to the main assembly
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.exe")))
                exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.exe");
            // Find the path from the registry
            if (exePath == null)
            {
                var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\7-Zip");
                if (key == null)
                    key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                        .OpenSubKey("SOFTWARE\\7-Zip");
                if (key != null)
                    exePath = key.GetValue("Path") as string;
            }
            // Find 7z.exe from the PATH variable if still not found
            exePath ??= Environment.GetEnvironmentVariable("PATH").Split(';').ToList()
                .Where(s => File.Exists(Path.Combine(s, "7z.exe"))).FirstOrDefault();

            // Checks if 7-Zip is installed
            if (exePath != null)
            {
                string exe = Path.Combine(exePath, "7z.exe");

                // Path to the install temp directory
                string tempDirectory = Path.Combine(HedgeApp.StartDirectory, "temp_install");

                if (Directory.Exists(tempDirectory))
                    DeleteReadOnlyDirectory(tempDirectory);
                Directory.CreateDirectory(tempDirectory);

                // Extracts the archive to the temp directory
                var psi = new ProcessStartInfo(exe, $"x \"{path}\" -o\"{tempDirectory}\" -y");
                Process.Start(psi).WaitForExit(1000 * 60 * 5);

                InstallModDirectory(tempDirectory, root);

                DeleteReadOnlyDirectory(tempDirectory);
                return true;
            }
            return false;
        }

        // TODO: Add WinRAR x86 support
        // TODO: Needs Testing
        public static bool InstallModArchiveUsingWinRAR(string path, string root)
        {
            // Gets WinRAR's Registry Key
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WinRAR");
            // If null then try get it from the 64-bit Registry
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE\\WinRAR");
            // Checks if WinRAR is installed by checking if the key and path value exists
            if (key != null && key.GetValue("exe64") is string exePath)
            {
                // Path to the install temp directory
                string tempDirectory = Path.Combine(HedgeApp.StartDirectory, "temp_install");

                // Deletes the temp directory if it exists
                if (Directory.Exists(tempDirectory))
                    DeleteReadOnlyDirectory(tempDirectory);

                // Creates the temp directory
                Directory.CreateDirectory(tempDirectory);

                // Extracts the archive to the temp directory
                var psi = new ProcessStartInfo(exePath, $"x \"{path}\" \"{tempDirectory}\"");
                Process.Start(psi).WaitForExit(1000 * 60 * 5);

                // Search and install mods from the temp directory
                InstallModDirectory(tempDirectory, root);

                // Deletes the temp directory with all of its contents
                DeleteReadOnlyDirectory(tempDirectory);
                key.Close();
                return true;
            }
            // WinRAR is not installed
            return false;
        }

        public static void InstallModDirectory(string path, string root)
        {
            // A list of folders that have mod.ini in them
            var directories = new List<string>();

            // Looks though all the folders for mods
            directories.AddRange(Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                .Where(t => File.Exists(Path.Combine(t, "mod.ini"))));

            // Checks if there is a file called "mod.ini" inside the selected folder
            if (File.Exists(Path.Combine(path, "mod.ini")))
                directories.Add(path);

            // Check if there is any mods
            if (directories.Count > 0)
            {
                foreach (string folder in directories)
                {
                    string directoryName = Path.GetFileName(folder);

                    // If it doesn't know the name of the mod its installing
                    if (directoryName == "temp_install")
                    {
                        var mod = new ModInfo(folder);
                        directoryName = new string(mod.Title.Where(x => !Path.GetInvalidFileNameChars()
                            .Contains(x)).ToArray());
                    }

                    // Creates all of the directories.
                    Directory.CreateDirectory(Path.Combine(root, Path.GetFileName(folder)));
                    foreach (string dirPath in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(folder, Path.Combine(root, directoryName)));

                    // Copies all the files from the Directories.
                    foreach (string filePath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                        File.Copy(filePath, filePath.Replace(folder, Path.Combine(root, directoryName)), true);
                }
            }
        }

        public static void DeleteReadOnlyDirectory(string dir)
        {
            DeleteReadOnlyDirectory(new DirectoryInfo(dir));
        }

        public static void DeleteReadOnlyDirectory(DirectoryInfo dir)
        {
            // Recursively perform this function 
            foreach (var subDir in dir.GetDirectories())
                DeleteReadOnlyDirectory(subDir);

            // Delete all files in the directory and remove readonly
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }catch{}
            }

            try
            {
                // remove readonly from the directory
                dir.Attributes = FileAttributes.Normal;
                // Delete the directory
                dir.Delete();
            }catch{}
        }

        public void CreateMod(ModInfo mod, string[] folders, bool openFolder)
        {
            string path = string.Concat(mod.Title.Split(Path.GetInvalidFileNameChars()));
            path = Path.Combine(RootDirectory, path);
            Directory.CreateDirectory(path);
            foreach (string folder in folders)
                if (!string.IsNullOrEmpty(folder))
                    Directory.CreateDirectory(Path.Combine(path, folder));
            mod.RootDirectory = path;
            mod.Save();

            if (!string.IsNullOrEmpty(mod.ConfigSchemaFile))
            {
                var schema = new FormSchema() { IniFile = "Config.ini" };
                schema.Groups.Add(new FormGroup()
                {
                    DisplayName = "Example Group",
                    Name = "Group1",
                    Elements = new List<FormElement>()
                    {
                        new FormElement()
                        {
                            DisplayName = "Example Bool", Name = "exBool", DefaultValue = false, Type = "bool", Description = new List<string>() { "Line 1", "Line 2" }
                        },
                        new FormElement()
                        {
                            DisplayName = "Example String", Name = "exString", DefaultValue = string.Empty, Type = "string", Description = new List<string>() { "Line 1", "Line 2" }
                        },
                        new FormElement()
                        {
                            DisplayName = "Example Float", Name = "exFloat", DefaultValue = 0.0f, Type = "float", Description = new List<string>() { "Line 1", "Line 2" }
                        },
                        new FormElement()
                        {
                            DisplayName = "Example Int", Name = "exInt", DefaultValue = 0, Type = "int", Description = new List<string>() { "Line 1", "Line 2" }
                        },
                        new FormElement()
                        {
                            DisplayName = "Example Enum", Name = "exEnum", DefaultValue = "Item1", Type = "ExampleEnum", Description = new List<string>() { "Line 1", "Line 2" }
                        }
                    }
                });

                schema.Enums.Add("ExampleEnum", new List<FormEnum>()
                {
                    new FormEnum(){ DisplayName = "Item 1", Value = "Item1", Description = new List<string>() { "Line 1", "Line 2" }},
                    new FormEnum(){ DisplayName = "Item 2", Value = "Item2", Description = new List<string>() { "Line 1", "Line 2" }}
                });
                schema.SaveIni(Path.Combine(mod.RootDirectory, schema.IniFile));
                File.WriteAllText(Path.Combine(mod.RootDirectory, mod.ConfigSchemaFile), JsonConvert.SerializeObject(schema, Formatting.Indented));
            }

            if (openFolder)
            {
                HedgeApp.StartURL(path);
            }
        }

        public List<ModInfo> GetInvalidMods()
        {
            var invalid = new List<ModInfo>();

            foreach (var mod in Mods)
            {
                if(!mod.ValidateIncludeDirectories())
                    invalid.Add(mod);
            }

            return invalid;
        }

        public IEnumerator<ModInfo> GetEnumerator()
        {
            return Mods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DependencyReport
    {
        public class ErrorInfo
        {
            public ModInfo Mod { get; set; }
            public List<ModDepend> UnresolvedDepends { get; set; } = new List<ModDepend>();
        }

        public List<ErrorInfo> Errors { get; set; } = new List<ErrorInfo>();
        public bool HasErrors => Errors.Count > 0;

        public string BuildMarkdown()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Localise("MainUIMissingDepends"));

            bool resolvable = false;

            foreach (var error in Errors)
            {
                builder.AppendLine($"- {error.Mod.Title}");
                foreach (var depend in error.UnresolvedDepends)
                {
                    builder.AppendLine(depend.HasLink
                        ? $"  - [{BuildName()}]({depend.Link})"
                        : $"  - {BuildName()}");
                    if (!resolvable && GBAPI.GetGameBananaModID(depend.Link) != -1)
                        resolvable = true;

                    string BuildName()
                    {
                        return $"{depend.Title}{(depend.ModVersion != null ? $" ({Lang.Localise("ModsUIVersion")}: {depend.VersionString})" : "")}";
                    }
                }
            }

            if (resolvable)
                builder.AppendLine("\n" + Localise("MainUIResolvable"));

            return builder.ToString();
        }
    }
}
