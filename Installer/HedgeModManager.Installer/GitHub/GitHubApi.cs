namespace HedgeModManager.Installer.GitHub;
using Newtonsoft.Json;

public class GitHubApi
{
    private const string PathSeparator = "/";
    private const string ApiBaseUrl = "https://api.github.com";
    private const string GitHubReposDirectory = "repos";

    public static Task<ReleaseInfo> GetLatestRelease(string username, string repo)
    {
        return GetObject<ReleaseInfo>(CombinePathURL(ApiBaseUrl, GitHubReposDirectory, username, repo, "releases", "latest"));
    }

    public static async Task<T> GetObject<T>(string url)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(await Application.Current.Http.GetStringAsync(url));
        }
        catch
        {
            return default;
        }
    }

    private static string CombinePathURL(params string[] paths)
    {
        return string.Join(PathSeparator, paths);
    }
}