using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Drawing;
using System.Security.Cryptography;
using Microsoft.Win32;
using SLWModLoader.Properties;
using System.Xml.Linq;

namespace SLWModLoader
{
    public partial class MainForm : Form
    {
        // Variables/Constants
        public static string GensExecutablePath = Path.Combine(Program.StartDirectory, "SonicGenerations.exe");
        public static string LWExecutablePath = Path.Combine(Program.StartDirectory, "slw.exe");
        public static string ModsFolderPath = Path.Combine(Program.StartDirectory, "mods");
        public static string ModsDbPath = Path.Combine(ModsFolderPath, "ModsDB.ini");

        public static ModsDatabase ModsDb;
        public static Dictionary<string, byte[]> GenerationsPatches = new Dictionary<string, byte[]>();
        public static IniFile CPKREDIRIni = null;
        public static Thread ModUpdatingThread;
        public static bool Exit = false;
        public static bool Ready = false;
        public static bool CheckIncludes = false;

        public static string DefaultModsPath = "";
        public static string CustomModsPath = "";

        // Constructors
        public MainForm()
        {
            InitializeComponent();
            GenerationsPatches.Clear();
            Ready = false;
        }

        // Methods
        /// <summary>
        /// Copy all the ModLoader files into a custom directory
        /// and creates a shortcut, After that it restarts the ModLoader in that custom directory
        /// </summary>
        /// <param name="path">Path to either Sonic Generations or Sonic Lost World</param>
        /// <param name="gameName">The Name of the game</param>
        /// <returns></returns>
        public bool InstallModLoader(string path, string gameName)
        {
            path = path.Replace('/', '\\');
            if (MessageBox.Show("Install SLW Mod Loader in \n" + path + "?", Resources.ApplicationTitle,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // SLWModLoader.exe, SLWModLoader.pdb, cpkredir.dll, cpkredir.ini, cpkredir.txt
                var files = new string[] { Program.ExecutableName, Path.ChangeExtension(Program.ExecutableName, "pdb"),
                    "cpkredir.dll", "cpkredir.ini", "cpkredir.txt" };

                foreach (string file in files)
                {
                    string filePath = Path.Combine(Program.StartDirectory, file);
                    if (File.Exists(filePath))
                    {
                        // Copies the current file into the custom filepath
                        File.Copy(filePath, Path.Combine(path, file), true);

                        // Don't delete this file as its in use, same thing should be done for the pdb
                        if (file == Program.ExecutableName)
                            continue;

                        // Trys o delete the old files
                        try { File.Delete(filePath); } catch { }
                    }
                    else
                    { // Missing file has been detected
                        MessageBox.Show("Could not find " + file, Program.ProgramName);
                    }
                }

                LogFile.AddMessage("Creating Shortcut for " + gameName);

                try
                {
                    // Creates a shortcut to the ModLoader
                    string shortcutPath = Path.Combine(Program.StartDirectory, $"SLWModLoader - {gameName}.lnk");
                    var wsh = new IWshRuntimeLibrary.WshShell();
                    var shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
                    shortcut.Description = $"SLWModLoader - {gameName}";
                    shortcut.TargetPath = Path.Combine(path, Program.ExecutableName);
                    shortcut.WorkingDirectory = path;
                    shortcut.Save();
                    LogFile.AddMessage("    Done.");
                }
                catch (Exception ex)
                {
                    AddMessage("Exception thrown while creating a shortcut.", ex);
                }

                // Starts the ModLoader
                var startInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(path, Program.ExecutableName),
                    Verb = "runas" // Run as Admin
                };
                Process.Start(startInfo);
                
                try
                {
                    // Trys to delete SLWModLoader.exe
                    startInfo = new ProcessStartInfo()
                    {
                        Arguments = $"/c powershell start-sleep 2 & del \"{Application.ExecutablePath}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        FileName = "cmd.exe"
                    };
                    Process.Start(startInfo);
                }
                catch { }
                return true;
            }
            return false;
        }

