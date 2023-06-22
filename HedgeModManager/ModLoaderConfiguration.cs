namespace HedgeModManager;
using Foundation;
using Text;

public class ModLoaderConfiguration : IModLoaderConfiguration
{
    public const string LegacySectionName = "CPKREDIR";

    public bool Enabled { get; set; } = true;
    public bool EnableSaveFileRedirection { get; set; } = true;
    public string DatabasePath { get; set; } = string.Empty;
    public string LogType { get; set; } = "none";

    public void Parse(Ini ini)
    {
        if (ini.TryGetValue(LegacySectionName, out var group))
        {
            Enabled = group.Get<int>("Enabled") != 0;
            EnableSaveFileRedirection = group.Get<int>("EnableSaveFileRedirection") != 0;
            
            DatabasePath = group.Get<string>("ModsDbIni");
            LogType = group.Get<string>("LogType");
        }
    }
}