using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLWModLoader
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
        private IniFile modsDB;

        public string RootDirectory { get; set; }
        public string FilePath => modsDB.IniPath;

        private List<Mod> mods;

        public ModsDatabase()
        {
            RootDirectory = String.Empty;
            mods = new List<Mod>();

            modsDB = new IniFile();

            IniGroup mainGroup = new IniGroup("Main");
            mainGroup.AddParameter("ReverseLoadOrder", 0, typeof(int));
            mainGroup.AddParameter("ActiveModCount", 0, typeof(int));

            IniGroup modsGroup = new IniGroup("Mods");

            modsDB.AddGroup(mainGroup);
            modsDB.AddGroup(modsGroup);
        }

        public ModsDatabase(string path, string directory)
        {
            RootDirectory = directory;
            mods = new List<Mod>();
            modsDB = new IniFile(path);

            modsDB["Mods"].Clear();
            foreach (string folder in Directory.GetDirectories(directory))
            {
                string iniPath = Path.Combine(folder, "mod.ini");

                if (File.Exists(iniPath))
                {
                    Mod mod = new Mod(folder);
                    AddMod(mod);
                }
            }
        }

        public bool ReverseLoadOrder
        {
            get { return (bool)modsDB["Main"]["ReverseLoadOrder", typeof(bool)]; }
            set { modsDB["Main"]["ReverseLoadOrder", typeof(int)] = value == true ? 1 : 0; }
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

            mods.Add(mod);
            modsDB["Mods"].AddParameter(mod.Title, mod.FilePath);
        }

        public void RemoveMod(Mod mod)
        {
            if (!mods.Contains(mod))
                return;

            if (IsModActive(mod))
                DeactivateMod(mod);

            mods.Remove(mod);
            modsDB["Mods"].RemoveParameter(mod.Title);
        }

        public int ActiveModCount => (int)modsDB["Main"]["ActiveModCount", typeof(int)];

        public void ActivateMod(Mod mod)
        {
            if (!modsDB["Mods"].ContainsParameter(mod.Title))
                modsDB["Mods"].AddParameter(mod.Title, mod.FilePath);

            if (IsModActive(mod))
                return;

            string parameterKey = $"ActiveMod{modsDB["Main"]["ActiveModCount", typeof(int)]}";
            modsDB["Main"].AddParameter(parameterKey, mod.Title);

            modsDB["Main"]["ActiveModCount", typeof(int)] = modsDB["Main"].ParameterCount - 2;
        }

        public void DeactivateMod(Mod mod)
        {
            // We're now going to write the order again, so we're collecting the activated mods into list
            List<Mod> activeMods = new List<Mod>();
            
            for (int i = 0; i < ActiveModCount; i++)
            {
                string parameterKey = $"ActiveMod{i}";

                Mod activeMod = GetMod(modsDB["Main"][parameterKey]);

                if (activeMod != mod)
                    activeMods.Add(activeMod);

                modsDB["Main"].RemoveParameter(parameterKey);
            }

            for (int i = 0; i < activeMods.Count; i++)
            {
                string parameterKey = $"ActiveMod{i}";
                modsDB["Main"].AddParameter(parameterKey, activeMods[i].Title);
            }

            modsDB["Main"]["ActiveModCount", typeof(int)] = activeMods.Count;
        }

        public bool IsModActive(Mod mod)
        {
            for (int i = 0; i < ActiveModCount; i++)
            {
                string parameterKey = $"ActiveMod{i}";

                if (modsDB["Main"].ContainsParameter(parameterKey) && modsDB["Main"][parameterKey] == mod.Title)
                {
                    return true;
                }
            }

            return false;
        }

        public void SaveModsDB(string path)
        {
            modsDB.Save(path);
        }

        public void SaveModsDB()
        {
            modsDB.Save();
        }
    }
}
