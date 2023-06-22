namespace HedgeModManager;
using Foundation;
using Text;

public class ModAbleGameGeneric : IModAbleGameTDatabase<ModDatabaseGeneric>, IModAbleGameTConfiguration<ModLoaderConfiguration>
{
    public string Platform { get; }
    public string ID { get; }
    public string Name { get; }
    public string Root { get; init; }
    public string? Executable { get; init; }
    public string ModLoaderName { get; init; } = "Unknown";
    public string ModLoaderFileName { get; init; } = string.Empty;
    public ModDatabaseGeneric ModDatabase { get; } = new ModDatabaseGeneric();
    public ModLoaderConfiguration ModLoaderConfiguration { get; set; } = new ModLoaderConfiguration();

    public ModAbleGameGeneric(IGame game)
    {
        Platform = game.Platform;
        ID = game.ID;
        Name = game.Name;
        Root = game.Root;
        Executable = game.Executable;
    }

    public Task InitializeAsync()
    {
        try
        {
            // TODO: Change this
            ModLoaderConfiguration.Parse(Ini.FromFile(Path.Combine(Root, "cpkredir.ini")));
        }
        catch
        {
            ModLoaderConfiguration.DatabasePath = Path.Combine(Root, "Mods", ModDatabaseGeneric.DefaultDatabaseName);
        }

        ModDatabase.LoadDatabase(ModLoaderConfiguration.DatabasePath);
        return Task.CompletedTask;
    }

    public Task<bool> InstallModLoaderAsync()
    {
        return Task.FromResult(false);
    }

    public bool IsModLoaderInstalled()
    {
        if (string.IsNullOrEmpty(ModLoaderFileName))
        {
            return false;
        }

        return File.Exists(Path.Combine(Root, ModLoaderFileName));
    }
}