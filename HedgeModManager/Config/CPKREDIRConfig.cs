using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class CPKREDIRConfig : IniFile
    {
        // CPKREDIR
        public bool Enabled
        {
            get { return (int)this["CPKREDIR"]["Enabled", typeof(int)] != 0; }
            set { this["CPKREDIR"]["Enabled"] = (value ? "1" : "0"); }
        }

        public bool PlaceTocAtEnd
        {
            get { return (int)this["CPKREDIR"]["PlaceTocAtEnd", typeof(int)] != 0; }
            set { this["CPKREDIR"]["PlaceTocAtEnd"] = (value ? "1" : "0"); }
        }

        public bool HandleCpksWithoutExtFiles
        {
            get { return (int)this["CPKREDIR"]["HandleCpksWithoutExtFiles", typeof(int)] != 0; }
            set { this["CPKREDIR"]["HandleCpksWithoutExtFiles"] = (value ? "1" : "0"); }
        }

        public string LogFile
        {
            get { return this["CPKREDIR"]["LogFile"]; }
            set { this["CPKREDIR"]["LogFile"] = value; }
        }

        public int ReadBlockSizeKB
        {
            get { return (int)this["CPKREDIR"]["ReadBlockSizeKB", typeof(int)]; } 
            set { this["CPKREDIR"]["ReadBlockSizeKB"] = value.ToString(); }
        }

        public string ModsDbIni
        {
            get { return this["CPKREDIR"]["ModsDbIni"]; }
            set { this["CPKREDIR"]["ModsDbIni"] = value; }
        }

        public bool EnableSaveFileRedirection
        {
            get { return (int)this["CPKREDIR"]["EnableSaveFileRedirection", typeof(int)] != 0; }
            set { this["CPKREDIR"]["EnableSaveFileRedirection"] = (value ? "1" : "0"); }
        }

        public bool EnableDebugConsole
        {
            get { return this["CPKREDIR"]["LogType", "none"] == "console"; }
            set { this["CPKREDIR"]["LogType"] = (value ? "console" : "none"); }
        }

        public string SaveFileFallback
        {
            get { return this["CPKREDIR"]["SaveFileFallback"]; }
            set { this["CPKREDIR"]["SaveFileFallback"] = value; }
        }

        public string SaveFileOverride
        {
            get { return this["CPKREDIR"]["SaveFileOverride"]; }
            set { this["CPKREDIR"]["SaveFileOverride"] = value; }
        }

        // HedgeModManager
        public bool CheckForUpdates
        {
            get { return (int)this["HedgeModManager"]["AutoCheckForUpdates", typeof(int)] != 0; }
            set { this["HedgeModManager"]["AutoCheckForUpdates"] = (value ? "1" : "0"); }
        }

        public bool KeepOpen
        {
            get { return (int)this["HedgeModManager"]["KeepModLoaderOpen", typeof(int)] != 0; }
            set { this["HedgeModManager"]["KeepModLoaderOpen"] = (value ? "1" : "0"); }
        }

        public bool CheckLoaderUpdates
        {
            get { return (int)this["HedgeModManager"]["CheckLoader", typeof(int)] != 0; }
            set { this["HedgeModManager"]["CheckLoader"] = (value ? "1" : "0"); }
        }

        public CPKREDIRConfig(string path)
        {
            if (File.Exists(path))
                using (var stream = File.OpenRead(path))
                    Read(stream);
            else
            {
                Groups.Add("CPKREDIR", new IniGroup());

                // CPKREDIR
                Enabled = true;
                PlaceTocAtEnd = true;
                HandleCpksWithoutExtFiles = false;
                LogFile = "cpkredir.log";
                ReadBlockSizeKB = 4096;
                ModsDbIni = "mods\\ModsDB.ini";
                EnableSaveFileRedirection = false;
                SaveFileFallback = "cpkredir.sav";
                SaveFileOverride = "";
            }
            if (!Groups.ContainsKey("HedgeModManager"))
            {
                Groups.Add("HedgeModManager", new IniGroup());
                // HedgeModManager
                CheckForUpdates = true;
                KeepOpen = false;
                CheckLoaderUpdates = true;
            }
        }

        public void Save(string path)
        {
            using (var stream = File.Create(path))
            {
                Write(stream);
            }
        }

    }
}
