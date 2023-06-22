namespace HedgeModManager.Foundation;

public interface IModAbleGame : IGame
{
    public IModLoaderConfiguration ModLoaderConfiguration { get; }
    public IModDatabase ModDatabase { get; }
    public string ModLoaderName { get; }
    
    public Task InitializeAsync();
    public Task<bool> InstallModLoaderAsync();
    public bool IsModLoaderInstalled();
}

public interface IModAbleGameTDatabase<out TDatabase> : IModAbleGame where TDatabase : IModDatabase
{
    public new TDatabase ModDatabase { get; }

    IModDatabase IModAbleGame.ModDatabase => ModDatabase;
}

public interface IModAbleGameTConfiguration<out TConfig> : IModAbleGame where TConfig : IModLoaderConfiguration
{
    public new TConfig ModLoaderConfiguration { get; }

    IModLoaderConfiguration IModAbleGame.ModLoaderConfiguration => ModLoaderConfiguration;
}