        public bool RunInstaller()
        {
            // Gets Steam's Registry Key
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
            // If null then try get it from the 64-bit Registry
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE\\Valve\\Steam");


            // Checks if the Key and Value exists.
            if (key != null && key.GetValue("SteamPath") is string steamPath)
            {
                // This is set to true if it installs successfully
                bool installed = false;
                string commonPath = Path.Combine(steamPath, "steamapps\\common");
                // Looks for supported games in the default library location
                if (Directory.Exists(commonPath))
                {
                    if (File.Exists(Path.Combine(commonPath, "Sonic Lost World\\slw.exe")) && !installed)
                        installed = InstallModLoader(Path.Combine(commonPath, "Sonic Lost World"),
                            "Sonic Lost World");

                    if (File.Exists(Path.Combine(commonPath, "Sonic Generations\\SonicGenerations.exe")) && !installed)
                        installed = InstallModLoader(Path.Combine(commonPath, "Sonic Generations"),
                            "Sonic Generations");
                }

                // Looks at other libraries for a supported game
                var vdf = SLWSaveForm.VDFFile.ReadVDF(Path.Combine(steamPath, "steamapps\\libraryfolders.vdf"));

                foreach (var library in vdf.Array.Elements)
                {
                    if (int.TryParse(library.Key, out int index))
                    {
                        string path = library.Value.Value as string;
                        if (Directory.Exists(Path.Combine(path, "steamapps\\common")))
                        {
                            commonPath = Path.Combine(path, "steamapps\\common");
                            if (File.Exists(Path.Combine(commonPath, "Sonic Lost World\\slw.exe")) && !installed)
                                installed = InstallModLoader(Path.Combine(commonPath, "Sonic Lost World"),
                                    "Sonic Lost World");

                            if (File.Exists(Path.Combine(commonPath, "Sonic Generations\\SonicGenerations.exe"))
                                && !installed)
                                installed = InstallModLoader(Path.Combine(commonPath, "Sonic Generations"),
                                    "Sonic Generations");
                        }
                    }
                }
                if (!installed)
                {
                    MessageBox.Show("Failed to Install SLWModLoader,\n" +
                        "It has either been declined or It could not find a supported game.",
                        Resources.ApplicationTitle, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
                else
                    return true;
            }
            else
            {
                MessageBox.Show("Could not find Steam's Registry Key, Closing...", Resources.ApplicationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// Loads all the mods from the mods folder into ModsDb
        /// </summary>
        public void LoadMods()
        {
            if (File.Exists(ModsDbPath))
            {
                LogFile.AddMessage("Found ModsDB, loading mods...");
                ModsDb = new ModsDatabase(ModsDbPath, ModsFolderPath);
                LogFile.AddMessage($"    {ModsDb.ActiveModCount} / {ModsDb.ModCount} mods are active.");
            }
            else
            {
                LogFile.AddMessage("Could not find ModsDB, creating one...");
                ModsDb = new ModsDatabase(ModsFolderPath);
            }

            LogFile.AddMessage($"Loaded total {ModsDb.ModCount} mods from \"{ModsFolderPath}\".");
        }

        public void FillModList()
        { 
            for (int i = 0; i < ModsDb.ModCount; ++i)
            {
                var modItem = ModsDb.GetMod(i);

                try
                {
                    // Checks if all Include directories are valid
                    if (modItem.GetIniFile()["Main"].ContainsParameter("IncludeDirCount") && CheckIncludes)
                        for (int i2 = 0; i2 < modItem.IncludeDirCount; ++i2)
                        {
                            string includeDir = modItem.GetIniFile()["Main"]["IncludeDir" + i2];
                            if (!Directory.Exists(Path.Combine(modItem.RootDirectory, includeDir)) && includeDir != ".")
                            {
                                if (MessageBox.Show(string.Format(Resources.InvalidIncludeDirText, modItem.Title, i2),
                                    Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                    == DialogResult.Yes)
                                    AddModForm.FixIncludeDir(i2, modItem);
                            }
                        }

                    var modListViewItem = new ListViewItem(modItem.Title);
                    modListViewItem.Tag = modItem;
                    modListViewItem.SubItems.Add(modItem.Version);
                    modListViewItem.SubItems.Add(modItem.Author);
                    modListViewItem.SubItems.Add(modItem.SaveFile.Length > 0 ? "Yes" : "No");
                    modListViewItem.SubItems.Add(modItem.UpdateServer.Length > 0 ? "Check" : "N/A");

                    modListViewItem.Checked = ModsDb.IsModActive(modItem);
                    
                    ModsList.Items.Add(modListViewItem);
                }catch(Exception ex)
                {
                    AddMessage("Exception thrown while adding mods to ModsList.", ex,
                        $"Index: {i}", $"Active Mod Count: {ModsDb.ActiveModCount}",
                        $"File Path: {ModsDb.FilePath}", $"Root Directory: {ModsDb.RootDirectory}");
                }
            }

            // Shows the no mods controls if not mods has been detected
            NoModsFoundLabel.Visible = linkLabel1.Visible = (ModsDb.ModCount <= 0);
            LogFile.AddMessage("Succesfully updated list view!");
        }

        /// <summary>
        /// Moves the sctive mods to the top of the list
        /// </summary>
        public void OrderModList()
        {
            var modsDBIni = ModsDb.getIniFile();
            int count = int.Parse(modsDBIni["Main"]["ActiveModCount"]);
            int index = 0;
            for (int i = 0; i < count; ++i)
            {
                foreach (ListViewItem lvi in ModsList.Items)
                {
                    if (lvi.Text.Equals(modsDBIni["Main"]["ActiveMod" + i]))
                    {
                        ModsList.Items.Remove(lvi);
                        ModsList.Items.Insert(index++, lvi);
                    }
                }
            }
        }

        public void RefreshModsList()
        {
            ModsList.Items.Clear();
            LoadMods();
            FillModList();
            OrderModList();
            ModsList.Select();

            // Updates the background colour on the ListView if DarkTheme is enabled
            int i = 0;
            if (CPKREDIRIni[Program.ProgramNameShort].ContainsParameter("DarkTheme") &&
                CPKREDIRIni[Program.ProgramNameShort]["DarkTheme"] == "1")
                foreach (ListViewItem lvi in ModsList.Items)
                    if (++i % 2 == 0) lvi.BackColor = Color.FromArgb(46, 46, 46);
                    else lvi.BackColor = Color.FromArgb(54, 54, 54);
        }

        public void SaveModDB()
        {
            // Deactivates All mods that are active
            ModsDb.DeactivateAllMods();
            // Activates all mods that are currently checked
            foreach (ListViewItem lvi in ModsList.CheckedItems)
                ModsDb.ActivateMod(ModsDb.GetMod(lvi.Text));

            // Saves and refreshes the mod list
            ModsDb.SaveModsDb(ModsDbPath);
            RefreshModsList();
        }
        
        public void LoadConfig()
        {
            // Checks if "cpkredir.ini" exists as SLWModLoader uses it to store its config
            if (File.Exists(Path.Combine(Program.StartDirectory, "cpkredir.ini")))
            {
                try
                {
                    CPKREDIRIni = new IniFile(Path.Combine(Program.StartDirectory, "cpkredir.ini"));
                    IniGroup group = null;
                    if (CPKREDIRIni.ContainsGroup(Program.ProgramNameShort))
                        group = CPKREDIRIni[Program.ProgramNameShort];
                    else
                    {
                        group = new IniGroup(Program.ProgramNameShort);
                        CPKREDIRIni.AddGroup(group);
                    }

                    if (group.ContainsParameter("DO A BARREL ROLL"))
                        foreach (Control control in Controls)
                            control.Font = new Font("Comic Sans MS", control.Font.Size, FontStyle.Bold);

                    if (!group.ContainsParameter("AutoCheckForUpdates"))
                        group.AddParameter("AutoCheckForUpdates", "0");
                    if (!group.ContainsParameter("KeepModLoaderOpen"))
                        group.AddParameter("KeepModLoaderOpen", "1");
                    if (!group.ContainsParameter("DarkTheme"))
                        group.AddParameter("DarkTheme", "1");
                    if (!group.ContainsParameter("CheckIncludes"))
                        group.AddParameter("CheckIncludes", "0");
                    if (!group.ContainsParameter("CustomModsDirectory"))
                        group.AddParameter("CustomModsDirectory", "0");
                    if (!group.ContainsParameter("DefaultModsPath"))
                        group.AddParameter("DefaultModsPath", "mods");
                    if (!group.ContainsParameter("CustomModsPath"))
                        group.AddParameter("CustomModsPath", "C:\\SLWMods");

                    AutoCheckUpdateCheckBox.Checked = group["AutoCheckForUpdates"] != "0";
                    KeepModLoaderOpenCheckBox.Checked = group["KeepModLoaderOpen"] != "0";
                    CheckBox_CustomModsDirectory.Checked = group["CustomModsDirectory"] != "0";
                    CheckIncludes = group["CheckIncludes"] != "0";

                    DefaultModsPath = group["DefaultModsPath"];
                    CustomModsPath = group["CustomModsPath"];

                    TextBox_CustomModsDirectory.Text = CustomModsPath;

                    if (CheckBox_CustomModsDirectory.Checked)
                    {
                        ModsFolderPath = CustomModsPath;
                        ModsDbPath = Path.Combine(ModsFolderPath, "ModsDB.ini");
                    }
                    else
                    {
                        ModsFolderPath = Path.Combine(Program.StartDirectory, "mods");
                        ModsDbPath = Path.Combine(ModsFolderPath, "ModsDB.ini");
                    }

                    TextBox_CustomModsDirectory.Enabled = CheckBox_CustomModsDirectory.Enabled;

                    EnableSaveFileRedirectionCheckBox.Checked =
                        CPKREDIRIni["CPKREDIR"]["EnableSaveFileRedirection"] != "0";
                    EnableCPKREDIRConsoleCheckBox.Checked = CPKREDIRIni["CPKREDIR"].ContainsParameter("LogType");

                    if (group["DarkTheme"] != "0")
                    {
                        ModsList.OwnerDraw = true;
                        ApplyDarkTheme(this, splitContainer.Panel1, splitContainer.Panel2, splitContainer);
                    }

                }
                catch (Exception ex)
                {
                    AddMessage("Exception thrown while loading configurations.", ex);
                }
            }
            else
            {
                MessageBox.Show("Could not find cpkredir.ini\n" +
                    "SG, LW and the ModLoader may not run correctly without this file.",
                    Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void Init()
        {
            Text += $" (v{Program.VersionString})";
            if (File.Exists(LWExecutablePath) || File.Exists(GensExecutablePath))
            {
                if (File.Exists(LWExecutablePath))
                {
                    Text += " - Sonic Lost World";
                    LogFile.AddMessage("Found Sonic Lost World.");
                    PatchGroupBox.Visible = false;
                    EnableSaveFileRedirectionCheckBox.Text += " (.sdat > .msdat)";
                }
                else if (File.Exists(GensExecutablePath))
                {
                    Text += " - Sonic Generations";
                    LogFile.AddMessage("Found Sonic Generations");

                    // Adds SG Patches
                    GenerationsPatches.Add("Enable Blue Trail", Resources.Enable_Blue_Trail);
                    GenerationsPatches.Add("Disable Blue Trail", Resources.Disable_Blue_Trail);
                    GenerationsPatches.Add("", null);
                    GenerationsPatches.Add("Enable FxPipeline", Resources.Enable_FxPipeline);
                    GenerationsPatches.Add("Disable FxPipeline", Resources.Disable_FxPipeline);

                    // Adds a Button for each patch
                    for (int i = 0; i < GenerationsPatches.Count; ++i)
                    {
                        // Ignores entries with no data
                        if (GenerationsPatches.ToList()[i].Value == null)
                            continue;

                        var btn = new Button()
                        {
                            Text = GenerationsPatches.ToList()[i].Key,
                            Size = new Size(128, 32),
                            Location = new Point(12 + 140 * (i / 3), 16 + (i % 3) * 42)
                        };
                        btn.Click += new EventHandler(PatchButton_Click);
                        PatchGroupBox.Controls.Add(btn);
                    }
                }

                LoadConfig();

                // Checks if the mods directory exists, If not, then ask to create one
                if (!Directory.Exists(ModsFolderPath))
                {
                    if (MessageBox.Show(Resources.CannotFindModsDirectoryText, Resources.ApplicationTitle,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        LogFile.AddMessage($"Creating mods folder at \"{ModsFolderPath}\"...");
                        Directory.CreateDirectory(ModsFolderPath);
                    }
                    else return;
                }

                // Loads all the mods, fills the list then reorders them
                RefreshModsList();
                
                string gameName = (File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations");
                if (!IsCPKREDIRInstalled())
                {
                    if (MessageBox.Show(string.Format(Resources.ExecutableNotPatchedText, gameName),
                        Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        InstallCPKREDIR(true);
                }
                // Sets the PatchLabel's Text to show the user if CPKREDIR is installed in either game
                PatchLabel.Text = gameName + ": " + (IsCPKREDIRInstalled() ? "Installed" : "Not Installed");
            }
            else
            { // No supported game were found
                if (MessageBox.Show(Resources.CannotFindExecutableText, Resources.ApplicationTitle,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    RunInstaller();
                else
                { // Still Couldn't find a supported game
                    MessageBox.Show(Resources.CannotFindExecutableText, Resources.ApplicationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogFile.AddMessage("Could not find executable, closing form...");
                }
                Close();
                return;
            }

            // Runs CheckForModLoaderUpdates in another thread
            new Thread(new ThreadStart(CheckForModLoaderUpdates)).Start();
            if (AutoCheckUpdateCheckBox.Checked)
            {
                ModUpdatingThread = new Thread(new ThreadStart(CheckAllModUpdates));
                ModUpdatingThread.Start();
            }

            try
            {
                // Add URI Scheme (Requires Admin)
                var key = Registry.ClassesRoot.OpenSubKey("SLWModLoader", true);
                if (key == null)
                {
                    key = Registry.ClassesRoot.CreateSubKey(Program.ProgramNameShort);
                    key.SetValue("URL Protocol", "slwmodloader");
                    key = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                    key.SetValue("", $"\"{Program.SLWModLoaderPath}\" \"%1\"");
                }
                key.Close();
            }
            catch { }

            Ready = true;
        }

        /// <summary>
        /// Does what its says, It starts the game.
        /// NOTE: Both games uses Steam's protocol to start as its required for Lost World
        ///         (steam://rungameid/329440 tells Steam to run Sonic Lost World)
        /// </summary>
        public void StartGame()
        {
            if (File.Exists(LWExecutablePath))
            {
                AddMessage("Starting Sonic Lost World...");
                Process.Start("steam://rungameid/329440");
                if (!KeepModLoaderOpenCheckBox.Checked) Close();
            }
            else if (File.Exists(GensExecutablePath))
            {
                AddMessage("Starting Sonic Generations...");
                Process.Start("steam://rungameid/71340");
                if (!KeepModLoaderOpenCheckBox.Checked) Close();
            }
        }

        
        /// <summary>
        /// Installs or Uninstalls CPKREDIR
        /// </summary>
        /// <param name="install">
        /// TRUE: Only check and Install CPKREDIR
        /// FALSE: Only check and Uninstall CPKREDIR
        /// NULL: Only check and Uninstall CPKREDIR if already installed, and vice versa
        /// </param>
        /// <returns>True if it Installs at the end</returns>
        public bool InstallCPKREDIR(bool? install)
        {
            string executablePath = GensExecutablePath;
            if (File.Exists(LWExecutablePath))
                executablePath = LWExecutablePath;
            try
            {
                AddMessage("Scaning Executable...");

                byte[] cpkredir = { 0x63, 0x70, 0x6B, 0x72, 0x65, 0x64, 0x69, 0x72 };
                byte[] imagehlp = { 0x69, 0x6D, 0x61, 0x67, 0x65, 0x68, 0x6C, 0x70 };

                var bytes = File.ReadAllBytes(executablePath);
                for (int i = 11918000; i < bytes.Length - 16; i += 2)
                {
                    // 63 70 6B 72 65 64 69 72
                    // c  p  k  r  e  d  i  r 

                    if (bytes[i + 0] == 0x63 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x6B &&
                        bytes[i + 3] == 0x72 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x64 &&
                        bytes[i + 6] == 0x69 && bytes[i + 7] == 0x72 && (install == null || install == false))
                    {
                        // Writes "imagehlp" to the executable
                        imagehlp.CopyTo(bytes, i);
                        
                        // Deletes the old executable
                        File.Delete(executablePath);

                        // Writes the newly modified executable
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Uninstalled.");
                        return false;
                    }

                    // 69 6D 61 67 65 68 6C 70
                    // i  m  a  g  e  h  l  p

                    if (bytes[i + 0] == 0x69 && bytes[i + 1] == 0x6D && bytes[i + 2] == 0x61 &&
                        bytes[i + 3] == 0x67 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x68 &&
                        bytes[i + 6] == 0x6C && bytes[i + 7] == 0x70 && (install == null || install == true))
                    {
                        // Writes "cpkredir" to the executable
                        cpkredir.CopyTo(bytes, i);

                        // Creates a backup of the executable if one hasn't been made
                        if (!File.Exists(Path.ChangeExtension(executablePath, ".Backup2.exe")))
                            File.Move(executablePath, Path.ChangeExtension(executablePath, ".Backup2.exe"));
                        else
                            File.Delete(executablePath);

                        // Write the newly modified executable
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Installed.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while installing/uninstalling CPKREDIR.", ex,
                    $"executablePath: {executablePath}", $"install: {install}");
            }
            return false;
        }

        /// <summary>
        /// Installs or Uninstalls CPKREDIR
        /// </summary>
        /// <param name="install">
        /// TRUE: Only check and Install CPKREDIR
        /// FALSE: Only check and Uninstall CPKREDIR
        /// NULL: Only check and Uninstall CPKREDIR if already installed, and vice versa
        /// </param>
        /// <returns>True if it Installs at the end</returns>
        public bool InstallCPKREDIR_OLD(bool? install)
        {
            string executablePath = GensExecutablePath;
            if (File.Exists(LWExecutablePath))
                executablePath = LWExecutablePath;
            try
            {
                AddMessage("Scaning Executable...");
                var bytes = File.ReadAllBytes(executablePath);
                for (int i = 11918000; i < bytes.Length; ++i)
                {
                    // 63 70 6B 72 65 64 69 72
                    // c  p  k  r  e  d  i  r 

                    if (bytes[i] == 0x63 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x6B &&
                        bytes[i + 3] == 0x72 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x64 &&
                        bytes[i + 6] == 0x69 && bytes[i + 7] == 0x72 && (install == null || install == false))
                    {
                        // Writes "imagehlp" to the executable
                        bytes[i] = 0x69;
                        bytes[i + 1] = 0x6D;
                        bytes[i + 2] = 0x61;
                        bytes[i + 3] = 0x67;
                        bytes[i + 4] = 0x65;
                        bytes[i + 5] = 0x68;
                        bytes[i + 6] = 0x6C;
                        bytes[i + 7] = 0x70;

                        // Deletes the old executable
                        File.Delete(executablePath);

                        // Writes the newly modified executable
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Uninstalled.");
                        return false;
                    }

                    // 69 6D 61 67 65 68 6C 70
                    // i  m  a  g  e  h  l  p

                    if (bytes[i] == 0x69 && bytes[i + 1] == 0x6D && bytes[i + 2] == 0x61 &&
                        bytes[i + 3] == 0x67 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x68 &&
                        bytes[i + 6] == 0x6C && bytes[i + 7] == 0x70 && (install == null || install == true))
                    {
                        // Writes "cpkredir" to the executable
                        bytes[i] = 0x63;
                        bytes[i + 1] = 0x70;
                        bytes[i + 2] = 0x6B;
                        bytes[i + 3] = 0x72;
                        bytes[i + 4] = 0x65;
                        bytes[i + 5] = 0x64;
                        bytes[i + 6] = 0x69;
                        bytes[i + 7] = 0x72;

                        // Creates a backup of the executable if one hasn't been made
                        if (!File.Exists(Path.ChangeExtension(executablePath, ".Backup2.exe")))
                            File.Move(executablePath, Path.ChangeExtension(executablePath, ".Backup2.exe"));
                        else
                            File.Delete(executablePath);

                        // Write the newly modified executable
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Installed.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while installing/uninstalling CPKREDIR.", ex,
                    $"executablePath: {executablePath}", $"install: {install}");
            }
            return false;
        }

        public bool IsCPKREDIRInstalled()
        {
            try
            {
                var bytes = File.ReadAllBytes(File.Exists(LWExecutablePath)
                    ? LWExecutablePath : GensExecutablePath);
                for (int i = 11918000; i < bytes.Length - 16; i += 2)
                {
                    // 63 70 6B 72 65 64 69 72
                    // c  p  k  r  e  d  i  r 

                    if (bytes[i + 0] == 0x63 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x6B &&
                        bytes[i + 3] == 0x72 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x64 &&
                        bytes[i + 6] == 0x69 && bytes[i + 7] == 0x72)
                        return true;

                    // 69 6D 61 67 65 68 6C 70
                    // i  m  a  g  e  h  l  p

                    if (bytes[i + 0] == 0x69 && bytes[i + 1] == 0x6D && bytes[i + 2] == 0x61 &&
                        bytes[i + 3] == 0x67 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x68 &&
                        bytes[i + 6] == 0x6C && bytes[i + 7] == 0x70)
                        return false;
                }
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while checking executable.", ex);
            }
            AddMessage("Failed to check executable");
            return false;
        }

        // GUI Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit = true;
            if (ModUpdatingThread != null && ModUpdatingThread.IsAlive)
                while (ModUpdatingThread.IsAlive)
                    Thread.Sleep(1000);

            LogFile.AddEmptyLine();
            LogFile.AddMessage("The form has been closed.");

            LogFile.Close();
            if (CPKREDIRIni != null)
                CPKREDIRIni.Save();
        }

        #region Logging

        public void AddMessage(string message)
        {
            LogFile.AddMessage(message);
            // Adds a character limit if not compiled in debug
            #if !DEBUG
            if (message.Length < 128)
            #endif
            Invoke(new Action(() => StatusLabel.Text = message));
        }

        public static void AddMessage(string message, Exception exception, params string[] extraData)
        {
            if (Exit) return;
            LogFile.AddMessage(message);
            LogFile.AddMessage($"    Exception: {exception}");
            if (extraData != null)
            {
                LogFile.AddMessage("    Extra Data: ");
                foreach (string s in extraData)
                    LogFile.AddMessage($"        {s}");
            }
            MessageBox.Show(Resources.ExceptionText, Program.ProgramName);
            #if DEBUG
            throw exception;
            #endif
        }

        #endregion Logging

        #region Updating methods (A bit messy)

        /// <summary>
        /// Searches for any new ModLoader updates
        /// NOTE: The method is really messy and can also easily fail
        /// </summary>
        public void CheckForModLoaderUpdates()
        {
            try
            {
                LogFile.AddMessage("Checking for Updates...");

                var webClient = new WebClient();
                string url = "https://api.github.com/repos/thesupersonic16/SLW-Mod-Loader/releases";
                string data = "";
                string updateUrl = "";
                string releaseBody = "";
                string currentVersion = Program.VersionString;
                string latestVersion = "0.0";

                webClient.Headers.Add("user-agent", Program.UserAgent);
                data = webClient.DownloadString(url);
                latestVersion = Program.GetStringAfter("tag_name", data);
                updateUrl =     Program.GetStringAfter("browser_download_url", data);
                releaseBody =   Program.EscapeString(Program.GetStringAfter("body", data));
                
                // If true, then a new update is available
                if (latestVersion != currentVersion)
                {
                    AddMessage($"New Update Found v{latestVersion}");
                    if (new ChangeLogForm(latestVersion, releaseBody, updateUrl).ShowDialog() == DialogResult.Yes)
                    {
                        Invoke(new Action(() => Visible = false));
                        AddMessage("Starting Update...");
                        new UpdateForm(updateUrl).ShowDialog();
                    }
                    else
                        AddMessage("Update Canceled. :(");
                }
                else
                    LogFile.AddMessage("No updates are available.");
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while checking for Mod Loader updates.", ex);
            }
        }

        /// <summary>
        /// Checks for updates for all mods
        /// </summary>
        public void CheckAllModUpdates()
        {
            // Amount of loaded mods
            int count = 0;
            Invoke(new Action(() => count = ModsList.Items.Count));

            for (int i = 0; i < count; ++i)
            {
                Mod mod = null;
                ListViewItem item = null;
                Invoke(new Action(() => item = ModsList.Items[i]));
                if (item == null)
                    continue;
                Invoke(new Action(() => mod = ModsDb.GetMod(item.Text)));
                if (mod.UpdateServer.Length != 0)
                {
                    // TODO: Find a way to get the Update SubItem without hardcoding a number
                    Invoke(new Action(() => item.SubItems[4].Text = "Checking..."));
                    string status = CheckForModUpdates(mod.Title, true);
                    if (Exit)
                        return;
                    Invoke(new Action(() => item.SubItems[4].Text = status));
                }
            }
        }

        /// <summary>
        /// Checks for individual mod updates
        /// </summary>
        /// <param name="modName">The Name of the mod (Frendly Name)</param>
        /// <param name="silent">True If you don't want dialog constently comming up</param>
        /// <returns>The Status</returns>
        public string CheckForModUpdates(string modName, bool silent = false)
        {
            var modUpdater = new ModUpdater();
            string status = "";
            var mod = ModsDb.GetMod(modName ?? ModsList.FocusedItem.Text);
            if (mod == null) return status;

            if (mod.UpdateServer.Length == 0 && mod.Url.Length != 0)
            { // No Update Server, But has Website
                if (!silent && MessageBox.Show(string.Format(Resources.NoUpdateServerText,
                    mod.Title, mod.Url), Program.ProgramName, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                        if (Program.IsURL(mod.Url))
                            Process.Start(mod.Url);
            }
            else if (mod.UpdateServer.Length == 0 && mod.Url.Length == 0)
            { // No Update Server and Website
                if (!silent) MessageBox.Show(string.Format(Resources.NoUpdateServerAndURLText, mod.Title),
                    Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (mod.UpdateServer.Length != 0)
            { // Has Update Server
                try
                {
                    var webClient = new WebClient();
                    string data = webClient.DownloadString(mod.UpdateServer);

                    if (mod.UpdateServer.EndsWith(".txt"))
                    { // TXT file.
                        if (!silent)
                            MessageBox.Show("Not Implemented Yet", Program.ProgramName, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        status = "Not Implemented";
                    }
                    else if (mod.UpdateServer.EndsWith(".xml"))
                    { // XML file
                        var xml = XDocument.Parse(data);
                        var update = modUpdater.GetUpdateFromXML(mod, xml);
                        string updateStatus = UpdateMod(mod, update);
                        if (!silent && updateStatus == "Up to date")
                            MessageBox.Show($"{mod.Title} is already up to date.", Program.ProgramName);
                    }
                    else if (mod.UpdateServer.EndsWith(".ini"))
                    { // mod_version.ini file
                        var update = modUpdater.GetUpdateFromINI(mod, data);
                        string updateStatus = UpdateMod(mod, update);
                        if (!silent && updateStatus == "Up to date")
                            MessageBox.Show($"{mod.Title} is already up to date.", Program.ProgramName);
                    }
                }
                catch (Exception ex)
                {
                    AddMessage("Exception thrown while updating.", ex, $"Update Server: {mod.UpdateServer}");
                    status = "Failed";
                }
            }
            return status;
        }

        public string UpdateMod(Mod mod, ModUpdater.ModUpdate update)
        {
            if (update.VersionString != mod.Version)
            {
                if (MessageBox.Show(string.Format(Resources.ModUpdateText, update.Name,
                    mod.Version, update.VersionString, update.DownloadSizeString), Program.ProgramName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new UpdateModForm(mod, update).ShowDialog();
                    return "Done";
                }
                else
                    return "Available";
            }
            else
                return "Up to date";
        }

        #endregion

        #region ButtonEvents

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshModsList();
        }

        private void SaveAndPlayButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveModDB();
                AddMessage("ModsDB Saved");
                StartGame();
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while saving ModsDB and starting.", ex,
                $"Active Mod Count: {ModsDb.ActiveModCount}", $"File Path: {ModsDb.FilePath}",
                $"Root Directory: {ModsDb.RootDirectory}");
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveModDB();
                AddMessage("ModsDB Saved");
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while saving ModsDB.", ex,
                $"Active Mod Count: {ModsDb.ActiveModCount}", $"File Path: {ModsDb.FilePath}",
                $"Root Directory: {ModsDb.RootDirectory}");
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartGame();
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while starting the game.", ex);
                Close();
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void ScanExecutableButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            string gameName = (File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations");
            PatchLabel.Text = gameName + ": " + (IsCPKREDIRInstalled() ? "Installed" : "Not Installed");
        }

        private void AddModButton_Click(object sender, EventArgs e)
        {
            new AddModForm().ShowDialog();
            RefreshModsList();
        }

        private void InstallUninstallButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            string gameName = File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations";
            PatchLabel.Text = gameName + ": " + (InstallCPKREDIR(null) ? "Installed" : "Not Installed");
        }

        private void RemoveModButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to delete \"{ModsList.FocusedItem.Text}\"?",
                Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Directory.Delete(ModsDb.GetMod(ModsList.FocusedItem.Text).RootDirectory, true);
                RefreshModsList();
                SaveModDB();
            }
            ModsList.FocusedItem = null;
            RemoveModButton.Enabled = false;
        }

        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            // Checks if an item is selected and it is not the first item on the list
            if (ModsList.FocusedItem == null || ModsList.FocusedItem.Index == 0) return;
            // Gets the mod item so we can remove it from the list
            var lvi = ModsList.FocusedItem;
            // Gets the position of the selected mod so we can move it up by 1
            int pos = ModsList.Items.IndexOf(lvi) - 1;
            // Removes the mod and reinserts it back into the list above where it was before
            ModsList.Items.Remove(lvi);
            ModsList.Items.Insert(pos, lvi);
            // Selects the moved item
            ModsList.FocusedItem = lvi;
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            // Checks if an item is selected and it is not the last item on the list
            if (ModsList.FocusedItem == null && ModsList.FocusedItem.Index > ModsList.Items.Count) return;
            // Checks if the item that is currently selected is not the last checked item
            if (ModsList.FocusedItem.Index >= ModsList.CheckedItems.Count - 1) return;
            // Gets the mod item so we can remove it from the list
            var lvi = ModsList.FocusedItem;
            // Gets the position of the selected mod so we can move it down by 1
            int pos = ModsList.Items.IndexOf(lvi) + 1;
            // Removes the mod and reinserts it back in after where it used to be
            ModsList.Items.Remove(lvi);
            ModsList.Items.Insert(pos, lvi);
            // Selects the moved item.
            ModsList.FocusedItem = lvi;
        }

        private void MoveUpAll_Click(object sender, EventArgs e)
        {
            // Checks if a item is selected and is not the first item
            if (ModsList.FocusedItem == null || ModsList.FocusedItem.Index == 0) return;
            // Gets the mod item so we can remove it from the list and add it back in
            var lvi = ModsList.FocusedItem;
            // Removes the mod from the mod list
            ModsList.Items.Remove(lvi);
            // Adds the mod to the start of the mod list
            ModsList.Items.Insert(0, lvi);

            // Selects the moved item
            ModsList.FocusedItem = lvi;
        }

        private void MoveDownAll_Click(object sender, EventArgs e)
        {
            // Checks if there is an item selected and that its not at the end of the list
            if (ModsList.FocusedItem == null && ModsList.Items.Count > ModsList.FocusedItem.Index) return;
            // Checks if the item that is selected is not the last checked item
            if (ModsList.FocusedItem.Index >= ModsList.CheckedItems.Count - 1) return;
            // Gets the mod item so we can remove it from the list and add it back in
            var lvi = ModsList.FocusedItem;
            // Removes the mod from the mod list
            ModsList.Items.Remove(lvi);
            // Adds the mod to the start of the mod list
            ModsList.Items.Insert(ModsList.CheckedItems.Count, lvi);
            // Selects the moved item
            ModsList.FocusedItem = lvi;
        }

        private void PatchButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(GensExecutablePath))
            {
                try
                {
                    // Creates a backup of the executable if one hasn't been made
                    if (!File.Exists(Path.ChangeExtension(GensExecutablePath, ".Backup2.exe")))
                        File.Copy(GensExecutablePath, Path.ChangeExtension(GensExecutablePath, ".Backup2.exe"), true);

                    string patchName = ((Button)sender).Text;
                    var patchData = GenerationsPatches[patchName];
                    
                    LogFile.AddMessage($"Installing {patchName}");

                    // Opens a FileStream to the Generations executable
                    using (var outStream = File.OpenWrite(GensExecutablePath))
                    {
                        var reader = new BinaryReader(new MemoryStream(patchData));
                        var writer = new BinaryWriter(outStream);

                        // Amount of blocks
                        int blockCount = reader.ReadInt32();
                        for (int i = 0; i < blockCount; ++i)
                        {
                            // Seek to the start of the block is going to written at (offset)
                            writer.Seek(reader.ReadInt32(), SeekOrigin.Begin);
                            // Gets the size of the current block
                            int blockSize = reader.ReadInt32();
                            // Writes the block to the stream
                            writer.Write(reader.ReadBytes(blockSize));
                        }
                        // Closes the reader
                        reader.Close();
                    }
                    MessageBox.Show("Done.", Program.ProgramName);
                    LogFile.AddMessage($"Finished Installing {patchName}");
                }
                catch (Exception ex)
                {
                     AddMessage("Exception thrown while applying a patch.", ex,
                        "Button Name: " + ((Button)sender).Text);
                }
            }
        }

        private void Button_SaveAndReload_Click(object sender, EventArgs e)
        {
            CPKREDIRIni[Program.ProgramNameShort]["CustomModsPath"] = TextBox_CustomModsDirectory.Text;
            CPKREDIRIni[Program.ProgramNameShort]["CustomModsPath"] = TextBox_CustomModsDirectory.Text;
            CPKREDIRIni[Program.ProgramNameShort]["CustomModsDirectory"] = CheckBox_CustomModsDirectory.Checked ? "1" : "0";
            CPKREDIRIni["CPKREDIR"]["ModsDbIni"] = CheckBox_CustomModsDirectory.Checked ?
                Path.Combine(CPKREDIRIni[Program.ProgramNameShort]["CustomModsPath"], "ModsDB.ini") :
                Path.Combine(CPKREDIRIni[Program.ProgramNameShort]["DefaultModsPath"], "ModsDB.ini");
            if (CPKREDIRIni != null)
                CPKREDIRIni.Save();

            Program.Restart = true;
            Close();
        }

        #endregion ButtonEvents

        #region ToolStripMenuItemEvents

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForModUpdates(ModsList.FocusedItem.Text);
        }

        private void DeleteModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete \"" + ModsList.FocusedItem.Text + "\"?",
                Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Directory.Delete(ModsDb.GetMod(ModsList.FocusedItem.Text).RootDirectory, true);
                RefreshModsList();
                SaveModDB();
            }
            // Deselect all items.
            ModsList.FocusedItem = null;
            ModsList_SelectedIndexChanged(null, null);
        }

        private void OpenModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", ModsDb.GetMod(ModsList.FocusedItem.Text).RootDirectory);
        }

        private void DesciptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mod = ModsDb.GetMod(ModsList.FocusedItem.Text);
            new DescriptionForm(mod).ShowDialog();
        }

        private void EditModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var nmf = new NewModForm(ModsDb.GetMod(ModsList.FocusedItem.Text));
            nmf.ShowDialog();
            RefreshModsList();
        }

        private void CreateModUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var modified = ModsDb.GetMod(ModsList.FocusedItem.Text);
            new CreateUpdateForm(modified).ShowDialog();
        }

        #endregion ToolStripMenuItemEvents

        #region OtherGUIEvents

        private void ReportLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/thesupersonic16/SLW-Mod-Loader/issues/new");
        }

        private void ModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MoveUpAll.Enabled = MoveDownAll.Enabled = MoveUpButton.Enabled = MoveDownButton.Enabled
                = RemoveModButton.Enabled = createModUpdateToolStripMenuItem.Enabled = editModToolStripMenuItem.Enabled
                = deleteModToolStripMenuItem.Enabled = checkForUpdatesToolStripMenuItem.Enabled
                = desciptionToolStripMenuItem.Enabled = openModFolderToolStripMenuItem.Enabled
                = ModsList.SelectedItems.Count == 1;
        }

        private void ModsList_DragDrop(object sender, DragEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    // Checks if what is being dragged in is a FileDrop, And that its a list of files
                    if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                        e.Data.GetData(DataFormats.FileDrop) is string[] files)
                    {
                        // Checks if there is only one file being dragged in, And its a .sdat file
                        if (files.Length == 1 && Path.GetExtension(files[0]) == ".sdat")
                        {
                            new SLWSaveForm(files[0]).ShowDialog();
                            return;
                        }

                        // The previous mod count
                        int prevModCount = ModsDb.ModCount;

                        Invoke(new Action(() => AddMessage("Installing mods...")));

                        foreach (string file in files)
                        {
                            string extension = Path.GetExtension(file);
                            // Checks if its a Directory
                            if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                                AddModForm.InstallFromFolder(file); // Install from a folder
                            else if (extension == ".zip")
                                AddModForm.InstallFromZip(file); // Install from a zip
                            else if (extension == ".7z" || extension == ".rar")
                                AddModForm.InstallFrom7zArchive(file); // Install from a 7z or rar

                            // Refreshes mod list
                            Invoke(new Action(() => RefreshModsList()));
                        }
                        Invoke(new Action(() => AddMessage("Finished Mod Installation. Installed " +
                            (ModsDb.ModCount - prevModCount) + " mod(s).")));
                    }
                }
                catch (Exception ex)
                {
                    object data = e.Data.GetData(DataFormats.FileDrop);
                    string dataType = data == null ? "NULL" : data.GetType().ToString();
                    AddMessage("Exception thrown while handling drag and drop.", ex,
                        $"Data Present: {e.Data.GetDataPresent(DataFormats.FileDrop)}", $"Data Type: {dataType}");
                }
            }).Start();
        }

        private void ModsList_DragEnter(object sender, DragEventArgs e)
        {
            // Checks if what is being dragged in is a FileDrop, And that its a list of files
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop) is string[] files)
                e.Effect = DragDropEffects.Copy;
        }

