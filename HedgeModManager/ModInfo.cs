using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Serialization;

namespace HedgeModManager
{
    public class ModInfo
    {
        public string RootDirectory;

        [PropertyIgnore]
        public bool Enabled { get; set; }

        [PropertyIgnore]
        public bool HasUpdates => !string.IsNullOrEmpty(UpdateServer);

        [PropertyIgnore]
        public bool SupportsSave => !string.IsNullOrEmpty(SaveFile);
        
        // Main
        [IniField("Main")]
        public string UpdateServer { get; set; }

        [IniField("Main")]
        public string SaveFile { get; set; }

        [IniField("Main", "IncludeDir")]
        public List<string> IncludeDirs { get; set; } = new List<string>();

        // Desc
        [IniField("Desc")]
        public string Title { get; set; }

        [IniField("Desc")]
        public string Description { get; set; }

        [IniField("Desc")]
        public string Version { get; set; }

        [IniField("Desc")]
        public string Date { get; set; }

        [IniField("Desc")]
        public string Author { get; set; }

        public ModInfo()
        {

        }

        public ModInfo(string modPath)
        {
            RootDirectory = modPath;
            using (var stream = File.OpenRead(Path.Combine(modPath, "mod.ini")))
            {
                if (!Read(stream))
                {
                    // Close the file so we can write a valid file back
                    IncludeDirs.Add(".");
                    stream.Close();
                    Title = Path.GetFileName(RootDirectory);
                    Description = "None";
                    Save();
                }
            }
        }

        public bool Read(Stream stream)
        {
            try
            {
                IniSerializer.Deserialize(this, stream);
            }
            catch
            {
                return false;
            }

            Description = Description.Replace("\\n", "\n");
            return true;
        }

        public void Save()
        {
            using (var stream = File.Create(Path.Combine(RootDirectory, "mod.ini")))
            {
                IniSerializer.Serialize(this, stream);
            }
        }
    }
}