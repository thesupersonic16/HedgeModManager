using System;
using System.IO;
using System.Windows.Forms;
using SLWModLoader.Properties;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.IO.Compression;
using System.Linq;
using Microsoft.Win32;
using System.Text;

namespace SLWModLoader
{
    public partial class MainForm : Form
    {
        public static readonly string GensExecutablePath = Path.Combine(Program.StartDirectory, "SonicGenerations.exe");
        public static readonly string LWExecutablePath = Path.Combine(Program.StartDirectory, "slw.exe");
        public static readonly string ModsFolderPath = Path.Combine(Program.StartDirectory, "mods");
        public static readonly string ModsDbPath = Path.Combine(ModsFolderPath, "ModsDB.ini");
        public static Dictionary<string, byte[]> GenerationsPatches = new Dictionary<string, byte[]>();
        public ModsDatabase ModsDb;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogFile.AddEmptyLine();
            LogFile.AddMessage("The form has been closed.");

            LogFile.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text += $" (v{Program.VersionString})";

            if (File.Exists(LWExecutablePath))
            {
                Text += " - Sonic Lost World";
                LogFile.AddMessage("Found Sonic Lost World.");
                PatchGroupBox.Visible = false;
            }
            if (File.Exists(GensExecutablePath))
            {
                Text += " - Sonic Generations";
                LogFile.AddMessage("Found Sonic Generations");
                GenerationsPatches.Add("Enable Blue Trail", Resources.Enable_Blue_Trail);
                GenerationsPatches.Add("Disable Blue Trail", Resources.Disable_Blue_Trail);
                GenerationsPatches.Add("", null);
                GenerationsPatches.Add("Enable FxPipeline", Resources.Enable_FxPipeline);
                GenerationsPatches.Add("Disable FxPipeline", Resources.Disable_FxPipeline);

                for (int i = 0; i < GenerationsPatches.Count; i++)
                {
                    if (GenerationsPatches.ToList()[i].Value == null)
                        continue;
                    Button btn = new Button()
                    {
                        Text = GenerationsPatches.ToList()[i].Key,
                        Size = new System.Drawing.Size(128, 32),
                        Location = new System.Drawing.Point(12 + 140 * (i / 3), 16 + (i % 3) * 42)
                    };
                    btn.Click += new EventHandler(PatchButton_Click);
                    PatchGroupBox.Controls.Add(btn);
                }
            }

