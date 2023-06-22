namespace HedgeModManager.Foundation;

public interface ICode
{
    public string ID { get; set; }
    public bool Enabled { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public CodeType Type { get; }
}