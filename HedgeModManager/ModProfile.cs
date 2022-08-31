using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Misc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace HedgeModManager
{
    public class ModProfile : INotifyPropertyChanged
    {
        public static JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        [JsonIgnore]
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string ModDBPath { get; set; }
        public string FileName => string.Join(string.Empty, Name.Split(Path.GetInvalidFileNameChars())).Replace(" ", "") + ".ini";

        public ModProfile(string name, string modDBPath)
        {
            Name = name;
            ModDBPath = modDBPath;
        }

        public void GeneratePath()
        {
            ModDBPath = GeneratePath(Name);
        }

        public static string GeneratePath(string name)
        {
            string basePath = string.Join(string.Empty, name.Split(Path.GetInvalidFileNameChars())).Replace(" ", "");
            string path = basePath + ".ini";

            for (int i = 0; File.Exists(Path.Combine(HedgeApp.ModsDbPath, path)); i++)
            {
                path = $"{basePath}{i}.ini";
            }

            return path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Export(string path)
        {
            var ex = ExportProfile.Create(this);
            File.WriteAllText(path, JsonConvert.SerializeObject(ex, Formatting.Indented, JsonSettings));
        }
    }

    public class ExportProfile
    {
        public const string ObjectType = "profile";
        public string Type => ObjectType;
        public int Version => 1;

        public string Name { get; set; }
        public List<Mod> ActiveMods { get; set; } = new List<Mod>();
        public List<Mod> FavoriteMods { get; set; } = new List<Mod>();
        public List<string> ActiveCodes { get; set; } = new List<string>();

        public void FromProfile(ModProfile profile)
        {
            Name = profile.Name;

            if (!File.Exists(Path.Combine(HedgeApp.ModsDbPath, profile.ModDBPath)))
                return;

            var db = new ModsDB(HedgeApp.ModsDbPath, profile.ModDBPath);
            foreach (var mod in db.Mods)
            {
                if (mod.Enabled)
                    ActiveMods.Add(CreateExportMod(mod));

                if (mod.Favorite)
                    FavoriteMods.Add(CreateExportMod(mod));
            }

            foreach (string code in db.Codes)
                ActiveCodes.Add(code);

            Mod CreateExportMod(ModInfo modInfo)
            {
                string config = null;
                string profileConfigPath = Path.Combine(modInfo.RootDirectory, "profiles", profile.FileName);
                if (File.Exists(profileConfigPath))
                    config = File.ReadAllText(profileConfigPath);
                return new Mod { ID = modInfo.ID, Name = modInfo.Title, Config = config };
            }
        }

        public static ExportProfile Create(ModProfile profile)
        {
            var exProfile = new ExportProfile();
            exProfile.FromProfile(profile);
            return exProfile;
        }

        public static ImportResult Import(string path)
        {
            using var stream = File.OpenRead(path);
            return Import(stream);
        }

        public static ImportResult Import(Stream file)
        {
            var result = new ImportResult();
            using var reader = new StreamReader(file);

            ExportProfile profile;
            try
            {
                var jsReader = new JsonTextReader(reader);
                var jObj = JObject.Load(jsReader);
                var type = jObj.GetValue(nameof(Type), StringComparison.InvariantCultureIgnoreCase)?.Value<string>();

                if (string.IsNullOrEmpty(type))
                    return result;

                if (!type.Equals(ObjectType, StringComparison.InvariantCultureIgnoreCase))
                    return result;

                profile = jObj.ToObject<ExportProfile>(new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            catch
            {
                return result;
            }

            if (profile == null)
                return result;

            var db = new ModsDB
            {
                RootDirectory = HedgeApp.ModsDbPath,
                FileName = ModProfile.GeneratePath(profile.Name)
            };
            db.DetectMods();
            result.Database = db;
            result.Profile = new ModProfile(profile.Name, db.FileName);

            foreach (var mod in profile.ActiveMods)
            {
                var dbMod = db.Mods.FirstOrDefault(m => m.ID == mod.ID);
                if (dbMod == null)
                {
                    result.UnresolvedMods.Add(mod);
                    continue;
                }
                if (!string.IsNullOrEmpty(mod.Config))
                {
                    string profileConfigPath = Path.Combine(dbMod.RootDirectory, "profiles", result.Profile.FileName);
                    if (!Directory.Exists(Path.GetDirectoryName(profileConfigPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(profileConfigPath));
                    File.WriteAllText(profileConfigPath, mod.Config);
                }
                
                dbMod.Enabled = true;
            }

            foreach (var mod in profile.FavoriteMods)
            {
                var dbMod = db.Mods.FirstOrDefault(m => m.ID == mod.ID);
                if (dbMod == null)
                {
                    result.UnresolvedMods.Add(mod);
                    continue;
                }

                dbMod.Favorite = true;
            }

            foreach (var code in profile.ActiveCodes)
            {
                var dbCode = MainWindow.CodesDatabase.Codes.FirstOrDefault(c => c.Name == code);
                if (dbCode == null)
                {
                    result.UnresolvedCodes.Add(code);
                    continue;
                }

                db.Codes.Add(code);
            }

            result.UnresolvedMods = result.UnresolvedMods.DistinctBy(m => m.ID).ToList();
            return result;
        }

        public class Mod
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Config { get; set; }
        }

        public class ImportResult
        {
            public ModProfile Profile { get; set; }
            public ModsDB Database { get; set; }
            public List<Mod> UnresolvedMods { get; set; } = new List<Mod>();
            public List<string> UnresolvedCodes { get; set; } = new List<string>();
            public bool HasErrors => UnresolvedMods.Count > 0 || UnresolvedCodes.Count > 0;
            public bool IsInvalid => Database == null;

            public string BuildMarkdown()
            {
                var builder = new StringBuilder();
                builder.AppendLine(Lang.Localise("ProfileWindowUIImportMissingMods"));

                foreach (var mod in UnresolvedMods)
                {
                    builder.AppendLine($"- {mod.Name}");
                }

                builder.AppendLine();

                if (UnresolvedCodes.Count > 0)
                    builder.AppendLine(Lang.Localise("ProfileWindowUIImportMissingCodes"));

                foreach (var code in UnresolvedCodes)
                {
                    builder.AppendLine($"- {code}");
                }

                return builder.ToString();
            }
        }
    }
}
