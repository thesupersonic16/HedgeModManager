using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public static async Task<T> GetObject<T>(string url)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(await HedgeApp.HttpClient.GetStringAsync(url));
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
