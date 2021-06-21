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
using HedgeModManager.Misc;
using HedgeModManager.Serialization;
using HedgeModManager.UI;
using Newtonsoft.Json;
using PropertyTools.DataAnnotations;

namespace HedgeModManager
{
    public class ModInfo : INotifyPropertyChanged
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        public IEnumerable<Column> IncludeDirColumns => StringWrapper.Columns;

        [PropertyTools.DataAnnotations.Browsable(false)]
        public IEnumerable<Column> ModDependsColumns => ModDepend.Columns;

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

        // Main
        [PropertyTools.DataAnnotations.Category("Main")]
        [IniField("Main")]
        public string UpdateServer { get; set; }

        [IniField("Main")]
        public string SaveFile { get; set; }

        [IniField("Main")]
        public string ID { get; set; }

        [PropertyTools.DataAnnotations.DisplayName("Include Directories")]
        [ColumnsProperty(nameof(IncludeDirColumns))]
        public ObservableCollection<StringWrapper> IncludeDirsProperty { get; set; } = new ObservableCollection<StringWrapper>();

        [PropertyTools.DataAnnotations.Browsable(false)]
        [IniField("Main", "IncludeDir")]
        public List<string> IncludeDirs { get; set; } = new List<string>();

        [PropertyTools.DataAnnotations.DisplayName("Dependencies")]
        [ColumnsProperty(nameof(ModDependsColumns))]
        [IniField("Main", "Depends")]
        public ObservableCollection<ModDepend> DependsOn { get; set; } = new ObservableCollection<ModDepend>();

        [PropertyTools.DataAnnotations.DisplayName("DLL File")]
        [IniField("Main")]
        public string DLLFile { get; set; } = string.Empty;

        [PropertyTools.DataAnnotations.DisplayName("Code File")]
        [IniField("Main")]
        public string CodeFile { get; set; } = string.Empty;

        [IniField("Main")]
        public string ConfigSchemaFile { get; set; } = string.Empty;

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
                        Date = "Unknown";
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

            if (string.IsNullOrEmpty(ID))
                ID = Title?.GetDeterministicHashCode().ToString("X") ?? modPath.GetHashCode().ToString("X");

            foreach (var dir in IncludeDirs)
                IncludeDirsProperty.Add(new StringWrapper(dir));
        }

        public bool Read(Stream stream)
        {
            try
            {
                IniSerializer.Deserialize(this, stream);
            }
            catch (Exception e)
            {
                return false;
            }

            Description = Description.Replace("\\n", "\n");
            return true;
        }

        public void Save()
        {
            Description = Description.Replace("\r", "").Replace("\n", "\\n");
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

            if (validDirs.Count == 0)
                validDirs.Add(".");

            IncludeDirs = validDirs;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class StringWrapper
    {
        public static IEnumerable<Column> Columns { get; } = new[]
        {
            new Column("Value", "Directory", null, "*", 'L'),
        };

        public string Value { get; set; }

        public StringWrapper() { }

        public StringWrapper(string str)
            => Value = str;
    }

    public class ModDepend : IIniConvertible
    {
        public static IEnumerable<Column> Columns { get; } = new[]
        {
            new Column(nameof(ID),    "ID", null, "*", 'L'),
            new Column(nameof(Title), "Title", null, "*", 'L'),
            new Column(nameof(VersionString),  "Version", null, "*", 'L'),
            new Column(nameof(Link),  "Link", null, "*", 'L'),
        };

        public string ID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public Version ModVersion { get; set; }

        public string VersionString
        {
            get => ModVersion?.ToString();
            set
            {
                if (Version.TryParse(value, out var result))
                    ModVersion = result;
            }
        }

        public bool HasLink => !string.IsNullOrEmpty(Link);
        private const string Delimiter = "|";

        public ModDepend()
        {

        }

        public ModDepend(string id, string title)
        {
            ID = id;
            Title = title;
        }

        public void ParseString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            var fields = input.Split(Delimiter[0]);
            for (int i = 0; i < fields.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        ID = fields[i];
                        break;

                    case 1:
                        Title = fields[i];
                        break;

                    case 2:
                        {
                            if (Version.TryParse(fields[i], out var version))
                                ModVersion = version;
                            else
                                Link = fields[i];

                            break;
                        }


                    case 3:
                        {
                            if (Version.TryParse(fields[i], out var version))
                                ModVersion = version;

                            break;
                        }
                }
            }
        }

        public static ModDepend FromString(string str)
        {
            var depend = new ModDepend();
            depend.ParseString(str);
            return depend;
        }

        public void FromIni(string value, IniConvertMeta data)
        {
            ParseString(value);
        }

        public string ToIni(IniFile file)
        {
            return string.Join(Delimiter, ID, Title, Link, VersionString);
        }
    }
}