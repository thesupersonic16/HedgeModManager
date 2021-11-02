using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.GitHub
{
    public class ArtifactInfo
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("node_id")]
        public string NodeID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size_in_bytes")]
        public ulong Size { get; set; }

        [JsonProperty("url")]
        public Uri URL { get; set; }

        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }

        public class ArtifactList
        {
            [JsonProperty("artifacts")]
            public List<ArtifactInfo> Artifacts { get; set; }
        }

    }
}
