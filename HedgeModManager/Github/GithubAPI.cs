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

        public static ReleaseInfo GetLatestRelease(string username, string repo)
        {
            return GetObject<ReleaseInfo>(CombinePathURL(GithubAPIUrl, GithubReposDirectory, username, repo, "releases", "latest"));
        }

        public static T GetObject<T>(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", HedgeApp.WebRequestUserAgent);
                    return JsonConvert.DeserializeObject<T>(client.DownloadString(url));
                }
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
