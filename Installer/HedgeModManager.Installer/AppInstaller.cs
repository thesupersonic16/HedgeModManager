namespace HedgeModManager.Installer;
using System.Reflection;

public class AppInstaller : IInstaller
{
    public string Name { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public FileStorage Storage { get; set; }

    public AppInstaller(string name, string author, string publisher, FileStorage storage)
    {
        Name = name;
        Author = author;
        Publisher = publisher;
        Storage = storage;
    }

    public async Task Install()
    {
        var latest = await GitHubApi.GetLatestRelease(Author, Name);
        using var installerSourceStream = File.OpenRead(Assembly.GetExecutingAssembly().Location);

        var info = new ApplicationInfo(Name, true)
        {
            DisplayName = Name,
            DisplayIcon = Storage.GetFullPath($"{Name}.exe"),
            Publisher = Publisher,
            UninstallAction = $"\"{Storage.GetFullPath($"{Name}.Installer.exe")}\" --uninstall"
        };
        info.SetParameter("Directory", Storage.BasePath);

        Console.WriteLine($"Downloading {Name}.");
        using var sourceStream = await Application.Current.Http.GetStreamAsync(latest.GetAsset($"{Name}.exe").BrowserDownloadUrl);
        using var destStream = Storage.Create($"{Name}.exe");
        await sourceStream.CopyToAsync(destStream, 20048);
        Console.WriteLine("Finished downloading.");

        using var destInstallerStream = Storage.Create($"{Name}.Installer.exe");

        await installerSourceStream.CopyToAsync(destInstallerStream, 20048);
        CreateStartMenuShortcut(Name, $"{Name}.exe");
        info.Save();
    }

    public Task Uninstall()
    {
        Directory.Delete(Application.Current.StartMenuStorage.BasePath, true);
        ApplicationInfo.Delete(Name, true);
        Storage.Delete($"{Name}.exe");

        return Task.CompletedTask;
    }

    public void CreateStartMenuShortcut(string name, string destName)
    {
        Utilities.CreateShortcut(Application.Current.StartMenuStorage.GetFullPath($"{name}.lnk"), Storage.GetFullPath(destName));
    }
}