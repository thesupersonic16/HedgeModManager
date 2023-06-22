namespace HedgeModManager.Steam;
using Foundation;

public class SteamGame : IGame
{
    public string Platform => "Steam";
    public string ID { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Root { get; init; } = string.Empty;
    public string? Executable { get; init; }
}