using HedgeModManager.Properties;
using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class InstallForm : Form
    {
        public InstallForm()
        {
            InitializeComponent();
        }

        public static List<Entry> FindGames()
        {
            var entries = new List<Entry>();
            var paths = new List<string>();

            string vdfLocation = Path.Combine(Steam.SteamLocation, "steamapps\\libraryfolders.vdf");
            LogFile.AddMessage($"Reading VDF: {vdfLocation}");
            Steam.VDFFile vdf = null;
            try
            {
                vdf = Steam.VDFFile.ReadVDF(vdfLocation);
            }catch(Exception ex)
            {
                MainForm.AddMessage("Exception thrown while reading Steam's VDF file.", ex,
                $"VDFPath: {vdfLocation}");
            }

            // Default Common Path
            paths.Add(Path.Combine(Steam.SteamLocation, "steamapps\\common"));

            // Gets all the custom libraries
            foreach (var library in vdf.Array.Elements)
                if (int.TryParse(library.Key, out int index))
                    paths.Add(Path.Combine(library.Value.Value, "steamapps\\common"));
            
            foreach (string path in paths)
                if (Directory.Exists(path))
                {
                    string lwPath = Path.Combine(path, "Sonic Lost World\\slw.exe");
                    string gensPath = Path.Combine(path, "Sonic Generations\\SonicGenerations.exe");
                    string forcesPath = Path.Combine(path, "SonicForces\\build\\main\\projects\\exec\\Sonic Forces.exe");
                    if (CheckGameAndSupport(lwPath))
                        entries.Add(new Entry() { GameName = "Sonic Lost World", Path = lwPath });
                    if (CheckGameAndSupport(gensPath))
                        entries.Add(new Entry() { GameName = "Sonic Generations", Path = gensPath });
                    if (CheckGameAndSupport(forcesPath))
                        entries.Add(new Entry() { GameName = "Sonic Forces", Path = forcesPath });
                }
            return entries;
        }

        public static bool CheckGameAndSupport(string path)
        {
            return File.Exists(path) && !(
                File.Exists(Path.Combine(Path.GetDirectoryName(path), "steamclient64.dll")) ||
                File.Exists(Path.Combine(Path.GetDirectoryName(path), "steamclient.dll")));
        }

        private void InstallForm_Load(object sender, EventArgs e)
        {
            try
            {
                Steam.Init();
            }
            catch (Exception ex)
            {
                MainForm.AddMessage("Exception thrown while Finding Steam", ex);
            }

            if (Steam.SteamLocation == null)
            {
                MainForm.AddMessageToUser($"Steam is not Setup correctly, " +
                    $"Please report this issue onto the {Program.ProgramName} GitHub Repo with {Program.ProgramName}.log attached.\n" +
                    $"Please copy all files that came with {Program.ProgramName} into your Game folder. Exiting...");
                Close();
                return;
            }

            var games = FindGames();
            foreach (var game in games)
            {
                var lvi = new ListViewItem();
                lvi.Tag = game;
                lvi.SubItems[0].Text = game.GameName;
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, game.Path));
                listView1.Items.Add(lvi);
            }
            Theme.ApplyDarkThemeToAll(this);
        }

        public class Entry
        {
            public string GameName, Path;

            public override string ToString()
            {
                return GameName;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var game = listView1.SelectedItems[0].Tag as Entry;
                if (game != null)
                {
                    if (InstallModLoader(Path.GetDirectoryName(game.Path), game.GameName))
                        Close();
                }
            }
        }

        /// <summary>
        /// Copy all the ModLoader files into a custom directory
        /// and creates a shortcut, After that it restarts the ModLoader in that custom directory
        /// </summary>
        /// <param name="path">Path to an Executable</param>
        /// <param name="gameName">The Name of the game</param>
        /// <returns></returns>
        public bool InstallModLoader(string path, string gameName)
        {
            path = path.Replace('/', '\\');
            if (MessageBox.Show("Install HedgeModManager into \n" + path + "?", Resources.ApplicationTitle,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // HedgeModManager.exe, HedgeModManager.pdb, cpkredir.dll, cpkredir.txt
                string[] files = new string[]
                {
                    Program.ExecutableName,
                    Path.ChangeExtension(Program.ExecutableName, "pdb"),
                    "cpkredir.dll",
                    "cpkredir.txt"
                };

                foreach (string file in files)
                {
                    string filePath = Path.Combine(Program.StartDirectory, file);
                    if (File.Exists(filePath))
                    {
                        // Copies the current file into the custom filepath
                        File.Copy(filePath, Path.Combine(path, file), true);

                        // Don't delete this file as its in use
                        if (file == Program.ExecutableName)
                            continue;

                        // Tries to delete the old files
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
                    string shortcutPath = Path.Combine(Program.StartDirectory, $"HedgeModManager - {gameName}.lnk");
                    var wsh = new IWshRuntimeLibrary.WshShell();
                    var shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
                    shortcut.Description = $"HedgeModManager - {gameName}";
                    shortcut.TargetPath = Path.Combine(path, Program.ExecutableName);
                    shortcut.WorkingDirectory = path;
                    shortcut.Save();
                    LogFile.AddMessage("    Done.");
                }
                catch
                {
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
                    // Tries to delete the old HedgeModManager.exe
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
    }
}
