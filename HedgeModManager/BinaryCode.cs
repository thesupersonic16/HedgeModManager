namespace HedgeModManager;
using Foundation;
using IO;

public class BinaryCode : ICode
{
    public string ID { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public CodeType Type => CodeType.Unknown;
    public string Path { get; set; }

    public BinaryCode(string path)
    {
        Path = path;
        Name = PathEx.GetFileName(path).ToString();
    }
}