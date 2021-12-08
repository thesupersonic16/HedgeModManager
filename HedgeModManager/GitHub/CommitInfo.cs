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
        // From: https://github.blog/changelog/2021-02-08-github-actions-skip-pull-request-and-push-workflows-with-skip-ci/
        private readonly string[] noCIStrings = { "[skip ci]", "[ci skip]", "[no ci]", "[skip actions]", "[actions skip]" };

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

        public bool IsSkipCI()
        {
            foreach (string noCiString in noCIStrings)
            {
                if (Message.Contains(noCiString)) return true;
            }
            return false;
        }
    }
}
