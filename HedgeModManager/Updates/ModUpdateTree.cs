using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.Updates
{
    [JsonObject]
    public class ModUpdateTree : ModUpdateEntry
    {
        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = IgnoreEmptyEnumerableResolver.Instance
        };

        [JsonIgnore]
        public new string Name { get; }

        public ModUpdateTree()
        {
            Name = DirectorySeparatorCharAsString;
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented, JsonSettings));
        }

        public static ModUpdateTree Load(Stream input)
        {
            using var reader = new StreamReader(input);
            using var jsReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var item = serializer.Deserialize<ModUpdateTree>(jsReader);
            item?.FixChildren();

            return item;
        }

        public static async Task<ModUpdateTree> LoadFromUrl(string url)
        {
            var response = await HedgeApp.HttpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            using var readStream = await response.Content.ReadAsStreamAsync();
            return Load(readStream);
        }

        public override string ToString()
        {
            return DirectorySeparatorCharAsString;
        }
    }
}
