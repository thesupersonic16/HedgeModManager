namespace HedgeModManager.Steam;
using System.Runtime.Versioning;
using Microsoft.Win32;

public class SteamLocator
{
    private string? mSteamPath;

    [SupportedOSPlatform("windows")]
    private string? FindSteamLibraryWindows()
    {
        var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam") ??
                  RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey("SOFTWARE\\Valve\\Steam");

        if (key != null)
        {
            var path = key.GetValue("InstallPath")?.ToString();
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
        }

        return null;
    }

    private string? FindSteamLibraryUnix()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam", "steam");
        return Directory.Exists(path) ? path : null;
    }

    public string? FindDefaultSteamLibrary()
    {
        if (mSteamPath == null)
        {
            if (OperatingSystem.IsWindows())
            {
                mSteamPath = FindSteamLibraryWindows();
                return mSteamPath;
            }

            mSteamPath = FindSteamLibraryUnix();
            return mSteamPath;
        }

        return mSteamPath;
    }

    public List<SteamGame> LocateGames()
    {
        var games = new List<SteamGame>();
        var library = FindDefaultSteamLibrary();
        if (string.IsNullOrEmpty(library))
        {
            return games;
        }

        var folders = ValveDataFile.FromFile(Path.Combine(library, "steamapps", "libraryfolders.vdf"));

        foreach (var folder in folders)
        {
            var path = folder.GetCaseInsensitive("path").GetString();
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            var apps = folder.GetCaseInsensitive("apps");
            if (apps == null)
            {
                continue;
            }

            var libPath = Path.Combine(path, "steamapps");
            foreach (var app in apps)
            {
                var appid = app.Name;
                if (string.IsNullOrEmpty(appid))
                {
                    continue;
                }

                try
                {
                    var manifest = ValveDataFile.FromFile(Path.Combine(libPath, $"appmanifest_{appid}.acf"));
                    var name = manifest.GetCaseInsensitive("name").GetString();
                    var installDir = manifest.GetCaseInsensitive("installdir").GetString();
                    var root = Path.Combine(libPath, "common", installDir);
                    if (Directory.Exists(root))
                    {
                        games.Add(new SteamGame
                        {
                            ID = appid,
                            Name = name,
                            Root = root
                        });
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        return games;
    }
}