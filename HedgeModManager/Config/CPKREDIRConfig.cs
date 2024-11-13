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
        [IniField("CPKREDIR", true)]
        public bool Enabled { get; set; } = true;

        [IniField("CPKREDIR", true)]
        public bool PlaceTocAtEnd { get; set; } = true;

        [IniField("CPKREDIR", true)]
        public bool HandleCpksWithoutExtFiles { get; set; } = false;

        [IniField("CPKREDIR", true)]
        public string LogFile { get; set; } = "cpkredir.log";

        [IniField("CPKREDIR", true)]
        public int ReadBlockSizeKB { get; set; } = 4096;

        [IniField("CPKREDIR", true)]
        public string ModsDbIni { get; set; } = null;

        [IniField("CPKREDIR", "EnableSaveFileRedirection", true)]
        public bool EnableSaveFileRedirection { get; set; } = false;

        [IniField("CPKREDIR", true)]
        public string SaveFileFallback { get; set; } = string.Empty;

        [IniField("CPKREDIR", true)]
        public string SaveFileOverride { get; set; } = string.Empty;

        [IniField("CPKREDIR")]
        public string LogType { get; set; }

        // HedgeModManager
        [IniField("HedgeModManager", nameof(EnableFallbackSaveRedirection), true)]
        public bool EnableFallbackSaveRedirection { get; set; } = true;

        [IniField("HedgeModManager", "ModProfile", true)]
        public string ModProfile { get; set; } = "Default";

        [IniField("HedgeModManager", "UseLauncher", true)]
        public bool UseLauncher { get; set; } = true;

        public bool EnableDebugConsole 
        { 
            get => LogType == "console"; 
            set => LogType = value ? "console" : "file";
        }

        public CPKREDIRConfig(string path)
        {
            if (File.Exists(path))
                Load(path);
            else
                Save(path);
        }

        public CPKREDIRConfig(GameInstall gameInstall)
        {
            string path = Path.Combine(gameInstall.GameDirectory, "cpkredir.ini");
            if (File.Exists(path))
            {
                Load(path);
            }
            else
            {
                ModsDbIni = Path.Combine(gameInstall.Game.ModsDirectoryName, "ModsDB.ini");
                Save(path);
            }
        }

        public void Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var file = new IniFile(stream);
                IniSerializer.Deserialize(this, file);
            }

            // Remove corrupted ModsDbIni
            try
            {
                if (ModsDbIni != null && Path.GetInvalidPathChars().Any(ModsDbIni.Contains))
                    ModsDbIni = null;

                Path.GetFullPath(ModsDbIni);
            }
            catch
            {
                ModsDbIni = null;
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
