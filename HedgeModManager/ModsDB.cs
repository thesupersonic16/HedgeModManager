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

namespace HedgeModManager
{
    public class ModsDB : IniFile
    {
        public List<ModInfo> Mods = new List<ModInfo>();
        public string RootDirectory { get; set; }
        public int ModCount => Mods.Count;

        public bool ReverseLoadOrder
        {
            get { return (int)this["Main"]["ReverseLoadOrder", typeof(int), 1] != 0; }
            set { this["Main"]["ReverseLoadOrder"] = (value ? "1" : "0"); }
        }

        public ModsDB()
        {
        }

        public ModsDB(string modsDirectiory)
        {
            RootDirectory = modsDirectiory;
            string iniPath = Path.Combine(RootDirectory, "ModsDb.ini");
            if (File.Exists(iniPath))
                using (var stream = File.OpenRead(iniPath))
                    Read(stream);
            else
            {
                Application.Current.MainWindow?.Hide();
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

            if (!Groups.ContainsKey("Main"))
                Groups.Add("Main", new IniGroup());
            if (!Groups.ContainsKey("Mods"))
                Groups.Add("Mods", new IniGroup());

            DetectMods();
            GetEnabledMods();
        }

        public void SetupFirstTime()
        {
            Directory.CreateDirectory(RootDirectory);
            Groups.Add("Main", new IniGroup());
            Groups.Add("Mods", new IniGroup());
            SaveDB();
        }

        public void DetectMods()
        {
            Mods.Clear();

            foreach (string folder in Directory.GetDirectories(RootDirectory))
            {
                if (File.Exists(Path.Combine(folder, "mod.ini")))
                {
                    var mod = new ModInfo(folder);
                    Mods.Add(mod);
                }
            }
        }

        public void GetEnabledMods()
        {
            int activeCount = (int)this["Main"]["ActiveModCount", typeof(int)];
            for (int i = 0; i < activeCount; i++)
            {
                var mod = Mods.FirstOrDefault(t => Path.GetFileName(t.RootDirectory) == this["Main"]?[$"ActiveMod{i}"]);
                if (mod != null)
                    mod.Enabled = true;
            }
        }

        public void BuildList()
        {
            this["Mods"].Params.Clear();
            foreach (var mod in Mods)
            {
                this["Mods"][Path.GetFileName(mod.RootDirectory)] = Path.Combine(mod.RootDirectory, "mod.ini"); 
            }
        }

        public void BuildMain()
        {
            ClearIniList();
            var count = 0;
            foreach (var mod in Mods.Where(mod => mod.Enabled))
            {
                this["Main"].Params.Add($"ActiveMod{count}", Path.GetFileName(mod.RootDirectory));
                ++count;
            }
            this["Main"].Params.Add("ActiveModCount", count.ToString());
        }

        public void SaveDB()
        {
            BuildMain();
            BuildList();
            using (var stream = File.Create(Path.Combine(RootDirectory, "ModsDB.ini")))
            {
                Write(stream);
            }
        }

        public void DeleteMod(ModInfo mod)
        {
            Mods.Remove(mod);
            Directory.Delete(mod.RootDirectory, true);
        }

        public void ClearIniList()
        {
            var list = new List<KeyValuePair<string, string>>();
            list.AddRange(this["Main"].Params.Where(t => t.Key.StartsWith("ActiveMod")));
            foreach (var o in list)
                this["Main"].Params.Remove(o.Key);
        }

        public void DisableAllMods()
        {
            Mods.Where(mod => mod.Enabled).ToList().ForEach(t => t.Enabled = false);
        }

        public void InstallMod(string path)
        {
            if (File.Exists(path))
                InstallModArchive(path);
            else if (Directory.Exists(path))
                InstallModDirectory(path);
        }

        public void InstallModArchive(string path)
        {
            if (Path.GetExtension(path) == ".zip")
            {
                InstallModArchiveUsingZipFile(path);
                return;
            }
            if (!InstallModArchiveUsing7Zip(path))
                if (!InstallModArchiveUsingWinRAR(path))
                {
                    var box = new HedgeMessageBox("ERROR", "Failed to install mods using 7-Zip and WinRAR!\n" +
                        "Make sure you have either one installed on your system.");
                    box.AddButton("  Close  ", () => box.Close());
                    box.ShowDialog();
                }
        }

        public void InstallModArchiveUsingZipFile(string path)
        {
            // Path to the install temp folder
            string tempFolder = Path.Combine(App.StartDirectory, "temp_install");

            // Deletes the temp Directory if it exists
            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);

            // Extracts all contents inside of the zip file
            ZipFile.ExtractToDirectory(path, tempFolder);

            // Install mods from the temp folder
            InstallModDirectory(tempFolder);

            // Deletes the temp folder with all of its contents
            Directory.Delete(tempFolder, true);
        }

        public bool InstallModArchiveUsing7Zip(string path)
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
                InstallModDirectory(tempDirectory);

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
        public bool InstallModArchiveUsingWinRAR(string path)
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
                InstallModDirectory(tempDirectory);

                // Deletes the temp directory with all of its contents
                Directory.Delete(tempDirectory, true);
                key.Close();
                return true;
            }
            // WinRAR is not installed
            return false;
        }

        public void InstallModDirectory(string path)
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
                    foreach (string dirPath in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(folder, Path.Combine(RootDirectory, directoryName)));

                    // Copies all the files from the Directories.
                    foreach (string filePath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                        File.Copy(filePath, filePath.Replace(folder, Path.Combine(RootDirectory, directoryName)), true);
                }
            }
        }
    }
}
