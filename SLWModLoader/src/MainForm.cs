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

		// Constructors
		public MainForm()
        {
            InitializeComponent();
        }

		// Methods
		/// <summary>
		/// Copy all the ModLoader files into a custom directory
		/// and creates a shortcut after that it restarts the ModLoader in the custom directory.
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
                    if(File.Exists(filePath))
                    {
						// Copies the current file into the custom filepath.
                        File.Copy(filePath, Path.Combine(path, file), true);
						// Trys To delete the old file, Its almost always SLWModLoader.exe that fails.
						try { File.Delete(filePath); } catch { }
                    }
                    else
                    { // Missing file has been detected.
                        MessageBox.Show("Could not find " + file, Program.ProgramName);
                    }
                }

                LogFile.AddMessage("Creating Shortcut for " + gameName);
                
                // Creates a shortcut to the modloader.
                string shortcutPath = Path.Combine(Program.StartDirectory, $"SLWModLoader - {gameName}.lnk");
                var wsh = new IWshRuntimeLibrary.WshShell();
                var shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.Description = "SLWModLoader - "+gameName;
                shortcut.TargetPath = Path.Combine(path, Program.ExecutableName);
                shortcut.WorkingDirectory = path;
                shortcut.Save();
                LogFile.AddMessage("    Done.");

                MessageBox.Show("Done.", Program.ProgramName);

				// Starts the modloader.
                Process.Start(shortcutPath);

                try
                {
					// Trys to delete SLWModLoader.exe again.
                    var info = new ProcessStartInfo()
                    {
                        Arguments = $" /c sleep 3 & del \"{Application.ExecutablePath}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        FileName = "cmd.exe"
                    };
                    Process.Start(info);
                }
                catch { }
                return true;
            }
            return false;
        }

		/// <summary>
		/// Loads all the mods into ModsDb and fills in the Mods List.
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

            for (int i = 0; i < ModsDb.ModCount; ++i)
            {
                var modItem = ModsDb.GetMod(i);

                try
                {
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

			// Shows the no mods controls if not mods has been detected.
            NoModsFoundLabel.Visible = linkLabel1.Visible = (ModsDb.ModCount <= 0);
            LogFile.AddMessage("Succesfully updated list view!");
        }

		/// <summary>
		/// Moves the sctive mods to the top of the list.
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

            // Updates the background colour on the ListView if DarkTheme is enabled.
            int i = 0;
            if (CPKREDIRIni[Program.ProgramNameShort].ContainsParameter("DarkTheme") &&
                CPKREDIRIni[Program.ProgramNameShort]["DarkTheme"] == "1")
                foreach (ListViewItem lvi in ModsList.Items)
                    if (++i % 2 == 0) lvi.BackColor = Color.FromArgb(46, 46, 46);
                    else lvi.BackColor = Color.FromArgb(54, 54, 54);
        }

        private void SaveModDB()
        {
            // Deactivates All mods that are active.
            ModsDb.DeactivateAllMods();
            // Activates all mods that are currently checked.
            foreach (ListViewItem lvi in ModsList.CheckedItems)
                ModsDb.ActivateMod(ModsDb.GetMod(lvi.Text));

            // Saves and refreshes the mod list.
            ModsDb.SaveModsDb(ModsDbPath);
            RefreshModsList();
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

		// Events
		private void MainForm_Load(object sender, EventArgs e)
		{
			Text += $" (v{Program.VersionString})";
			if (File.Exists(LWExecutablePath) || File.Exists(GensExecutablePath))
			{

				if (File.Exists(LWExecutablePath))
				{
					Text += " - Sonic Lost World";
					LogFile.AddMessage("Found Sonic Lost World.");
					PatchGroupBox.Visible = false;
				}
				else if (File.Exists(GensExecutablePath))
				{
					Text += " - Sonic Generations";
					LogFile.AddMessage("Found Sonic Generations");

					// Adds SG Patches.
					GenerationsPatches.Add("Enable Blue Trail", Resources.Enable_Blue_Trail);
					GenerationsPatches.Add("Disable Blue Trail", Resources.Disable_Blue_Trail);
					GenerationsPatches.Add("", null);
					GenerationsPatches.Add("Enable FxPipeline", Resources.Enable_FxPipeline);
					GenerationsPatches.Add("Disable FxPipeline", Resources.Disable_FxPipeline);

					// Adds a Button for each patch.
					for (int i = 0; i < GenerationsPatches.Count; ++i)
					{
						// Ignores entries with no data.
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

				// Checks if the mods directory exits, If not, then ask to create one.
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

				// Loads all the mods into ModDb and fills the listView.
				LoadMods();
				// Reorders the mod list so active mods are on the top.
				OrderModList();

				string gameName = (File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations");
				if (!IsCPKREDIRInstalled())
				{
					if (MessageBox.Show(string.Format(Resources.ExecutableNotPatchedText, gameName),
						Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
						InstallCPKREDIR(true);
				}
				// Sets the PatchLabel's Text to show the user if CPKREDIR is installed in either game.
				PatchLabel.Text = gameName + ": " + (IsCPKREDIRInstalled() ? "Installed" : "Not Installed");
			}
			else
			{ // No supported game were found.
				if (MessageBox.Show(Resources.CannotFindExecutableText, Resources.ApplicationTitle,
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					// Gets Steam's Registry Key.
					var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
					// If null then try get it from the 64-bit Registry.
					if (key == null)
						key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Valve\\Steam");
					// Checks if the Key and Value exists.
					#region AutoInstaller its really messy.
					if (key != null && key.GetValue("SteamPath") is string steamPath)
					{
						// This is set to true if it installs successfully.
						bool installed = false;
						// Looks for supported games in the default library location. 
						if (Directory.Exists(Path.Combine(steamPath, "steamapps\\common")))
						{
							if (File.Exists(Path.Combine(steamPath, "steamapps\\common\\Sonic Lost World\\slw.exe")) && !installed)
								installed = InstallModLoader(Path.Combine(steamPath, "steamapps\\common\\Sonic Lost World"), "Sonic Lost World");

							if (File.Exists(Path.Combine(steamPath, "steamapps\\common\\Sonic Generations\\SonicGenerations.exe")) && !installed)
								installed = InstallModLoader(Path.Combine(steamPath, "steamapps\\common\\Sonic Generations"), "Sonic Generations");
						}
						// Looks at other libraries for a supported game. 
						var libraryfolders = File.ReadAllLines(Path.Combine(steamPath, "steamapps\\libraryfolders.vdf"));
						int i = 1;
						foreach (string libraryPath in libraryfolders)
						{
							if (libraryPath.IndexOf("\"" + i + "\"") != -1)
							{
								// Gets the location the library.
								string libraryLocation = libraryPath.Substring(libraryPath.IndexOf("\t\t\"") + 3,
									libraryPath.LastIndexOf('"') - (libraryPath.IndexOf("\t\t\"") + 3));
								// Looks for supported games in that library.
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
								++i;
							}

						}
						if (!installed)
						{
							MessageBox.Show("Failed to Install SLWModLoader,\n" +
								"It has either been declined or It could not find a supported game.",
								Resources.ApplicationTitle, MessageBoxButtons.OK,
								MessageBoxIcon.Error);
						}
					}
					else
					{
						MessageBox.Show("Could not find Steam's Registry Key, Closing...", Resources.ApplicationTitle,
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					#endregion
					Close();
				}
				else
				{ // Still Couldn't find a supported game.
					MessageBox.Show(Resources.CannotFindExecutableText, Resources.ApplicationTitle,
					MessageBoxButtons.OK, MessageBoxIcon.Error);
					LogFile.AddMessage("Could not find executable, closing form...");
					Close();
				}
				return;
			}

			// Checks if "cpkredit.ini" exists as SLWModLoader uses it to store its config.
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
						group.AddParameter("DarkTheme", "0");
					
					AutoCheckUpdateCheckBox.Checked = group["AutoCheckForUpdates"] != "0";
					KeepModLoaderOpenCheckBox.Checked = group["KeepModLoaderOpen"] != "0";
					
					EnableSaveFileRedirectionCheckBox.Checked = CPKREDIRIni["CPKREDIR"]["EnableSaveFileRedirection"] != "0";
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
			}else
			{
				MessageBox.Show("Could not find cpkredir.ini\nSG," +
					"LW and the ModLoader may not run correctly without this file.",
					Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			new Thread(new ThreadStart(CheckForModLoaderUpdates)).Start();
			if (AutoCheckUpdateCheckBox.Checked)
			{
				ModUpdatingThread = new Thread(new ThreadStart(CheckAllModUpdates));
				ModUpdatingThread.Start();
			}
			Ready = true;
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
                AddMessage("Exception thrown while saving ModsDB and starting.", ex, 
                $"Active Mod Count: {ModsDb.ActiveModCount}", $"File Path: {ModsDb.FilePath}", $"Root Directory: {ModsDb.RootDirectory}");
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
                $"Active Mod Count: {ModsDb.ActiveModCount}", $"File Path: {ModsDb.FilePath}", $"Root Directory: {ModsDb.RootDirectory}");
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

		/// <summary>
		/// Does what its saids, It starts the game.
		/// NOTE: Both games uses Steams protocol to start as its required for Lost World.
		/// </summary>
		public void StartGame()
        {
            if (File.Exists(LWExecutablePath))
            {
                AddMessage("Starting Sonic Lost World...");
                Process.Start("steam://rungameid/329440");
                if(!KeepModLoaderOpenCheckBox.Checked) Close();
            }
            else if (File.Exists(GensExecutablePath))
            {
                AddMessage("Starting Sonic Generations...");
                Process.Start("steam://rungameid/71340");
                if (!KeepModLoaderOpenCheckBox.Checked) Close();
            }
        }

		/// <summary>
		/// Searches for any new ModLoader updates.
		/// NOTE: The method is messy and can easily fail.
		/// </summary>
		public void CheckForModLoaderUpdates()
        {
            try
            {
                LogFile.AddMessage("Checking for Updates...");

                var webClient = new WebClient();
                string url = "https://api.github.com/repos/thesupersonic16/SLW-Mod-Loader/releases";
                string latestReleaseJson = "";
                string updateUrl = "";
                string releaseBody = "";
                float latestVersion = 0;
                float currentVersion = Convert.ToSingle(Program.VersionString.Substring(0, 3));

                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
                latestReleaseJson = webClient.DownloadString(url);
                latestVersion = Convert.ToSingle(latestReleaseJson.Substring(latestReleaseJson.IndexOf("tag_name") + 12, 3));
                updateUrl = Program.GetString(latestReleaseJson.IndexOf("\"browser_download_url\": ") + 23, latestReleaseJson);
                releaseBody = Program.GetString(latestReleaseJson.IndexOf("\"body\": \"") + 8, latestReleaseJson)
                    .Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\n");

                // If true, then a new update is available.
                if (latestVersion > currentVersion)
                {
                    AddMessage("New Update Found v" + latestVersion.ToString("0.0"));
                    if (new ChangeLogForm(latestVersion.ToString("0.0"), releaseBody, updateUrl).ShowDialog() == DialogResult.Yes)
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
        
        public void AddMessage(string message)
        {
            LogFile.AddMessage(message);
            // Adds a character limit if not compiled in debug. 
            #if !DEBUG
            if (message.Length < 128)
            #endif
            Invoke(new Action(() => StatusLabel.Text = message));
        }

        public static void AddMessage(string message, Exception exception, params string[] extraData)
        {
            if (Exit) return;
            LogFile.AddMessage(message);
            LogFile.AddMessage("    Exception: " + exception);
            if (extraData != null)
            {
                LogFile.AddMessage("    Extra Data: ");
                foreach (string s in extraData)
                    LogFile.AddMessage("        " + s);
            }
            MessageBox.Show(Resources.ExceptionText, Program.ProgramName);
            #if DEBUG
            throw exception;
            #endif
        }

        public bool IsCPKREDIRInstalled()
        {
            try
            {
                var bytes = File.ReadAllBytes(File.Exists(LWExecutablePath)
                    ? LWExecutablePath : GensExecutablePath);
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

                    if (bytes[  i  ] == 0x69 && bytes[i + 1] == 0x6D && bytes[i + 2] == 0x61 &&
                        bytes[i + 3] == 0x67 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x68 &&
                        bytes[i + 6] == 0x6C && bytes[i + 7] == 0x70)
                        return false;
                }
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while checking executeable.", ex);
            }
            AddMessage("Failed to check executeable");
            return false;
        }

		/// <summary>
		/// Installs or Uninstalls CPKREDIR.
		/// </summary>
		/// <param name="install">
		/// TRUE: Only check and Install CPKREDIR.
		/// FALSE: Only check and Uninstall CPKREDIR.
		/// NULL: Only check and Uninstall CPKREDIR if already installed, And can go the other way around
		/// </param>
		/// <returns>True if it Installs at the end.</returns>
		public bool InstallCPKREDIR(bool? install)
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

                    if (bytes[  i  ] == 0x63 && bytes[i + 1] == 0x70 && bytes[i + 2] == 0x6B &&
                        bytes[i + 3] == 0x72 && bytes[i + 4] == 0x65 && bytes[i + 5] == 0x64 &&
                        bytes[i + 6] == 0x69 && bytes[i + 7] == 0x72 && (install == null || install == false))
                    {
                        // Writes "imagehlp" to the executeable.
                        bytes[  i  ] = 0x69;
                        bytes[i + 1] = 0x6D;
                        bytes[i + 2] = 0x61;
                        bytes[i + 3] = 0x67;
                        bytes[i + 4] = 0x65;
                        bytes[i + 5] = 0x68;
                        bytes[i + 6] = 0x6C;
                        bytes[i + 7] = 0x70;

                        // Deletes the old executable.
                        File.Delete(executablePath);

                        // Writes the newly modified executable.
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
                        // Writes "cpkredir" to the executeable.
                        bytes[  i  ] = 0x63;
                        bytes[i + 1] = 0x70;
                        bytes[i + 2] = 0x6B;
                        bytes[i + 3] = 0x72;
                        bytes[i + 4] = 0x65;
                        bytes[i + 5] = 0x64;
                        bytes[i + 6] = 0x69;
                        bytes[i + 7] = 0x72;

                        // Backs up the original executeable.
                        if (!File.Exists(executablePath.Substring(0, executablePath.Length-4) + "_Backup.exe"))
                            File.Move(executablePath, executablePath.Substring(0, executablePath.Length - 4) + "_Backup.exe");
                        else
                            File.Delete(executablePath);

                        // Writes the newly modified executable.
                        File.WriteAllBytes(executablePath, bytes);
                        AddMessage("Done. CPKREDIR is now Installed.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage("Exception thrown while installing/uninstalling CPKREDIR.", ex, $"executablePath: {executablePath}",
                    $"install: {install}");
            }
            return false;
        }

		/// <summary>
		/// Checks for updates for all mods.
		/// </summary>
        public void CheckAllModUpdates()
        {
            // Amount of loaded mods.
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
                    // TODO: Find a way to get the Update SubItem without hardcoding a number.
                    Invoke(new Action(() => item.SubItems[4].Text = "Checking..."));
                    string status = CheckForModUpdates(mod.Title, true);
                    if (Exit)
                        return;
                    Invoke(new Action(() => item.SubItems[4].Text = status));
                }
            }
        }

		/// <summary>
		/// Checks for individual mod updates.
		/// </summary>
		/// <param name="modName">The Name of the mod (Frendly Name)</param>
		/// <param name="silent">True If you don't want dialog constently comming up</param>
		/// <returns>The Status</returns>
		public string CheckForModUpdates(string modName, bool silent = false)
        {
            string status = "";
            var mod = ModsDb.GetMod(modName ?? ModsList.FocusedItem.Text);
            if (mod == null) return status;

            if (mod.UpdateServer.Length == 0 && mod.Url.Length != 0)
            { // No Update Server, But has Website
                if (!silent && MessageBox.Show($"{Program.ProgramName} can not check for updates for {mod.Title}" +
                    $"because no update server has been set.\n\nThis Mod does have a website, " +
                    $"do you want to open it and check for updates manually?\n\n URL: {mod.Url}", Program.ProgramName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        if(Program.IsURL(mod.Url))
                            Process.Start(mod.Url);
            }
            else if (mod.UpdateServer.Length == 0 && mod.Url.Length == 0)
            { // No Update Server and Website
                if (!silent) MessageBox.Show($"{Program.ProgramName} can not check for updates for {mod.Title} because no update server has been set.",
                    Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (mod.UpdateServer.Length != 0)
            { // Has Update Server
                try
                {
                    var wc = new WebClient();

                    if (mod.UpdateServer.EndsWith(".txt"))
                    { // raw txt file.
                        if (!silent) MessageBox.Show("Not Implemented Yet", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        status = "Not Implemented";
                    }
                    else if (mod.UpdateServer.EndsWith("/") || mod.UpdateServer.EndsWith("mod_version.ini"))
                    { // mod_version.ini file.
                        string mod_version_url = mod.UpdateServer.EndsWith("/") ? mod.UpdateServer + "mod_version.ini" : mod.UpdateServer;
                        wc.DownloadFile(mod_version_url, Path.Combine(mod.RootDirectory, "mod_version.ini.temp"));
                        var mod_version = new IniFile(Path.Combine(mod.RootDirectory, "mod_version.ini.temp"));
                        if (mod_version["Main"]["VersionString"] != mod.Version)
                        { // New Version is Available.
                            if (MessageBox.Show($"There's a newer version of {mod.Title} available!\n\n" +
                                    $"Do you want to update from version {mod.Version} to " +
                                    $"{mod_version["Main"]["VersionString"]}? (about {mod_version["Main"]["DownloadSizeString"]})",
                                    Program.ProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // URL to mod_update_files.txt
                                string mod_update_files_url = mod.UpdateServer.EndsWith("/") ? mod.UpdateServer + "mod_update_files.txt" :
                                    mod.UpdateServer.Substring(0, mod.UpdateServer.Length - 15) + "mod_update_files.txt";
                                var files = new Dictionary<string, Tuple<string, string>>();
                                // Downloads mod_update_files.txt
                                string mod_update_files = wc.DownloadString(mod_update_files_url);
                                // Splits all the lines in mod_update_files.txt into an array.
                                var split = mod_update_files.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                // Adds the file name and url to the files array.
                                foreach (string line in split)
                                {
                                    // Checks if the line starts with ';' if does then continue to the next line.
                                    if (line.StartsWith(";")) continue;

                                    // Checks if the line starts with '#' if does then continue to the next line.
                                    if (line.StartsWith("#"))
                                        continue;

                                    files.Add(line.Split(':')[1],
                                        new Tuple<string, string>(line.Split(':')[0], line.Substring(line.IndexOf(":", line.IndexOf(":") + 1) + 1)));
                                }

                                var muf = new UpdateModForm(mod.Title, files, mod.RootDirectory);
                                muf.ShowDialog();
                                Invoke(new Action(() => RefreshModsList()));
                            }
                            else
                            {
                                status = "Available";
                            }
                        }
                        else
                        { // Mod is up to date or is newer then the one on the update server.
                            status = "Up to date";
                            if (!silent) MessageBox.Show($"{mod.Title} is already up to date.", Program.ProgramName);
                        }
                    }
                    else
                    {
                        if (!silent) MessageBox.Show("Unknown file type.", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        status = "Failed";
                    }

                }
                catch (WebException ex)
                {
                    if (!silent) MessageBox.Show("Failed to check for updates.\nMessage: " + ex.Message, Program.ProgramName);
                    LogFile.AddMessage("Exception thrown while updating.");
                    LogFile.AddMessage("    Exception: " + ex);
                    status = "Failed";
                }
                catch (Exception ex)
                {
                    AddMessage("Exception thrown while updating.", ex, $"Update Server: {mod.UpdateServer}");
                    status = "Failed";
                }
            }
            return status;
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void ReportLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/thesupersonic16/SLW-Mod-Loader/issues/new");
        }

        private void ScanExecuteableButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
			string gameName = (File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations");
			PatchLabel.Text = gameName + ": " + (IsCPKREDIRInstalled() ? "Installed" : "Not Installed");
		}

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

        private void ModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MoveUpAll.Enabled = MoveDownAll.Enabled = MoveUpButton.Enabled = MoveDownButton.Enabled
                = RemoveModButton.Enabled = createModUpdateToolStripMenuItem.Enabled = editModToolStripMenuItem.Enabled
                = deleteModToolStripMenuItem.Enabled = checkForUpdatesToolStripMenuItem.Enabled
                = desciptionToolStripMenuItem.Enabled = openModFolderToolStripMenuItem.Enabled
                = ModsList.SelectedItems.Count == 1;
        }

        private void AddModButton_Click(object sender, EventArgs e)
        {
            new AddModForm().ShowDialog();
            RefreshModsList();
        }

        private void InstallUninstallButton_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
			string gameName = (File.Exists(LWExecutablePath) ? "Sonic Lost World" : "Sonic Generations");
			PatchLabel.Text = gameName + ": " + (InstallCPKREDIR(null) ? "Installed" : "Not Installed");
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

        private void RemoveModButton_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show($"Are you sure you want to delete \"{ModsList.FocusedItem.Text}\"?",
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
            new Thread(() =>
            {
                try
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                        e.Data.GetData(DataFormats.FileDrop) is string[] files)
                    {
                        int prevModCount = ModsDb.ModCount;
                        Invoke(new Action(() => AddMessage("Installing mods...")));
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
                    // Creates a backup if one hasn't been made.
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
                    int position = 0;
                    // Amount of changes.
                    int changesCount = BitConverter.ToInt32(patchBytes, position);
                    // Adds 4 to position since we just read 4 bytes.
                    position += 4;
                    // Writes all bytes from executeableBytes back to the Generations executeable.
                    executeableStream.Write(executeableBytes, 0, executeableBytes.Length);
                    for (int i = 0; i < changesCount; ++i)
                    {
                        executeableStream.Position = BitConverter.ToInt32(patchBytes, position);
                        // Adds 4 to position since we just read 4 bytes.
                        position += 4;
                        // Gets the size of the current edit.
                        int size = BitConverter.ToInt32(patchBytes, position);
                        // Adds 4 to position since we just read 4 bytes.
                        position += 4;
                        // Writes the data to the executeable.
                        for (int i2 = 0; i2 < size; ++i2)
                            executeableStream.WriteByte(patchBytes[position + i2]);

                        position += size;
                    }
                    // Closes the FileStream.
                    executeableStream.Close();
                    MessageBox.Show("Done.", Program.ProgramName);
                    LogFile.AddMessage("Finished Installing " + button.Text);
                }
                catch (Exception ex)
                {
                    AddMessage("Exception thrown while applying a patch.", ex, "Button Name: " + ((Button)sender).Text);
                }
            }
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
            Mod unModified = null;
            string outLocation = Path.Combine(Program.StartDirectory, "temp_update");

            var saveFileDialog = new SaveFileDialog()
            {
                Title = Program.ProgramName,
                FileName = "Enter into a directory where you want all the update files to be saved, then press Save" +
                " (All files in the selected directory will be deleted)"
            };
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Open Unmodified mod.",
                Filter = "Mod Ini (mod.ini)|mod.ini;"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    outLocation = Path.GetDirectoryName(saveFileDialog.FileName);

                if(MessageBox.Show("Save all file into " + outLocation +
                    ", NOTE: All files in the selected directory will be deleted", Program.ProgramName,
                    MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                // Deletes everything inside of the output location.
                if (Directory.Exists(outLocation))
                    Directory.Delete(outLocation, true);
                Directory.CreateDirectory(outLocation);
                try
                {
                    unModified = new Mod(Path.GetDirectoryName(openFileDialog.FileName));
                    
                    // Creates all of the directories.
                    foreach (string dirPath in Directory.GetDirectories(modified.RootDirectory, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(modified.RootDirectory, outLocation));

                    // List of paths to files that are modified.
                    var files = new List<string>();

                    var sha = new SHA256Managed();
                    
                    foreach (string filePath in Directory.GetFiles(modified.RootDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        // Checks if the file exists in the unmodified folder.
                        if (!File.Exists(filePath.Replace(modified.RootDirectory, unModified.RootDirectory)))
                        {
                            files.Add(filePath);
                            continue;
                        }
                        var modifiedBytes = File.ReadAllBytes(filePath);
                        var unModifiedBytes = File.ReadAllBytes(filePath.Replace(modified.RootDirectory,
                            unModified.RootDirectory));
                        if (modifiedBytes.Length == unModifiedBytes.Length)
                        {
                            for (int i = 0; i < modifiedBytes.Length; ++i)
                                if (modifiedBytes[i] != unModifiedBytes[i])
                                {
                                    files.Add(filePath);
                                    continue;
                                }
                        }
                        else
                            files.Add(filePath);
                    }

                    var lines = new List<string>
                    {
                        "#Version:0",
                        ";SHA256 HASH:File Name (Including Subdirectories starting from the mod root):URL",
                        ";" + 0.ToString("X64") + ":mod.ini:http://localhost/ModName/mod.ini",
                        ";" + 0.ToString("X64") + ":bb3\\Sonic.ar.00:http://localhost/ModName/bb3/Sonic.ar.00",
                        ";" + 0.ToString("X64") + ":sonic2013_patch_0\\Sonic.pac:http://localhost/ModName/sonic2013_patch_0/Sonic.pac",
                        ""
                    };
                    foreach (string filePath in files)
                    {
                        string hash = "";
                        using (FileStream stream = File.OpenRead(filePath))
                           hash = UpdateModForm.ByteArrayToString(sha.ComputeHash(stream));

                        lines.Add($"{hash}:{filePath.Replace(modified.RootDirectory+"\\", "")}:{{URL}}{Path.GetFileName(filePath)}");
                    }


                    // Copies all the modified files into the output directory.
                    foreach (string filePath in files)
                        File.Copy(filePath, filePath.Replace(modified.RootDirectory, outLocation), true);

                    // Writes everything from the lines array to mod_update_files.txt.
                    File.WriteAllLines(Path.Combine(outLocation, "mod_update_files.txt"), lines.ToArray());

                    MessageBox.Show("Done.\n" +
                        "Make sure to replace all {URL} in mod_update_files.txt before uploading all the files " +
                        "on to your web server.",
                        Program.ProgramName);

                    // Opens the output folder in explorer.
                    Process.Start("explorer", outLocation);
                }catch(Exception ex)
                {
                    AddMessage("Exception thrown while creating update files", ex);
                }
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CPKREDIRIni != null && Ready)
            {
                CPKREDIRIni[Program.ProgramNameShort]["AutoCheckForUpdates"] = AutoCheckUpdateCheckBox.Checked ? "1" : "0";
                CPKREDIRIni[Program.ProgramNameShort]["KeepModLoaderOpen"] = KeepModLoaderOpenCheckBox.Checked ? "1" : "0";
				CPKREDIRIni["CPKREDIR"]["EnableSaveFileRedirection"] = EnableSaveFileRedirectionCheckBox.Checked ? "1" : "0";

				if (EnableCPKREDIRConsoleCheckBox.Checked && !CPKREDIRIni["CPKREDIR"].ContainsParameter("LogType"))
					CPKREDIRIni["CPKREDIR"].AddParameter("LogType", "console");
				else
					CPKREDIRIni["CPKREDIR"].RemoveParameter("LogType");
			}
        }

        #region Don't Look!

        private void ModsList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Colours
            var dark1 = Color.FromArgb(34, 34, 34);
            var dark2 = Color.FromArgb(70, 70, 70);
            
            // Draws the Header.
            if(e.Bounds.Contains(ModsList.PointToClient(MousePosition)))
                 e.Graphics.FillRectangle(new SolidBrush(dark1), e.Bounds);
            else e.Graphics.FillRectangle(new SolidBrush(dark2), e.Bounds);
            var point = new Point(0, 6);
            point.X = e.Bounds.X;
            var col = ModsList.Columns[e.ColumnIndex];
            e.Graphics.FillRectangle(new SolidBrush(dark1), point.X, 0, 2, e.Bounds.Height);
            point.X += col.Width / 2 - TextRenderer.MeasureText(col.Text, ModsList.Font).Width/2;
            TextRenderer.DrawText(e.Graphics, col.Text, ModsList.Font, point, ModsList.ForeColor);
        }

        private void ModsList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        public static void AddAllChildControls(Control control, List<Control> controls)
        {
            controls.Add(control);
            foreach (Control control2 in control.Controls)
                AddAllChildControls(control2, controls);
        }

        public static bool ApplyDarkTheme(Control control, params Control[] controls)
        {
            if (CPKREDIRIni[Program.ProgramNameShort].ContainsParameter("DarkTheme") && CPKREDIRIni[Program.ProgramNameShort]["DarkTheme"] == "1")
            {
                LogFile.WriteMessage($"Applying Dark Theme to {control.GetType().Name}...", true);

                var allControls = new List<Control>();

                AddAllChildControls(control, allControls);

                foreach (Control control0 in controls)
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