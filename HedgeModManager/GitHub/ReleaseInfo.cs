using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HedgeModManager.GitHub
{
    public class ReleaseInfo
    {
        [JsonProperty("url")]
        public Uri URL { get; set; }

        [JsonProperty("assets_url")]
        public Uri AssetsURL { get; set; }

        [JsonProperty("upload_url")]
        public Uri UploadURL { get; set; }

        [JsonProperty("html_url")]
        public Uri HTMLUrl { get; set; }

        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("node_id")]
        public string NodeID { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("draft")]
        public bool Draft { get; set; }

        [JsonProperty("author")]
        public UserInfo Author { get; set; }

        [JsonProperty("prerelease")]
        public bool IsPreRelease { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("published_at")]
        public DateTimeOffset PublishedAt { get; set; }

        [JsonProperty("assets")]
        public List<AssetInfo> Assets { get; set; } = new List<AssetInfo>();

        [JsonProperty("tarball_url")]
        public Uri TarballURL { get; set; }

        [JsonProperty("zipball_url")]
        public Uri ZipballURL { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        public Version GetVersion()
        {
            var pattern = @"(\d+)\.?(\d+)-?(\d+)?";
            var match = Regex.Match(TagName, pattern);
            int major = 0;
            int minor = 0;
            int rev = 0;

            switch (match.Groups.Count)
            {
                case 2:
                    int.TryParse(match.Groups[1].Value, out major);
                    break;

                case 3:
                    int.TryParse(match.Groups[1].Value, out major);
                    int.TryParse(match.Groups[2].Value, out minor);
                    break;

                case 4:
                    int.TryParse(match.Groups[1].Value, out major);
                    int.TryParse(match.Groups[2].Value, out minor);
                    int.TryParse(match.Groups[3].Value, out rev);
                    break;
            }

            return new Version(major, minor, 0, rev);
        }
    }
}
