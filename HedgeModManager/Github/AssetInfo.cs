using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.GitHub
{
    public class AssetInfo
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("node_id")]
        public string NodeID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("uploader")]
        public UserInfo Uploader { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("size")]
        public ulong Size { get; set; }

        [JsonProperty("download_count")]
        public ulong DownloadCount { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("browser_download_url")]
        public Uri BrowserDownloadUrl { get; set; }
    }
}
