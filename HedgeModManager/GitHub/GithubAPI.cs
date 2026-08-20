using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.GitHub
{
    public static class GitHubAPI
    {
        private const string PathSeperator = "/";
        private const string GitHubAPIUrl = "https://api.github.com";
        private const string GitHubReposDirectory = "repos";

        public static Task<ReleaseInfo> GetLatestRelease(string username, string repo)
        {
            return GetObject<ReleaseInfo>(CombinePathURL(GitHubAPIUrl, GitHubReposDirectory, username, repo, "releases", "latest"));
        }

        public static Task<WorkflowRunInfo.WorkflowList> GetAllRuns(string username, string repo, string workflow = null)
        {
            if (!string.IsNullOrEmpty(workflow))
                return GetObject<WorkflowRunInfo.WorkflowList>(CombinePathURL(GitHubAPIUrl, GitHubReposDirectory, username, repo, "actions", "workflows", workflow, "runs"));
            return GetObject<WorkflowRunInfo.WorkflowList>(CombinePathURL(GitHubAPIUrl, GitHubReposDirectory, username, repo, "actions", "runs"));
        }

        public static Task<GHCommitInfo[]> GetAllCommits(string username, string repo, string hash = null)
        {
            if (hash == null)
                return GetObject<GHCommitInfo[]>(CombinePathURL(GitHubAPIUrl, GitHubReposDirectory, username, repo, "actions", "runs"));
            return GetObject<GHCommitInfo[]>(CombinePathURL(GitHubAPIUrl, GitHubReposDirectory, username, repo, "commits") + "?sha=" + hash);
        }

        public static async Task<T> GetObject<T>(string url)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(await Singleton.GetInstance<HttpClient>().GetStringAsync(url));
            }
            catch
            {
                return default;
            }
        }

        private static string CombinePathURL(params string[] paths)
        {
            return string.Join(PathSeperator, paths);
        }
    }
}
