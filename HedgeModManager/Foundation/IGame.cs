namespace HedgeModManager.Foundation;

public interface IGame
{
    public string Platform { get; }
    public string ID { get; }
    public string Name { get; }
    public string Root { get; }
    public string? Executable { get; }
}