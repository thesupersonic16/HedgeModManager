namespace HedgeModManager.Foundation;

public interface IMod
{
    public bool Enabled { get; set; }
    public string ID { get; set; }
    public string Root { get; }
    public string UpdateServer { get; set; }
    public ModAttribute Attributes { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Date { get; set; }
    public List<ModAuthor> Authors { get; set; }
    public List<ModDependency> Dependencies { get; set; }
    public IReadOnlyList<ICode> Codes { get; }
}