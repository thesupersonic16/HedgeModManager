using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class ModDownload
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("game")]
        private string GameName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; } = "";
        [JsonProperty("authors")]
        public Dictionary<string, List<Author>> Authors { get; set; } = new Dictionary<string, List<Author>>();
        [JsonProperty("images")]
        public List<string> Images { get; set; } = new List<string>();
        [JsonProperty("download")]
        public string DownloadURL { get; set; }
        [JsonIgnore]
        public Game Game { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Game = Games.GetSupportedGames().FirstOrDefault(t => t.GameName == GameName);
        }

    }

    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        [JsonProperty("link")]
        public string Link { get; set; } = null;
        [JsonProperty("description")]
        public string Description { get; set; } = "";
    }
}
