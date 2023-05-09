using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Serialization;

namespace HedgeModManager
{
    public class CPKREDIRConfig : INotifyPropertyChanged
    {
        // CPKREDIR
        [IniField("CPKREDIR")]
        public bool Enabled { get; set; } = true;

        [IniField("CPKREDIR")]
        public bool PlaceTocAtEnd { get; set; } = true;

        [IniField("CPKREDIR")]
        public bool HandleCpksWithoutExtFiles { get; set; } = false;

        [IniField("CPKREDIR")]
        public string LogFile { get; set; } = "cpkredir.log";

        [IniField("CPKREDIR")]
        public int ReadBlockSizeKB { get; set; } = 4096;

        [IniField("CPKREDIR")]
        public string ModsDbIni { get; set; } = "mods\\ModsDB.ini";

        [IniField("CPKREDIR", "EnableSaveFileRedirection")]
        public bool EnableSaveFileRedirection { get; set; } = false;

        [IniField("CPKREDIR")]
        public string SaveFileFallback { get; set; } = string.Empty;

        [IniField("CPKREDIR")]
        public string SaveFileOverride { get; set; } = string.Empty;

        [IniField("CPKREDIR")]
        public string LogType { get; set; }

        // HedgeModManager
        [IniField("HedgeModManager", "AutoCheckForUpdates")]
        public bool CheckForUpdates { get; set; } = true;

        [IniField("HedgeModManager", "CheckLoader")]
        public bool CheckLoaderUpdates { get; set; } = true;

        [IniField("HedgeModManager", "CheckModUpdates")]
        public bool CheckForModUpdates { get; set; } = true;

        [IniField("HedgeModManager", "UpdateCodesOnLaunch")]
        public bool UpdateCodesOnLaunch { get; set; } = true;

        [IniField("HedgeModManager", "KeepModLoaderOpen")]
        public bool KeepOpen { get; set; } = true;

        [IniField("HedgeModManager", nameof(EnableFallbackSaveRedirection))]
        public bool EnableFallbackSaveRedirection { get; set; } = true;

        [IniField("HedgeModManager", "ModProfile")]
        public string ModProfile { get; set; } = "Default";

        [IniField("HedgeModManager", "UseLauncher")]
        public bool UseLauncher { get; set; } = true;

        public bool EnableDebugConsole 
        { 
            get => LogType == "console"; 
            set => LogType = value ? "console" : "file";
        }

        public CPKREDIRConfig(string path)
        {
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var file = new IniFile(stream);
                    IniSerializer.Deserialize(this, file);
                }
            }
            else
            {
                Save(path);
            }
        }

        public void Save(string path)
        {
            using (var stream = File.Create(path))
            {
                IniSerializer.Serialize(this, stream);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
