using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.GitHub
{
    public class CommitInfo
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("tree_id")]
        public string TreeID { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("Author")]
        public AuthorInfo Author { get; set; }

        [JsonProperty("committer")]
        public AuthorInfo Committer { get; set; }
    }
}
