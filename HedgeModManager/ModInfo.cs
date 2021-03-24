using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Serialization;
using HedgeModManager.UI;
using Newtonsoft.Json;
using PropertyTools.DataAnnotations;

namespace HedgeModManager
{
    public class ModInfo : INotifyPropertyChanged
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        public IEnumerable<Column> IncludeDirColumns { get; } = new[]
        {
            new Column("Value", "Directory", null, "*", 'L'),
        };

        public string RootDirectory;
        public FormSchema ConfigSchema;

        [PropertyTools.DataAnnotations.Browsable(false)]
        public bool Enabled { get; set; }

        [PropertyTools.DataAnnotations.Browsable(false)]
        public CodeFile Codes { get; set; }
        
        [PropertyTools.DataAnnotations.Browsable(false)]
        public bool HasUpdates => !string.IsNullOrEmpty(UpdateServer);

        [PropertyTools.DataAnnotations.Browsable(false)]
        public bool SupportsSave => !string.IsNullOrEmpty(SaveFile);

        [PropertyTools.DataAnnotations.Browsable(false)]
        public bool HasSchema => ConfigSchema != null;
        
        [PropertyTools.DataAnnotations.Browsable(false)]
        public bool Favorite { get; set; }

        // Main
        [PropertyTools.DataAnnotations.Category("Main")]
        [IniField("Main")]
        public string UpdateServer { get; set; }

        [IniField("Main")]
        public string SaveFile { get; set; }

        [PropertyTools.DataAnnotations.DisplayName("Include Directories")]
        [ColumnsProperty(nameof(IncludeDirColumns))]
        public ObservableCollection<StringWrapper> IncludeDirsProperty { get; set; } = new ObservableCollection<StringWrapper>();

        [PropertyTools.DataAnnotations.Browsable(false)]
        [IniField("Main", "IncludeDir")]
        public List<string> IncludeDirs { get; set; } = new List<string>();

        [PropertyTools.DataAnnotations.DisplayName("DLL File")]
        [IniField("Main")]
        public string DLLFile { get; set; } = string.Empty;

        [PropertyTools.DataAnnotations.DisplayName("Code File")]
        [IniField("Main")]
        public string CodeFile { get; set; } = string.Empty;

        [IniField("Main")]
        public string ConfigSchemaFile { get; set; } = string.Empty;

        // Desc
        [PropertyTools.DataAnnotations.Category("Description")]
        [IniField("Desc")]
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [IniField("Desc")]
        public string Description { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Version { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Date { get; set; } = string.Empty;

        [IniField("Desc")]
        public string Author { get; set; } = string.Empty;

        [IniField("Desc")]
        public string AuthorURL { get; set; } = string.Empty;

        [PropertyTools.DataAnnotations.Browsable(false)]
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

                    var schemaPath = Path.Combine(RootDirectory, ConfigSchemaFile);
                    if (File.Exists(schemaPath))
                    {
                        ConfigSchema = JsonConvert.DeserializeObject<FormSchema>(File.ReadAllText(schemaPath));
                        ConfigSchema.LoadValuesFromIni(Path.Combine(RootDirectory, ConfigSchema.IniFile));
                    }

                    var codesPath = Path.Combine(RootDirectory, CodeFile);
                    if (File.Exists(codesPath))
                    {
                        Codes = HedgeModManager.CodeFile.FromFile(codesPath);
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

            foreach (var dir in IncludeDirs)
            {
                IncludeDirsProperty.Add(new StringWrapper(dir));
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

        public bool ValidateIncludeDirectories()
        {
            var valid = true;
            foreach (var dir in IncludeDirs)
            {
                try
                {
                    if (!Directory.Exists(Path.Combine(RootDirectory, dir)))
                    {
                        valid = false;
                        break;
                    }
                }
                catch
                {
                    valid = false;
                    break;
                }
            }

            return valid;
        }

        public void FixIncludeDirectories()
        {
            var validDirs = new List<string>();
            foreach (var includeDir in IncludeDirs)
            {
                try
                {
                    if (Directory.Exists(Path.Combine(RootDirectory, includeDir)))
                        validDirs.Add(includeDir);
                }
                catch { }
            }

            if(validDirs.Count == 0)
                validDirs.Add(".");

            IncludeDirs = validDirs;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class StringWrapper
    {
        public string Value { get; set; }

        public StringWrapper() { }

        public StringWrapper(string str)
            => Value = str;
    }
}