using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class ModsDB : IniFile
    {
        public List<ModInfo> Mods = new List<ModInfo>();
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
            if (!Groups.ContainsKey("Main"))
                Groups.Add("Main", new IniGroup());
            if (!Groups.ContainsKey("Mods"))
                Groups.Add("Mods", new IniGroup());
            DetectMods();
            GetEnabledMods();
        }

        public void DetectMods()
        {
            Mods.Clear();

            foreach (string folder in Directory.GetDirectories(RootDirectory))
            {
                var mod = new ModInfo(folder);
                Mods.Add(mod);
            }
        }

        public void GetEnabledMods()
        {
            int activeCount = (int)this["Main"]["ActiveModCount", typeof(int)];
            for (int i = 0; i < activeCount; i++)
            {
                Mods.FirstOrDefault(t => Path.GetFileName(t.RootDirectory) == this["Main"][$"ActiveMod{i}"]).Enabled = true;
            }
        }

        public void BuildList()
        {
            this["Mods"].Params.Clear();
            foreach (var mod in Mods)
            {
                //use mod.title
                // lol I know
                //that works
                // looks good?
                // Dont use Title
                //cpkredir uses path?
                // lol We can use numbers iirc
                // I want it like GMI
                //GMI stores title tho
                // uh... Well All I know is that we should never use Titles
                // Its more likly to crash
                // seems legit
                this["Mods"][Path.GetFileName(mod.RootDirectory)] = Path.Combine(mod.RootDirectory, "mod.ini"); 
            }
        }

        public void SaveDB()
        {
            BuildList();
            using (var stream = File.OpenWrite(Path.Combine(RootDirectory, "ModsDB.ini")))
            {
                Write(stream);
            }
        }
    }
}
