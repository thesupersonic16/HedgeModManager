using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Type { get; } = ObjectType;
        public int Version { get; } = 1;
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
                    ActiveMods.Add(new Mod { ID = mod.ID, Name = mod.Title });

                if (mod.Favorite)
                    FavoriteMods.Add(new Mod { ID = mod.ID, Name = mod.Title });
            }

            foreach (string code in db.Codes)
                ActiveCodes.Add(code);
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
            var jsReader = new JsonTextReader(reader);
            var jObj = JObject.Load(jsReader);
            var type = jObj.GetValue(nameof(Type), StringComparison.InvariantCultureIgnoreCase)?.Value<string>();

            if (!string.IsNullOrEmpty(type) && !type.Equals(ObjectType, StringComparison.InvariantCultureIgnoreCase))
                return result;

            ExportProfile profile;
            try
            {
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
                db.Codes.Add(code);

            return result;
        }

        public class Mod
        {
            public string Name { get; set; }
            public string ID { get; set; }
        }

        public class ImportResult
        {
            public ModProfile Profile { get; set; }
            public ModsDB Database { get; set; }
            public List<Mod> UnresolvedMods { get; set; } = new List<Mod>();
            public bool HasErrors => UnresolvedMods.Count > 0;
            public bool IsInvalid => Database == null;

            public string BuildMarkdown()
            {
                var builder = new StringBuilder();
                builder.AppendLine(Lang.Localise("ProfileWindowUIImportMissingModsBody"));

                foreach (var mod in UnresolvedMods)
                {
                    builder.AppendLine($"- {mod.Name}");
                }

                return builder.ToString();
            }
        }
    }
}
