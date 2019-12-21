using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Serialization;

namespace HedgeModManager
{
    public class CPKREDIRConfig
    {
        // CPKREDIR
        [IniField("CPKREDIR")]
        public bool Enabled { get; set; }

        [IniField("CPKREDIR")]
        public bool PlaceTocAtEnd { get; set; }

        [IniField("CPKREDIR")]
        public bool HandleCpksWithoutExtFiles { get; set; }

        [IniField("CPKREDIR")]
        public string LogFile { get; set; }

        [IniField("CPKREDIR")]
        public int ReadBlockSizeKB { get; set; }

        [IniField("CPKREDIR")]
        public string ModsDbIni { get; set; }

        [IniField("CPKREDIR")]
        public bool EnableSaveFileRedirection { get; set; }

        [IniField("CPKREDIR")]
        public string SaveFileFallback { get; set; }

        [IniField("CPKREDIR")]
        public string SaveFileOverride { get; set; }

        [IniField("CPKREDIR")]
        public string LogType { get; set; }

        // HedgeModManager
        [IniField("HedgeModManager", "AutoCheckForUpdates")]
        public bool CheckForUpdates { get; set; }

        [IniField("HedgeModManager", "KeepModLoaderOpen")]
        public bool KeepOpen { get; set; }

        [IniField("HedgeModManager", "CheckLoader")]
        public bool CheckLoaderUpdates { get; set; }

        public bool EnableDebugConsole 
        { 
            get => LogType == "console"; 
            set => LogType = value ? "console" : "none";
        }

        public CPKREDIRConfig(string path)
        {
            if (File.Exists(path))
                using (var stream = File.OpenRead(path))
                    IniSerializer.Deserialize(this, stream);
            else
            {

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
            }
        }

        public void Save(string path)
        {
            using (var stream = File.Create(path))
            {
                IniSerializer.Serialize(this, stream);
            }
        }
    }
}
