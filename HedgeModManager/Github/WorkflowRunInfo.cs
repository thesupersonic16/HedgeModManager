using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.GitHub
{
    public class WorkflowRunInfo
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("node_id")]
        public string NodeID { get; set; }

        [JsonProperty("head_branch")]
        public string HeadBranch { get; set; }

        [JsonProperty("head_sha")]
        public string HeadSHA { get; set; }

        [JsonProperty("run_number")]
        public ulong RunNumber { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("conclusion")]
        public string Conclusion { get; set; }

        [JsonProperty("workflow_id")]
        public ulong WorkflowID { get; set; }

        [JsonProperty("check_suite_id")]
        public ulong CheckSuiteID { get; set; }
        
        [JsonProperty("check_suite_node_id")]
        public string CheckSuiteNodeID { get; set; }


        // ...

        [JsonProperty("url")]
        public Uri URL { get; set; }

        [JsonProperty("html_url")]
        public Uri HTMLURL { get; set; }

        // ...

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        // ...

        [JsonProperty("artifacts_url")]
        public Uri ArtifactsURL { get; set; }

        // ...

        [JsonProperty("head_commit")]
        public CommitInfo HeadCommit { get; set; }

        // ...

        public class WorkflowList
        {
            [JsonProperty("workflow_runs")]
            public List<WorkflowRunInfo> Runs { get; set; }
        }
    }
}
