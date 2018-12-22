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
            get { return (int)Groups["CPKREDIR"]["Enabled", typeof(int)] != 0; }
            set { Groups["CPKREDIR"]["Enabled"] = (value ? "1" : "0"); }
        }

        public bool PlaceTocAtEnd
        {
            get { return (int)Groups["CPKREDIR"]["PlaceTocAtEnd", typeof(int)] != 0; }
            set { Groups["CPKREDIR"]["PlaceTocAtEnd"] = (value ? "1" : "0"); }
        }

        public bool HandleCpksWithoutExtFiles
        {
            get { return (int)Groups["CPKREDIR"]["HandleCpksWithoutExtFiles", typeof(int)] != 0; }
            set { Groups["CPKREDIR"]["HandleCpksWithoutExtFiles"] = (value ? "1" : "0"); }
        }

        public string LogFile
        {
            get { return Groups["CPKREDIR"]["LogFile"]; }
            set { Groups["CPKREDIR"]["LogFile"] = value; }
        }

        public int ReadBlockSizeKB
        {
            get { return (int)Groups["CPKREDIR"]["ReadBlockSizeKB", typeof(int)]; } 
            set { Groups["CPKREDIR"]["ReadBlockSizeKB"] = value.ToString(); }
        }

        public string ModsDbIni
        {
            get { return Groups["CPKREDIR"]["ModsDbIni"]; }
            set { Groups["CPKREDIR"]["ModsDbIni"] = value; }
        }

        public bool EnableSaveFileRedirection
        {
            get { return (int)Groups["CPKREDIR"]["EnableSaveFileRedirection", typeof(int)] != 0; }
            set { Groups["CPKREDIR"]["EnableSaveFileRedirection"] = (value ? "1" : "0"); }
        }

        public bool EnableDebugConsole
        {
            get { return Groups["CPKREDIR"]["LogType"] == "console"; }
            set { Groups["CPKREDIR"]["LogType"] = (value ? "console" : "none"); }
        }

        public string SaveFileFallback
        {
            get { return Groups["CPKREDIR"]["SaveFileFallback"]; }
            set { Groups["CPKREDIR"]["SaveFileFallback"] = value; }
        }

        public string SaveFileOverride
        {
            get { return Groups["CPKREDIR"]["SaveFileOverride"]; }
            set { Groups["CPKREDIR"]["SaveFileOverride"] = value; }
        }

        // HedgeModManager
        public bool CheckForUpdates
        {
            get { return (int)Groups["HedgeModManager"]["AutoCheckForUpdates", typeof(int)] != 0; }
            set { Groups["HedgeModManager"]["AutoCheckForUpdates"] = (value ? "1" : "0"); }
        }

        public bool KeepOpen
        {
            get { return (int)Groups["HedgeModManager"]["KeepModLoaderOpen", typeof(int)] != 0; }
            set { Groups["HedgeModManager"]["KeepModLoaderOpen"] = (value ? "1" : "0"); }
        }

        public bool CheckLoaderUpdates
        {
            get { return (int)Groups["HedgeModManager"]["CheckLoader", typeof(int)] != 0; }
            set { Groups["HedgeModManager"]["CheckLoader"] = (value ? "1" : "0"); }
        }

        public string ModLoaderVersion
        {
            get { return Groups["HedgeModManager"]["ModLoaderVersion"]; }
            set { Groups["HedgeModManager"]["ModLoaderVersion"] = value; }
        }

        public string ModLoaderName
        {
            get { return Groups["HedgeModManager"]["ModLoaderName"]; }
            set { Groups["HedgeModManager"]["ModLoaderName"] = value; }
        }

        public CPKREDIRConfig(string path)
        {
            if (File.Exists(path))
                using (var stream = File.OpenRead(path))
                    Read(stream);
            else
            {
                Groups.Add("CPKREDIR", new IniGroup());
                Groups.Add("HedgeModManager", new IniGroup());
                
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

                // HedgeModManager
                CheckForUpdates = true;
                KeepOpen = false;
                CheckLoaderUpdates = true;
                ModLoaderVersion = "";
                ModLoaderName = "";
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
