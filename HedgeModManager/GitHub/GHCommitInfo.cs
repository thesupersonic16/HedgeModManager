using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.GitHub
{
    public class GHCommitInfo
    {
        [JsonProperty("sha")]
        public string SHA { get; set; }

        [JsonProperty("node_id")]
        public string NodeID { get; set; }

        [JsonProperty("commit")]
        public CommitInfo Commit { get; set; }

        [JsonProperty("url")]
        public Uri URL { get; set; }

        [JsonProperty("html_url")]
        public Uri HTMLURL { get; set; }

        [JsonProperty("comments_url")]
        public Uri CommentsURL { get; set; }

        [JsonProperty("author")]
        public UserInfo Author { get; set; }

        [JsonProperty("committer")]
        public UserInfo Committer { get; set; }

        // ...
    }
}
