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

        [PropertyIgnore]
        [IniField("Main", "IncludeDir")]
        public List<string> IncludeDirs { get; set; } = new List<string>();

        [IniField("Main")]
        public string DLLFile { get; set; } = string.Empty;

        // Desc
        [IniField("Desc")]
        public string Title { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Description { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Version { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Date { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Author { get; set; } = string.Empty;

        [PropertyIgnore]
        [IniField("CPKs")]
        public Dictionary<string, string> CPKs { get; set; } = new Dictionary<string, string>();

        public ModInfo()
        {

        }

        public ModInfo(string modPath)
        {
            RootDirectory = modPath;
            try
            {
                using (var stream = File.OpenRead(Path.Combine(modPath, "mod.ini")))
                {
                    if (!Read(stream))
                    {
                        // Close the file so we can write a valid file back
                        stream.Close();

                        IncludeDirs.Add(".");
                        Title = Path.GetFileName(RootDirectory);
                        Version = "0.0";
                        Date ="Unknown";
                        Author = "Unknown";
                        Description = "None";
                        Save();
                    }
                }

                if (IncludeDirs.Count < 1 && string.IsNullOrEmpty(DLLFile))
                {
                    IncludeDirs.Add(".");
                    Title = string.IsNullOrEmpty(Title) ? Path.GetFileName(RootDirectory) : Title;
                    Version = string.IsNullOrEmpty(Version) ? "0.0" : Version;
                    Date = string.IsNullOrEmpty(Date) ? "Unknown" : Date;
                    Author = string.IsNullOrEmpty(Author) ? "Unknown" : Author;
                    Description = string.IsNullOrEmpty(Description) ? "None" : Description;
                    Save();
                }
            }
            catch
            {
                Title = Path.GetFileName(RootDirectory);
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