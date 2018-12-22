using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HedgeModManager
{
    public class ModsDB : IniFile
    {
        public List<ModInfo> Mods = new List<ModInfo>();
        public bool ReverseLoadOrder = true;
        public string RootDirectory { get; set; }
        public int ModCount => Mods.Count;

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
                Mods.FirstOrDefault(t => Path.GetFileName(t.RootDirectory) == this["Main"]?[$"ActiveMod{i}"]).Enabled = true;
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
            this["Main"].Params.Clear();
            this["Main"].Params.Add("ReverseLoadOrder", Convert.ToInt32(ReverseLoadOrder).ToString());
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
    }
}
