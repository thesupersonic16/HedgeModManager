using System;
using System.IO;
using System.Windows.Forms;
using SLWModLoader.Properties;
using System.Diagnostics;

namespace SLWModLoader
{
    public partial class MainForm : Form
    {
        public static readonly string LWExecutablePath = Path.Combine(Program.StartDirectory, "slw.exe");
        public static readonly string GensExecutablePath = Path.Combine(Program.StartDirectory, "SonicGenerations.exe");
        public static readonly string ModsFolderPath = Path.Combine(Program.StartDirectory, "mods");
        public static readonly string ModsDbPath = Path.Combine(ModsFolderPath, "ModsDB.ini");
        public ModsDatabase ModsDb;

        public MainForm()
        {
            InitializeComponent();
            Text += $" (v{Program.VersionString})";

            if (File.Exists(LWExecutablePath) || File.Exists(GensExecutablePath))
            {
                //TODO
            }
            else {
                MessageBox.Show(Resources.CannotFindExecutableText, Resources.ApplicationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                LogFile.AddMessage("Could not find executable, closing form...");

                Close();
                return;
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
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogFile.AddEmptyLine();
            LogFile.AddMessage("The form has been closed.");

            LogFile.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadMods();
        }

        public void LoadMods()
        {
            if (File.Exists(ModsDbPath))
            {
                LogFile.AddMessage("Found ModsDB, loading mods...");
                ModsDb = new ModsDatabase(ModsDbPath, ModsFolderPath);
            }
            else
            {
                LogFile.AddMessage("Could not find ModsDB, creating one...");
                ModsDb = new ModsDatabase(ModsFolderPath);

                //Check if there's any existing mods there
                ModsDb.GetModsInFolder();
            }

            LogFile.AddMessage($"Loaded total {ModsDb.ModCount} mods from \"{ModsFolderPath}\".");

            for (int i = 0; i < ModsDb.ModCount; ++i)
            {
                Mod modItem = ModsDb.GetMod(i);

                ListViewItem modListViewItem = new ListViewItem(modItem.Title);
                modListViewItem.Tag = modItem;
                modListViewItem.SubItems.Add(modItem.Version);
                modListViewItem.SubItems.Add(modItem.Author);
                modListViewItem.SubItems.Add("No");

                if (ModsDb.IsModActive(modItem))
                {
                    modListViewItem.Checked = true;
                }

                ModsList.Items.Add(modListViewItem);
            }

            NoModsFoundLabel.Visible = linkLabel1.Visible = (ModsDb.ModCount <= 0);
            LogFile.AddMessage("Succesfully updated list view!");
        }

        public void RefreshModsList()
        {
            ModsList.Clear();
            LoadMods();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshModsList();
        }

        private void SaveAndPlayButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(LWExecutablePath))
            {
                LogFile.AddMessage("Starting Sonic Lost World...");
                ModsDb.SaveModsDb(ModsDbPath);
                Process.Start("steam://rungameid/329440");
                Close();
            }
            else if (File.Exists(GensExecutablePath))
            {
                LogFile.AddMessage("Starting Sonic Generations...");
                ModsDb.SaveModsDb(ModsDbPath);
                Process.Start("steam://rungameid/71340");
                Close();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            ModsDb.SaveModsDb(ModsDbPath);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(LWExecutablePath))
            {
                LogFile.AddMessage("Starting Sonic Lost World...");
                Process.Start("steam://rungameid/329440");
                Close();
            }
            else if (File.Exists(GensExecutablePath))
            {
                LogFile.AddMessage("Starting Sonic Generations...");
                Process.Start("steam://rungameid/71340");
                Close();
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void ReportLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Radfordhound/SLW-Mod-Loader/issues/new");
        }
    }
}