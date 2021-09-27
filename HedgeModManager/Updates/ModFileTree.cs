using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.Updates
{
    [JsonObject]
    public class ModFileTree : ModFileEntry
    {
        public const string FixedFileName = ".hmmtree";

        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = IgnoreEmptyEnumerableResolver.Instance
        };

        [JsonIgnore]
        public new string Name { get; }

        public ModFileTree()
        {
            Name = DirectorySeparatorCharAsString;
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented, JsonSettings));
        }

        public static ModFileTree Load(string path)
        {
            if (!File.Exists(path))
                return null;

            using var stream = File.OpenRead(path);
            return Load(stream);
        }

        public static ModFileTree Load(Stream input)
        {
            using var reader = new StreamReader(input);
            using var jsReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var item = serializer.Deserialize<ModFileTree>(jsReader);
            item?.FixChildren();

            return item;
        }

        public static async Task<ModFileTree> LoadFromUrl(string url, CancellationToken cancellationToken = default)
        {
            var response = await Singleton.GetInstance<HttpClient>().GetAsync(url, cancellationToken)
                .ConfigureAwait(false);

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
