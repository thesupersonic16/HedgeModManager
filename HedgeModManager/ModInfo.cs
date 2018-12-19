using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class ModInfo : IniFile
    {
        public List<string> IncludeDirs = new List<string>();
        public string UpdateServer;
        public string SaveFile;
        public string RootDirectory;


        // lol You did write it
        public bool Enabled { get; set; }

        public bool HasUpdates { get; set; }
 
        public bool SupportsSave => !string.IsNullOrEmpty(SaveFile);
        
        // Desc
        public string Title { get { return Groups["Desc"]["Title"]; } set { Groups["Desc"]["Title"] = value; } }

        public string Description { get { return Groups["Desc"]["Description"]; } set { Groups["Desc"]["Description"] = value; } }

        public string Version { get { return Groups["Desc"]["Version"]; } set { Groups["Desc"]["Version"] = value; } }

        public string Date { get { return Groups["Desc"]["Date"]; } set { Groups["Desc"]["Date"] = value; } }

        public string Author { get { return Groups["Desc"]["Author"]; } set { Groups["Desc"]["Author"] = value; } }

        public ModInfo()
        {
 
        }

        public ModInfo(string modPath)
        {
            RootDirectory = modPath;
            using (var stream = File.OpenRead(Path.Combine(modPath, "mod.ini")))
                Read(stream);

        }

        public override void Read(Stream stream)
        {
            base.Read(stream);
            UpdateServer = Groups["Main"]["UpdateServer"];
            SaveFile = Groups["Main"]["SaveFile"];
            Description = Groups["Desc"]["Description"];
            Version = Groups["Desc"]["Version"];
            Date = Groups["Desc"]["Date"];
            Author = Groups["Desc"]["Author"];
            var includeDirCount = int.Parse(Groups["Main"]["IncludeDirCount"]);
            for (int i = 0; i < includeDirCount; i++)
            {
                IncludeDirs.Add(Groups["Main"][$"IncludeDir{i}"]);
            }
        }
    }
}