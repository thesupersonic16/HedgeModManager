using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HedgeModManager
{
    public class Mod
    {
        private IniFile main;

        public string RootDirectory { get; set; }
        public string FilePath => Path.Combine(RootDirectory, "mod.ini");

        public Mod()
        {
            RootDirectory = string.Empty;
            main = new IniFile();

            IniGroup mainGroup = new IniGroup("Main");
            mainGroup.AddParameter("IncludeDirCount", 0, typeof(int));
            mainGroup.AddParameter("UpdateServer");
            mainGroup.AddParameter("SaveFile");

            IniGroup descGroup = new IniGroup("Desc");
            descGroup.AddParameter("Title");
            descGroup.AddParameter("Description");
            descGroup.AddParameter("Version");
            descGroup.AddParameter("Date");
            descGroup.AddParameter("Author");
            descGroup.AddParameter("AuthorURL");
            descGroup.AddParameter("URL");

            main.AddGroup(mainGroup);
            main.AddGroup(descGroup);
        }

        public Mod(string title, string description, string version, string date, string author, string url) : this()
        {
            Title = title;
            Description = description;
            Version = version;
            Date = date;
            Author = author;
            Url = url;
        }

        public Mod(string directory)
        {
            RootDirectory = directory;
            main = new IniFile(FilePath);
        }

        public void IncludeDirectory(string directory)
        {
            if (directory.StartsWith(RootDirectory))
                directory = directory.Replace(RootDirectory, ".");

            string parameterKey = $"IncludeDir{main["Main"].ParameterCount - 1}";
            main["Main"].AddParameter(parameterKey, directory);

            // Update the directory count
            main["Main"]["IncludeDirCount", typeof(int)] = main["Main"].ParameterCount - 1;
        }

        public string GetIncludedDirectory(int index)
        {
            string parameterKey = $"IncludeDir{index}";
            return main["Main"][parameterKey];
        }

        public int IncludeDirCount
        {
            get { return int.Parse(main["Main"]["IncludeDirCount"]); }
            set { main["Main"]["IncludeDirCount"] = value.ToString(); }
        }

        public string UpdateServer
        {
            get { return main["Main"]["UpdateServer"]; }
            set { main["Main"]["UpdateServer"] = value; }
        }

        public string SaveFile
        {
            get { return main["Main"]["SaveFile"]; }
            set { main["Main"]["SaveFile"] = value; }
        }
        
        public string Title
        {
            get { return main["Desc"]["Title"]; }
            set { main["Desc"]["Title"] = value; }
        }

        public string Description
        {
            get { return main["Desc"]["Description"]; }
            set { main["Desc"]["Description"] = value; }
        }

        public string Version
        {
            get { return main["Desc"]["Version"]; }
            set { main["Desc"]["Version"] = value; }
        }

        public string Date
        {
            get { return main["Desc"]["Date"]; }
            set { main["Desc"]["Date"] = value; }
        }

        public string Author
        {
            get { return main["Desc"]["Author"]; }
            set { main["Desc"]["Author"] = value; }
        }

        public string AuthorUrl
        {
            get { return main["Desc"]["AuthorURL"]; }
            set { main["Desc"]["AuthorURL"] = value; }
        }

        public string Url
        {
            get { return main["Desc"]["URL"]; }
            set { main["Desc"]["URL"] = value; }
        }

        public IniFile GetIniFile()
        {
            return main;
        }

        public void Save(string path)
        {
            main.Save(path);
        }

        public void Save()
        {
            main.Save(FilePath);
        }
    }

    public class ModsDatabase
    {
        private IniFile modsDb;
        private List<Mod> mods;
        private List<string> codes = new List<string>();

        public string RootDirectory { get; set; }
        public string FilePath => modsDb.IniPath;

        public int ActiveModCount => (int)modsDb["Main"]["ActiveModCount", typeof(int)];
        public int ModCount => mods.Count;

        public ModsDatabase()
        {
            RootDirectory = string.Empty;
            mods = new List<Mod>();

            modsDb = new IniFile();

            IniGroup mainGroup = new IniGroup("Main");
            mainGroup.AddParameter("ReverseLoadOrder", 0, typeof(int));
            mainGroup.AddParameter("ActiveModCount", 0, typeof(int));

            IniGroup modsGroup = new IniGroup("Mods");

            modsDb.AddGroup(mainGroup);
            modsDb.AddGroup(modsGroup);
        }

        public ModsDatabase(string directory) : this()
        {
            RootDirectory = directory;
            mods = new List<Mod>();

            GetModsInFolder();
        }

        public ModsDatabase(string path, string directory)
        {
            RootDirectory = directory;
            mods = new List<Mod>();
            modsDb = new IniFile(path);

            GetModsInFolder();
        }

        public void GetModsInFolder()
        {
            mods.Clear();
            modsDb["Mods"].Clear();

            foreach (string folder in Directory.GetDirectories(RootDirectory))
            {
                string iniPath = Path.Combine(folder, "mod.ini");

                if (File.Exists(iniPath))
                {
                    try
                    {

                        Mod mod = new Mod(folder);
                        AddMod(mod);

                        //LogFile.AddMessage($"Found mod: {mod.Title}");
                    }
                    catch(Exception ex)
                    {
                        MainForm.AddMessage("Exception thrown while loading mods.", ex,
                        $"Active Mod Count: {ActiveModCount}", $"File Path: {FilePath}", $"Root Directory: {RootDirectory}");
                    }
                }
            }
        }

        public bool ReverseLoadOrder
        {
            get { return (bool)modsDb["Main"]["ReverseLoadOrder", typeof(bool)]; }
            set { modsDb["Main"]["ReverseLoadOrder", typeof(int)] = value == true ? 1 : 0; }
        }

        public Mod GetMod(int index)
        {
            return mods[index];
        }

        public Mod GetMod(string title)
        {
            return mods.FirstOrDefault(t => t.Title == title);
        }

        public void AddMod(Mod mod)
        {
            if (mods.Contains(mod))
                return;

            if (!mod.RootDirectory.StartsWith(RootDirectory))
            {
                Directory.Move(mod.RootDirectory, RootDirectory);
                mod.RootDirectory = Path.Combine(RootDirectory, Path.GetDirectoryName(mod.RootDirectory));
            }

            modsDb["Mods"].AddParameter(mod.Title, mod.FilePath);
            mods.Add(mod);
        }

        public void AddCode(string codeName)
        {
            if (codes.Contains(codeName))
                return;

            codes.Add(codeName);
            BuildCodesList();
        }

        public void BuildCodesList()
        {
            if (!modsDb.ContainsGroup("Codes"))
                modsDb.AddGroup(new IniGroup("Codes"));
            modsDb["Codes"].RemoveAllParameters();
            for (int i = 0; i < codes.Count; ++i)
                modsDb["Codes"].AddParameter($"Code{i}", codes[i]);
        }

        public void ReadCodesList()
        {
            codes.Clear();
            if (!modsDb.ContainsGroup("Codes"))
                modsDb.AddGroup(new IniGroup("Codes"));
            int i = 0;
            while (modsDb["Codes"].ContainsParameter($"Code{i}"))
                codes.Add(modsDb["Codes"][$"Code{i++}"]);
        }

        public void RemoveCode(string codeName)
        {
            if (!codes.Contains(codeName))
                return;
            
            codes.Remove(codeName);
            BuildCodesList();
        }

        public void RemoveAllCodes()
        {
            codes.Clear();
            modsDb["Codes"].RemoveAllParameters();
        }

        public List<string> GetCodeList()
        {
            return codes;
        }

        public void RemoveMod(Mod mod)
        {
            if (!mods.Contains(mod))
                return;

            if (IsModActive(mod))
                DeactivateMod(mod);

            mods.Remove(mod);
            modsDb["Mods"].RemoveParameter(mod.Title);
        }

        public void ActivateMod(Mod mod)
        {
            if (!modsDb["Mods"].ContainsParameter(mod.Title))
                modsDb["Mods"].AddParameter(mod.Title, mod.FilePath);

            if (IsModActive(mod))
                return;

            string parameterKey = $"ActiveMod{modsDb["Main"]["ActiveModCount", typeof(int)]}";
            modsDb["Main"].AddParameter(parameterKey, mod.Title);

            modsDb["Main"]["ActiveModCount", typeof(int)] = modsDb["Main"].ParameterCount - 2;
        }

        public void DeactivateMod(Mod mod)
        {
            // We're now going to write the order again, so we're collecting the activated mods into list
            List<Mod> activeMods = new List<Mod>();
            
            for (int i = 0; i < ActiveModCount; i++)
            {
                string parameterKey = $"ActiveMod{i}";

                Mod activeMod = GetMod(modsDb["Main"][parameterKey]);

                if (activeMod != mod)
                    activeMods.Add(activeMod);

                modsDb["Main"].RemoveParameter(parameterKey);
            }

            for (int i = 0; i < activeMods.Count; i++)
            {
                string parameterKey = $"ActiveMod{i}";
                modsDb["Main"].AddParameter(parameterKey, activeMods[i].Title);
            }

            modsDb["Main"]["ActiveModCount", typeof(int)] = activeMods.Count;
        }

        public void DeactivateAllMods()
        {
            for (int i = 0; i < ActiveModCount; i++)
            {
                string parameterKey = $"ActiveMod{i}";
                modsDb["Main"].RemoveParameter(parameterKey);
            }
            modsDb["Main"]["ActiveModCount", typeof(int)] = modsDb["Main"].ParameterCount - 2;
        }

        public bool IsModActive(Mod mod)
        {
            for (int i = 0; i < ActiveModCount; i++)
            {
                string parameterKey = $"ActiveMod{i}";

                if (modsDb["Main"].ContainsParameter(parameterKey) && modsDb["Main"][parameterKey] == mod.Title)
                {
                    return true;
                }
            }

            return false;
        }

        public IniFile GetIniFile()
        {
            return modsDb;
        }

        public void SaveModsDb(string path)
        {
            modsDb.Save(path);
        }

        public void SaveModsDb()
        {
            modsDb.Save();
        }
    }
}
