using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.Github
{
    public static class GithubAPI
    {
        private const string PathSeperator = "/";
        private const string GithubAPIUrl = "https://api.github.com";
        private const string GithubReposDirectory = "repos";

        public static Task<ReleaseInfo> GetLatestRelease(string username, string repo)
        {
            return GetObject<ReleaseInfo>(CombinePathURL(GithubAPIUrl, GithubReposDirectory, username, repo, "releases", "latest"));
        }

        public static Task<WorkflowRunInfo.WorkflowList> GetAllRuns(string username, string repo)
        {
            return GetObject<WorkflowRunInfo.WorkflowList>(CombinePathURL(GithubAPIUrl, GithubReposDirectory, username, repo, "actions", "runs"));
        }

        public static Task<GHCommitInfo[]> GetAllCommits(string username, string repo, string hash = null)
        {
            if (hash == null)
                return GetObject<GHCommitInfo[]>(CombinePathURL(GithubAPIUrl, GithubReposDirectory, username, repo, "actions", "runs"));
            return GetObject<GHCommitInfo[]>(CombinePathURL(GithubAPIUrl, GithubReposDirectory, username, repo, "commits") + "?sha=" + hash);
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