        private void TextBox_CustomModsDirectory_DoubleClick(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Select Directory",
                FileName = "Enter into a directory and press Save"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
                TextBox_CustomModsDirectory.Text = Path.GetDirectoryName(sfd.FileName);
        }

        // Configuration Checkbox Event
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CPKREDIRIni != null && Ready)
            {
                CPKREDIRIni[Program.ProgramNameShort]["AutoCheckForUpdates"] = AutoCheckUpdateCheckBox.Checked ? "1" : "0";
                CPKREDIRIni[Program.ProgramNameShort]["KeepModLoaderOpen"] = KeepModLoaderOpenCheckBox.Checked ? "1" : "0";
                CPKREDIRIni[Program.ProgramNameShort]["CustomModsDirectory"] = CheckBox_CustomModsDirectory.Checked ? "1" : "0";
                CPKREDIRIni["CPKREDIR"]["EnableSaveFileRedirection"] = EnableSaveFileRedirectionCheckBox.Checked ? "1" : "0";

                TextBox_CustomModsDirectory.Enabled = CheckBox_CustomModsDirectory.Checked;
                
                if (EnableCPKREDIRConsoleCheckBox.Checked && !CPKREDIRIni["CPKREDIR"].ContainsParameter("LogType"))
                    CPKREDIRIni["CPKREDIR"].AddParameter("LogType", "console");
                else
                    CPKREDIRIni["CPKREDIR"].RemoveParameter("LogType");
            }
        }


        #endregion OtherGUIEvents

        #region Don't Look!

        private void ModsList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Colours
            var dark1 = Color.FromArgb(34, 34, 34);
            var dark2 = Color.FromArgb(70, 70, 70);
            
            // Draws the Header
            if (e.Bounds.Contains(ModsList.PointToClient(MousePosition)))
                 e.Graphics.FillRectangle(new SolidBrush(dark1), e.Bounds);
            else e.Graphics.FillRectangle(new SolidBrush(dark2), e.Bounds);
            var point = new Point(0, 6);
            point.X = e.Bounds.X;
            var column = ModsList.Columns[e.ColumnIndex];
            e.Graphics.FillRectangle(new SolidBrush(dark1), point.X, 0, 2, e.Bounds.Height);
            point.X += column.Width / 2 - TextRenderer.MeasureText(column.Text, ModsList.Font).Width/2;
            TextRenderer.DrawText(e.Graphics, column.Text, ModsList.Font, point, ModsList.ForeColor);
        }

        private void ModsList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }


        // Theme Functions
        public static void AddAllChildControls(Control control, List<Control> controls)
        {
            controls.Add(control);
            foreach (Control control2 in control.Controls)
                AddAllChildControls(control2, controls);
        }

        /// <summary>
        /// It Applies a horrible dark theme to control and to all the controls
        /// </summary>
        /// <param name="control">The Main control that the dark them is being applied to</param>
        /// <param name="controls">Extra controls to apply the theme to</param>
        /// <returns></returns>
        public static bool ApplyDarkTheme(Control control, params Control[] controls)
        {
            if (CPKREDIRIni[Program.ProgramNameShort].ContainsParameter("DarkTheme") && CPKREDIRIni[Program.ProgramNameShort]["DarkTheme"] == "1")
            {
                LogFile.WriteMessage($"Applying Dark Theme to {control.GetType().Name}...", true);

                var allControls = new List<Control>();

                AddAllChildControls(control, allControls);

                foreach (var control0 in controls)
                {
                    if (!allControls.Contains(control0)) allControls.Add(control0);
                    foreach (Control control1 in control0.Controls)
                        if (!allControls.Contains(control1))
                            allControls.Add(control1);
                }

                foreach (Control control0 in allControls)
                {
                    control0.BackColor = Color.FromArgb(46, 46, 46);
                    if (control0.ForeColor == Color.Black || control0.ForeColor == SystemColors.WindowText ||
                        control0.ForeColor == SystemColors.ControlText)
                        control0.ForeColor = Color.FromArgb(200, 200, 180);

                    if (control0.GetType() == typeof(Button))
                    {
                        ((Button)control0).FlatStyle = FlatStyle.Flat;
                        control0.BackColor = Color.FromArgb(54, 54, 54);
                    }

                    if (control0.GetType() == typeof(RadioButton))
                        ((RadioButton)control0).FlatStyle = FlatStyle.Flat;

                    if (control0.GetType() == typeof(GroupBox))
                        ((GroupBox)control0).ForeColor = Color.FromArgb(200, 200, 180);


                    if (control0.GetType() == typeof(StatusStrip))
                        control0.BackColor = Color.FromArgb(54, 54, 54);

                    if (control0.GetType() == typeof(TabPage) || control0.GetType() == typeof(LinkLabel) ||
                        control0.GetType() == typeof(CheckBox) || control0.GetType() == typeof(GroupBox) ||
                        control0.GetType() == typeof(Label))
                        control0.BackColor = Color.FromArgb(46, 46, 46);

                    if (control0.GetType() == typeof(ListView))
                    {
                        ((ListView)control0).OwnerDraw = true;
                        int i = 0;
                        foreach (ListViewItem lvi in ((ListView)control0).Items)
                            if (++i % 2 == 0) lvi.BackColor = Color.FromArgb(46, 46, 46);
                            else lvi.BackColor = Color.FromArgb(54, 54, 54);
                    }

                }
                LogFile.WriteMessage(" Done.\r\n", false);
                return true;
            }
            return false;
        }
        #endregion

    }
}