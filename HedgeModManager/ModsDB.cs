using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HedgeModManager.Serialization;
using HedgeModManager.UI;
using Newtonsoft.Json;

namespace HedgeModManager
{
    public class ModsDB
    {
        public List<ModInfo> Mods = new List<ModInfo>();

        [IniField("Main", "ActiveMod")]
        public List<string> ActiveMods = new List<string>();

        [IniField("Main", "ReverseLoadOrder")]
        public bool ReverseLoadOrder { get; set; }

        [IniField("Mods")]
        private Dictionary<string, string> mMods = new Dictionary<string, string>();

        [IniField("Codes", "Code")]
        public List<string> Codes = new List<string>();

        public string RootDirectory { get; set; }
        public int ModCount => Mods.Count;
        public int ActiveModCount => ActiveMods.Count;

        public ModsDB()
        {
        }

        public ModsDB(string modsDirectiory)
        {
            RootDirectory = modsDirectiory;
            string iniPath = Path.Combine(RootDirectory, "ModsDb.ini");
            if (File.Exists(iniPath))
            {
                try
                {
                    using (var stream = File.OpenRead(iniPath))
                        IniSerializer.Deserialize(this, stream);
                }
                catch
                {
                    DetectMods();
                }
            }
            else if (!Directory.Exists(RootDirectory))
            {
                Application.Current?.MainWindow?.Hide();
                var box = new HedgeMessageBox("No Mods Found", Properties.Resources.STR_UI_NO_MODS);

                box.AddButton("Yes", () =>
                {
                    SetupFirstTime();
                    box.Close();
                });

                box.AddButton("No", () => Environment.Exit(0));

                box.ShowDialog();
                Application.Current?.MainWindow?.Show();
                return;
            }

            DetectMods();
            GetEnabledMods();
        }

        public void SetupFirstTime()
        {
            Directory.CreateDirectory(RootDirectory);
            SaveDB();
        }

        public void DetectMods()
        {
            Mods.Clear();

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
                var mod = Mods.FirstOrDefault(t => Path.GetFileName(t.RootDirectory) == ActiveMods[i]);
                if (mod != null)
                    mod.Enabled = true;
                else
                    ActiveMods.RemoveAt(i--);
            }
        }
        public void SaveDB()
        {
            ActiveMods.Clear();
            mMods.Clear();

            Mods.ForEach(mod =>
            {
                if (mod.Enabled)
                    ActiveMods.Add(Path.GetFileName(mod.RootDirectory));
                mMods.Add(Path.GetFileName(mod.RootDirectory), $"{mod.RootDirectory}{Path.DirectorySeparatorChar}mod.ini");
            });
            using (var stream = File.Create(Path.Combine(RootDirectory, "ModsDB.ini")))
            {
                IniSerializer.Serialize(this, stream);
            }
            CodeList.WriteDatFile(Path.Combine(RootDirectory, CodeLoader.CodesPath), new List<Code>(MainWindow.CodesDatabase.Codes.Where((x, y) => x.Enabled)), App.CurrentGame.Is64Bit);
        }

        public void DeleteMod(ModInfo mod)
        {
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
                {
                    var box = new HedgeMessageBox("ERROR", "Failed to install mods using 7-Zip and WinRAR!\n" +
                        "Make sure you have either one installed on your system.");
                    box.AddButton("  Close  ", () => box.Close());
                    box.ShowDialog();
                }
        }

        public static void InstallModArchiveUsingZipFile(string path, string root)
        {
            // Path to the install temp folder
            string tempFolder = Path.Combine(App.StartDirectory, "temp_install");

            // Deletes the temp Directory if it exists
            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);

            // Extracts all contents inside of the zip file
            ZipFile.ExtractToDirectory(path, tempFolder);

            // Install mods from the temp folder
            InstallModDirectory(tempFolder, root);

            // Deletes the temp folder with all of its contents
            Directory.Delete(tempFolder, true);
        }

        public static bool InstallModArchiveUsing7Zip(string path, string root)
        {
            // Gets 7-Zip's Registry Key
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\7-Zip");
            // If null then try get it from the 64-bit Registry
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE\\7-Zip");
            // Checks if 7-Zip is installed by checking if the key and path value exists
            if (key != null && key.GetValue("Path") is string exePath)
            {
                // Path to 7z.exe
                string exe = Path.Combine(exePath, "7z.exe");

                // Path to the install temp directory
                string tempDirectory = Path.Combine(App.StartDirectory, "temp_install");

                // Deletes the temp directory if it exists
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);

                // Creates the temp directory
                Directory.CreateDirectory(tempDirectory);

                // Extracts the archive to the temp directory
                var psi = new ProcessStartInfo(exe, $"x \"{path}\" -o\"{tempDirectory}\" -y");
                Process.Start(psi).WaitForExit(1000 * 60 * 5);

                // Search and install mods from the temp directory
                InstallModDirectory(tempDirectory, root);

                // Deletes the temp directory with all of its contents
                Directory.Delete(tempDirectory, true);
                key.Close();
                return true;
            }
            // 7-Zip is not installed
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
                string tempDirectory = Path.Combine(App.StartDirectory, "temp_install");

                // Deletes the temp directory if it exists
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);

                // Creates the temp directory
                Directory.CreateDirectory(tempDirectory);

                // Extracts the archive to the temp directory
                var psi = new ProcessStartInfo(exePath, $"x \"{path}\" \"{tempDirectory}\"");
                Process.Start(psi).WaitForExit(1000 * 60 * 5);

                // Search and install mods from the temp directory
                InstallModDirectory(tempDirectory, root);

                // Deletes the temp directory with all of its contents
                Directory.Delete(tempDirectory, true);
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
                        var mod = new ModInfo(Path.Combine(folder, "mod.ini"));
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

        public void CreateMod(ModInfo mod, bool openFolder = false)
        {
            var path = Path.Combine(RootDirectory, mod.Title);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "disk"));
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
                Process.Start(path);
            }
        }
    }
}
