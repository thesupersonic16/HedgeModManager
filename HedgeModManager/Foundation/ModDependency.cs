namespace HedgeModManager.Foundation;

public class ModDependency
{
    public string ID { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public IMod? Mod { get; set; }
}