            if (!Directory.Exists(ModsFolderPath))
            {
                if (MessageBox.Show(Resources.CannotFindModsDirectoryText, Resources.ApplicationTitle,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    LogFile.AddMessage($"Creating mods folder at \"{ModsFolderPath}\"...");
                    Directory.CreateDirectory(ModsFolderPath);
                }
            }

            if (File.Exists(LWExecutablePath) || File.Exists(GensExecutablePath))
            {
                LoadMods();
                OrderModList();
                if(!isCPKREDIRInstalled())
                {
                    if (MessageBox.Show("Your "+(File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations") +
                        " executable has not yet been Installed for use with CPKREDIR, which is required to load mods.\nWould you like to patch it now?",
                        Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        InstallCPKREDIR(true);
                    }
                }
                PatchLabel.Text = (File.Exists(LWExecutablePath) ? Path.GetFileName(LWExecutablePath) : Path.GetFileName(GensExecutablePath)) +
                ": " + (isCPKREDIRInstalled() ? "Installed" : "Not Installed");
            }
            else
            {
                if (MessageBox.Show("SLW Mod Loader could not find a game executable in its startup directory.\n" +
                    "The mod loader can attempt to look for your game and install itself automatically.\n",
                    Resources.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    // Gets the 32 bit Registry Key.
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
                    // If null then try to get the 64 bit Registry Key.
                    if (key == null)
                        key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Valve\\Steam");
                    // Checks if the Key and Value exists.
                    if (key != null && key.GetValue("SteamPath") is string steamPath)
                    {
                        bool installed = false;
                        // Looks for games in the default location. 
                        if (Directory.Exists(Path.Combine(steamPath, "steamapps\\common")))
                        {
                            if (File.Exists(Path.Combine(steamPath, "steamapps\\common\\Sonic Lost World\\slw.exe")) && !installed)
                                installed = InstallModLoader(Path.Combine(steamPath, "steamapps\\common\\Sonic Lost World"), "Sonic Lost World");

                            if (File.Exists(Path.Combine(steamPath, "steamapps\\common\\Sonic Generations\\SonicGenerations.exe")) && !installed)
                                installed = InstallModLoader(Path.Combine(steamPath, "steamapps\\common\\Sonic Generations"), "Sonic Generations");
                        }
                        // Looks for other locations. 
                        var libraryfolders = File.ReadAllLines(Path.Combine(steamPath, "steamapps\\libraryfolders.vdf"));
                        int i = 1;
                        foreach (string s in libraryfolders)
                        {
                            if (s.IndexOf("\"" + i + "\"") != -1)
                            {
                                // Gets the location.
                                var libraryLocation = s.Substring(s.IndexOf("\t\t\"") + 3, s.LastIndexOf('"') - (s.IndexOf("\t\t\"") + 3));
                                // Looks for games in that location.
                                if (Directory.Exists(Path.Combine(libraryLocation, "steamapps\\common")))
                                {
                                    if (File.Exists(Path.Combine(libraryLocation, "steamapps\\common\\Sonic Lost World\\slw.exe")) && !installed)
                                        installed = InstallModLoader(Path.Combine(libraryLocation, "steamapps\\common\\Sonic Lost World"),
                                            "Sonic Lost World");

                                    if (File.Exists(Path.Combine(libraryLocation, "steamapps\\common\\Sonic Generations\\SonicGenerations.exe"))
                                        && !installed)
                                        installed = InstallModLoader(Path.Combine(libraryLocation, "steamapps\\common\\Sonic Generations"),
                                            "Sonic Generations");
                                }
                                i++;
                            }

                        }
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show(Resources.CannotFindExecutableText, Resources.ApplicationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogFile.AddMessage("Could not find executable, closing form...");
                    Close();
                }
                return;
            }

            new Thread(new ThreadStart(CheckForModLoaderUpdates)).Start();
        }

        public bool InstallModLoader(string path, string gameName)
        {
            path = path.Replace('/', '\\');
            if (MessageBox.Show("Install SLW Mod Loader in \n"+path+"?", Resources.ApplicationTitle,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string[] files = new string[] { Program.ExecutableName, "cpkredir.dll", "cpkredir.ini", "cpkredir.txt" };
                foreach (string file in files)
                {
                    var filePath = Path.Combine(Program.StartDirectory, file);
                    if(File.Exists(filePath))
                    {
                        File.Copy(filePath, Path.Combine(path, file), true);
                        try { File.Delete(filePath); } catch { }
                    }
                    else
                    {
                        MessageBox.Show("Could not find "+file+".");
                    }
                }

                LogFile.AddMessage("Creating Shortcut for " + gameName);
                // Creates a shortcut to the modloader.
                string shortcutPath = Path.Combine(Program.StartDirectory, $"SLWModLoader - {gameName}.lnk");
                IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.Description = "SLWModLoader - "+gameName;
                shortcut.TargetPath = Path.Combine(path, Program.ExecutableName);
                shortcut.WorkingDirectory = path;
                shortcut.Save();
                LogFile.AddMessage("    Done.");

                MessageBox.Show("Done.");

                return true;
            }
            return false;
        }

        public void LoadMods()
        {
            if (File.Exists(ModsDbPath))
            {
                LogFile.AddMessage("Found ModsDB, loading mods...");
                ModsDb = new ModsDatabase(ModsDbPath, ModsFolderPath);
                LogFile.AddMessage($"\t{ModsDb.ActiveModCount} / {ModsDb.ModCount} mods are active.");
            }
            else
            {
                LogFile.AddMessage("Could not find ModsDB, creating one...");
                ModsDb = new ModsDatabase(ModsFolderPath);
            }

            LogFile.AddMessage($"Loaded total {ModsDb.ModCount} mods from \"{ModsFolderPath}\".");

            for (int i = 0; i < ModsDb.ModCount; ++i)
            {
                Mod modItem = ModsDb.GetMod(i);

                ListViewItem modListViewItem = new ListViewItem(modItem.Title);
                modListViewItem.Tag = modItem;
                modListViewItem.SubItems.Add(modItem.Version);
                modListViewItem.SubItems.Add(modItem.Author);
                modListViewItem.SubItems.Add(modItem.SaveFile.Length > 0 ? "Yes" : "No");
                modListViewItem.SubItems.Add(modItem.UpdateServer.Length > 0 ? "Check" : "N/A");

                if (ModsDb.IsModActive(modItem))
                {
                    modListViewItem.Checked = true;
                }

                ModsList.Items.Add(modListViewItem);
            }

            NoModsFoundLabel.Visible = linkLabel1.Visible = (ModsDb.ModCount <= 0);
            LogFile.AddMessage("Succesfully updated list view!");
        }

        // Move Active mods to the top of the list
        public void OrderModList()
        {
            IniFile modsDBIni = ModsDb.getIniFile();
            int count = int.Parse(modsDBIni["Main"]["ActiveModCount"]);
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                foreach (ListViewItem lvi in ModsList.Items)
                {
                    if(lvi.Text.Equals(modsDBIni["Main"]["ActiveMod" + i]))
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
            OrderModList();
            ModsList.Select();
        }

        private void SaveModDB()
        {
            // Getting the ModDb.ini file
            IniFile modsDBIni = ModsDb.getIniFile();
            // Deactivates All mods that were active
            ModsDb.DeactivateAllMods();
            // Activates all mods that are checked
            foreach (ListViewItem lvi in ModsList.Items)
            {
                if(lvi.Checked)
                    ModsDb.ActivateMod(ModsDb.GetMod(lvi.Text));
            }
            
            // Saving and refreshing the mod list
            ModsDb.SaveModsDb(ModsDbPath);
            RefreshModsList();

        }

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
                AddMessage("Exception thrown while saving ModsDB and starting.", ex, new String[]
                { $"Active Mod Count: {ModsDb.ActiveModCount}", $"File Path: {ModsDb.FilePath}", $"Root Directory: {ModsDb.RootDirectory}" });
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
                AddMessage("Exception thrown while saving ModsDB.", ex, new String[]
                { $"Active Mod Count: {ModsDb.ActiveModCount}", $"File Path: {ModsDb.FilePath}", $"Root Directory: {ModsDb.RootDirectory}" });
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
                AddMessage("Exception thrown while starting a game.", ex);
                Close();
            }
        }

        public void StartGame()
        {
            if (File.Exists(LWExecutablePath))
            {
                AddMessage("Starting Sonic Lost World...");
                Process.Start("steam://rungameid/329440");
                Close();
            }
            else if (File.Exists(GensExecutablePath))
            {
                AddMessage("Starting Sonic Generations...");
                Process.Start("steam://rungameid/71340");
                Close();
            }
        }

        public void CheckForModLoaderUpdates()
        {
            try
            {
                LogFile.AddMessage("Checking for Updates...");

                var webClient = new WebClient();
                var latestReleaseJson = string.Empty;
                var url = "https://api.github.com/repos/Radfordhound/SLW-Mod-Loader/releases/latest";
                var updateUrl = string.Empty;
                float latestVersion = 0;
                float currentVersion = Convert.ToSingle(Program.VersionString);

                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
                latestReleaseJson = webClient.DownloadString(url);
                latestVersion = Convert.ToSingle(latestReleaseJson.Substring(latestReleaseJson.IndexOf("tag_name") + 12, 3));
                updateUrl = latestReleaseJson.Substring(latestReleaseJson.IndexOf("browser_download_url") + 24,
                            latestReleaseJson.LastIndexOf(".zip") + 4 - (latestReleaseJson.IndexOf("browser_download_url") + 24));
                // If true then a new update is available
                if (latestVersion > currentVersion)
                {
                    LogFile.AddMessage("New Update Found v" + latestVersion);
                    if (MessageBox.Show($"A new version of {Program.ProgramName} is available. (Version v{latestVersion})\n" +
                                       $"Would you like to download it now?", Program.ProgramName,
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        LogFile.AddMessage("Starting Update...");
                        new UpdateForm(updateUrl).ShowDialog();
                    }
                    else
                        LogFile.AddMessage("Update Canceled. :(");
                }
                else
                    LogFile.AddMessage("No updates are available.");
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while checking for Mod Loader updates.", ex);
            }

        }

        public void AddMessage(string message)
        {
            LogFile.AddMessage(message);
            // Adds a character limit if not compiled in debug. 
            #if !DEBUG
            if (message.Length < 128)
            #endif
            Invoke(new Action(() => StatusLabel.Text = message));
        }

        public void AddMessage(string message, Exception exception, string[] extraData = null)
        {
            AddMessage(message);
            LogFile.AddMessage("    Exception: "+exception);
            if (extraData != null)
            {
                LogFile.AddMessage("    Extra Data: ");
                foreach (var s in extraData)
                {
                    LogFile.AddMessage("        "+s);
                }
            }
            MessageBox.Show(Resources.ResourceManager.GetString("ExceptionText"), Program.ProgramName);
        }

        public bool isCPKREDIRInstalled()
        {
            bool gens = false;
            if (File.Exists(GensExecutablePath))
                gens = true;
            try
            {
                byte[] bytes = File.ReadAllBytes(gens ? GensExecutablePath : LWExecutablePath);
                for (int i = 11918000; i < bytes.Length; ++i)
                {
                    // 63 70 6B 72 65 64 69 72
                    // c  p  k  r  e  d  i  r 

                    if (bytes[  i  ] == 0x63 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x6B &&
                        bytes[i + 3] == 0x72 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x64 &&
                        bytes[i + 6] == 0x69 && bytes[i + 7] == 0x72)
                        return true;

                    // 69 6D 61 67 65 68 6C 70
                    // i  m  a  g  e  h  l  p
                    if (bytes[i] == 0x69 && bytes[i + 1] == 0x6D && bytes[i + 2] == 0x61 &&
                        bytes[i + 3] == 0x67 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x68 &&
                        bytes[i + 6] == 0x6C && bytes[i + 7] == 0x70)
                        return false;
                }
            }
            catch(Exception ex)
            {
                AddMessage("Exception thrown while checking executeable.", ex, new String[] { $"Is Generations: {gens}" });
            }
            AddMessage("Failed to check executeable");
            return false;
        }

        public bool InstallCPKREDIR(bool? install)
        {
            string executablePath = LWExecutablePath;
            if (File.Exists(GensExecutablePath))
                executablePath = GensExecutablePath;
            try
            {
                AddMessage("Scaning Executable...");
                byte[] bytes = File.ReadAllBytes(executablePath);
                for (int i = 11918000; i < bytes.Length; ++i)
                {
                    // 63 70 6B 72 65 64 69 72
                    // c  p  k  r  e  d  i  r 

                    if (bytes[  i  ] == 0x63 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x6B &&
                        bytes[i + 3] == 0x72 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x64 &&
                        bytes[i + 6] == 0x69 && bytes[i + 7] == 0x72 && (install == null || install == false))
                    {
                        // Writing "imagehlp" to the executeable
                        bytes[  i  ] = 0x69;
                        bytes[i + 1] = 0x6D;
                        bytes[i + 2] = 0x61;
                        bytes[i + 3] = 0x67;
                        bytes[i + 4] = 0x65;
                        bytes[i + 5] = 0x68;
                        bytes[i + 6] = 0x6C;
                        bytes[i + 7] = 0x70;

                        // Deleting the old executable
                        File.Delete(executablePath);

                        // Now we're writing the newly modified exe.
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Uninstalled.");
                        return false;
                    }

                    // 69 6D 61 67 65 68 6C 70
                    // i  m  a  g  e  h  l  p

                    if (bytes[  i  ] == 0x69 && bytes[i + 1] == 0x6D && bytes[i + 2] == 0x61 &&
                        bytes[i + 3] == 0x67 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x68 &&
                        bytes[i + 6] == 0x6C && bytes[i + 7] == 0x70 && (install == null || install == true))
                    {
                        // Writing "cpkredir" to the executeable
                        bytes[  i  ] = 0x63;
                        bytes[i + 1] = 0x70;
                        bytes[i + 2] = 0x6B;
                        bytes[i + 3] = 0x72;
                        bytes[i + 4] = 0x65;
                        bytes[i + 5] = 0x64;
                        bytes[i + 6] = 0x69;
                        bytes[i + 7] = 0x72;

                        // Backing up the original executeable
                        if (!File.Exists(executablePath.Substring(0, executablePath.Length-4) + "_Backup.exe"))
                        {
                            File.Move(executablePath, executablePath.Substring(0, executablePath.Length - 4) + "_Backup.exe");
                        }else
                        {
                            File.Delete(executablePath);
                        }

                        // Now we're writing the newly modified exe.
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Installed.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while installing/uninstalling CPKREDIR.", ex, new String[]{
                    $"executablePath: {executablePath}", $"install: {install}" });
            }
            return false;
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void ReportLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Radfordhound/SLW-Mod-Loader/issues/new");
        }


        private void ScanExecuteableButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            PatchLabel.Text = (File.Exists(LWExecutablePath) ? Path.GetFileName(LWExecutablePath) : Path.GetFileName(GensExecutablePath)) +
                ": " + (isCPKREDIRInstalled() ? "Installed" : "Not Installed");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var modItem = ModsDb.GetMod(ModsList.FocusedItem.Text);
            if (modItem == null) return;
            
            if(modItem.UpdateServer.Length == 0 && modItem.Url.Length != 0)
            { // No Update Server, But has Website
                if (MessageBox.Show($"{Program.ProgramName} can not check for updates for {modItem.Title} because no update server has been set.\n\n" +
                    $"This Mod does have a website, do you want to open it to check for updates manually?\n\n URL: {modItem.Url}", Program.ProgramName,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start(modItem.Url);
                }
            }else if (modItem.UpdateServer.Length == 0 && modItem.Url.Length == 0)
            { // No Update Server and Website
                MessageBox.Show($"{Program.ProgramName} can not check for updates for {modItem.Title} because no update server has been set.",
                    Program.ProgramName, MessageBoxButtons.OK);
            }else if (modItem.UpdateServer.Length != 0)
            { // Has Update Server
                try
                {
                    WebClient wc = new WebClient();

                    if(modItem.UpdateServer.EndsWith(".txt"))
                    { // raw txt file.
                        MessageBox.Show("Not Implemented Yet", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }else if(modItem.UpdateServer.EndsWith("/") || modItem.UpdateServer.EndsWith("mod_version.ini"))
                    { // mod_version.ini file.
                        var mod_version_url = modItem.UpdateServer.EndsWith("/") ? modItem.UpdateServer + "mod_version.ini" : modItem.UpdateServer;
                        wc.DownloadFile(mod_version_url, Path.Combine(modItem.RootDirectory, "mod_version.ini.temp"));
                        IniFile mod_version = new IniFile(Path.Combine(modItem.RootDirectory, "mod_version.ini.temp"));
                        if (mod_version["Main"]["VersionString"] != modItem.Version)
                        { // New Version is Available.
                            if (MessageBox.Show($"There's a newer version of {modItem.Title} available!\n\n"+
                                    $"Do you want to update from version {modItem.Version} to " +
                                    $"{mod_version["Main"]["VersionString"]}? (about {mod_version["Main"]["DownloadSizeString"]})",
                                    Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // URL to mod_update_files.txt
                                var mod_update_files_url = modItem.UpdateServer.EndsWith("/") ? modItem.UpdateServer + "mod_update_files.txt" :
                                    modItem.UpdateServer.Substring(0, modItem.UpdateServer.Length - 15) + "mod_update_files.txt";
                                Dictionary<string, string> files = new Dictionary<string, string>();
                                // Downloads mod_update_files.txt
                                var mod_update_files = wc.DownloadString(mod_update_files_url);
                                // Splits all the lines in mod_update_files.txt into an array.
                                string[] split = mod_update_files.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                // Adds the file name and url to the files array.
                                foreach (var line in split)
                                {
                                    // Checks if the line starts with ';' if does then continue to the next line.
                                    if (line.StartsWith(";")) continue;
                                    files.Add(line.Split(':')[0], line.Substring(line.IndexOf(":") + 1));
                                }

                                UpdateModForm muf = new UpdateModForm(modItem.Title, files, modItem.RootDirectory);
                                muf.ShowDialog();
                                RefreshModsList();
                            }
                        }
                        else
                        { // Mod is up to date.or is newer then the one on the update server.
                            MessageBox.Show($"{modItem.Title} is already up to date.", Program.ProgramName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unknown file type.", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }catch(Exception ex)
                {
                    AddMessage("Exception thrown while updating.", ex, new String[]{ $"Update Server: {modItem.UpdateServer}" });
                }
            }
        }

        private void deleteModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete \"" + ModsList.FocusedItem.Text + "\"?", Program.ProgramName, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Directory.Delete(ModsDb.GetMod(ModsList.FocusedItem.Text).RootDirectory, true);
                RefreshModsList();

            }
            ModsList.FocusedItem = null;
            ModsList_SelectedIndexChanged(null, null);
        }

        private void ModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MoveUpAll.Enabled = MoveDownAll.Enabled = MoveUpButton.Enabled = MoveDownButton.Enabled = RemoveModButton.Enabled =
                editModToolStripMenuItem.Enabled = deleteModToolStripMenuItem.Enabled = checkForUpdatesToolStripMenuItem.Enabled = desciptionToolStripMenuItem.Enabled = 
                openModFolderToolStripMenuItem.Enabled = ModsList.SelectedItems.Count == 1;
        }

        private void AddModButton_Click(object sender, EventArgs e)
        {
            new AddModForm().ShowDialog();
            RefreshModsList();
        }

        private void InstallUninstallButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            PatchLabel.Text = (File.Exists(LWExecutablePath) ? Path.GetFileName(LWExecutablePath) : Path.GetFileName(GensExecutablePath)) +
                ": " + (InstallCPKREDIR(null) ? "Installed" : "Not Installed");
        }

        private void openModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", ModsDb.GetMod(ModsList.FocusedItem.Text).RootDirectory);
        }

        private void desciptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mod mod = ModsDb.GetMod(ModsList.FocusedItem.Text);
            new DescriptionForm(mod).ShowDialog();
        }

        private void RemoveModButton_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete \""+ ModsList.FocusedItem.Text+"\"?", Program.ProgramName, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Directory.Delete(ModsDb.GetMod(ModsList.FocusedItem.Text).RootDirectory, true);
                RefreshModsList();

            }
            ModsList.FocusedItem = null;
            RemoveModButton.Enabled = false;
        }

        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            // Checks if a item is selected and is not the first item.
            if (ModsList.FocusedItem == null || ModsList.FocusedItem.Index == 0) return;
            // Gets the mod item so we can remove it from the list and add it back in.
            var lvi = ModsList.FocusedItem;
            // Gets the position of the selected mod so we can move it down.
            int pos = ModsList.Items.IndexOf(lvi);
            // Removes the mod and reinserts it back in after where it used to be.
            ModsList.Items.Remove(lvi);
            ModsList.Items.Insert(pos - 1, lvi);
            // Selects the moved item.
            ModsList.FocusedItem = lvi;
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            // Checks if there is an item selected and that its not at the end of the list.
            if (ModsList.FocusedItem == null && ModsList.Items.Count > ModsList.FocusedItem.Index) return;
            // Checks if the item that is selected is not the last checked item.
            if (ModsList.FocusedItem.Index >= ModsList.CheckedItems.Count-1) return;
            // Gets the mod item so we can remove it from the list and add it back in.
            var lvi = ModsList.FocusedItem;
            // Gets the position of the selected mod so we can move it down.
            int pos = ModsList.Items.IndexOf(lvi);
            // Removes the mod and reinserts it back in after where it used to be.
            ModsList.Items.Remove(lvi);
            ModsList.Items.Insert(pos + 1, lvi);
            // Selects the moved item.
            ModsList.FocusedItem = lvi;
        }

        private void MoveUpAll_Click(object sender, EventArgs e)
        {
            // Checks if a item is selected and is not the first item.
            if (ModsList.FocusedItem == null || ModsList.FocusedItem.Index == 0) return;
            // Gets the mod item so we can remove it from the list and add it back in.
            var lvi = ModsList.FocusedItem;
            // Removes the mod from the mod list.
            ModsList.Items.Remove(lvi);
            // Adds the mod to the start of the mod list.
            ModsList.Items.Insert(0, lvi);

            // Selects the moved item.
            ModsList.FocusedItem = lvi;
        }

        private void MoveDownAll_Click(object sender, EventArgs e)
        {
            // Checks if there is an item selected and that its not at the end of the list.
            if (ModsList.FocusedItem == null && ModsList.Items.Count > ModsList.FocusedItem.Index) return;
            // Checks if the item that is selected is not the last checked item.
            if (ModsList.FocusedItem.Index >= ModsList.CheckedItems.Count - 1) return;
            // Gets the mod item so we can remove it from the list and add it back in.
            var lvi = ModsList.FocusedItem;
            // Removes the mod from the mod list.
            ModsList.Items.Remove(lvi);
            // Adds the mod to the start of the mod list.
            ModsList.Items.Insert(ModsList.CheckedItems.Count, lvi);

            // Selects the moved item.
            ModsList.FocusedItem = lvi;
        }

        private void ModsList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                foreach (string file in files)
                {
                    // Checks if its a Directory
                    if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                    {
                        AddModForm.InstallFromFolder(file);
                    }
                    else if (new FileInfo(file).Extension == ".zip")
                    { // Checks if it is a zip file.
                        AddModForm.InstallFromZip(file);
                    }
                    else if (new FileInfo(file).Extension == ".7z" || new FileInfo(file).Extension == ".rar")
                    { // Checks if it is a 7z or rar file.
                        AddModForm.InstallFrom7zArchive(file);
                    }
                }
                RefreshModsList();
            }
        }

        private void ModsList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void PatchButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(GensExecutablePath))
            {
                try
                {
                    // Creates a bckup if one hasn't been made.
                    if (!File.Exists(GensExecutablePath.Substring(0, GensExecutablePath.Length - 4) + "_Backup2.exe"))
                        File.Copy(GensExecutablePath, GensExecutablePath.Substring(0, GensExecutablePath.Length - 4) + "_Backup2.exe");
                    
                    var button = (Button)sender;
                    LogFile.AddMessage("Installing " + button.Text);

                    // A byte array holding the patch data. 
                    var patchBytes = GenerationsPatches[button.Text];
                    // Reads all the bytes from the Generations executeable.
                    var executeableBytes = File.ReadAllBytes(GensExecutablePath);
                    // Opens a FileStream to the Generations executeable.
                    var executeableStream = File.OpenWrite(GensExecutablePath);
                    // Current position of the patch file.
                    var position = 0;
                    // Amount of changes.
                    var changes = BitConverter.ToInt32(patchBytes, position);
                    // Adds 4 to position since we just read 4 bytes.
                    position += 4;
                    // Writes all bytes from executeableBytes back to the Generations executeable.
                    executeableStream.Write(executeableBytes, 0, executeableBytes.Length);
                    for (int i = 0; i < changes; ++i)
                    {
                        executeableStream.Position = BitConverter.ToInt32(patchBytes, position);
                        // Adds 4 to position since we just read 4 bytes.
                        position += 4;
                        // Gets the size of the current edit.
                        var size = BitConverter.ToInt32(patchBytes, position);
                        // Adds 4 to position since we just read 4 bytes.
                        position += 4;
                        // Writes the data to the executeable.
                        for (int i2 = 0; i2 < size; ++i2)
                        {
                            executeableStream.WriteByte(patchBytes[position + i2]);
                        }
                        position += size;
                    }
                    // Closes the FileStream.
                    executeableStream.Close();
                    MessageBox.Show("Done.", Program.ProgramName);
                    LogFile.AddMessage("Finished Installing " + button.Text);
                }
                catch (Exception ex)
                {
                    AddMessage("Exception thrown while applying a patch.", ex, new String[] { "Button Name: " + ((Button)sender).Text });
                }
            }
        }

        private void editModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewModForm nmf = new NewModForm(ModsDb.GetMod(ModsList.FocusedItem.Text));
            nmf.ShowDialog();
            RefreshModsList();
        }
    }
}