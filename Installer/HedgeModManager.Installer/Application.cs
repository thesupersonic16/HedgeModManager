namespace HedgeModManager.Installer;
using System.Net.Http;
using System.Reflection;

public class Application
{
    public const string Publisher = "NeverFinishAnything";
    public const string Author = "thesupersonic16";
    public const string Name = "HedgeModManager";
    public const string UserAgent = "Mozilla/5.0 (compatible; HedgeModManager-Installer/0.1)";
    public static Application Current { get; set; }
    public string[] Arguments { get; set; }
    public HttpClient Http { get; } = new();

    public FileStorage Storage { get; }
    public FileStorage StartMenuStorage { get; }

    public Application()
    {
        Current = this;
        Storage = FileStorage.FromSpecialFolder(Environment.SpecialFolder.LocalApplicationData, Path.Combine(Publisher, Name));
        StartMenuStorage = FileStorage.FromSpecialFolder(Environment.SpecialFolder.StartMenu, Path.Combine("Programs", Name));
        
        Http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
    }

    public Application(string[] args) : this()
    {
        Arguments = args;
    }

    public async Task Run()
    {
        bool uninstallMode = false;
        for (int i = 0; i < Arguments.Length; i++)
        {
            var arg = Arguments[i];
            if (arg.Equals("--uninstall", StringComparison.OrdinalIgnoreCase))
            {
                uninstallMode = true;
                continue;
            }
        }

        var installer = new AppInstaller(Name, Author, Publisher, Storage);

        if (!uninstallMode)
            await installer.Install();
        else
            await installer.Uninstall();
    }
}