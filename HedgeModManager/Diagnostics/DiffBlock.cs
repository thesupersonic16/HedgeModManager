namespace HedgeModManager.Diagnostics;

public record DiffBlock
{
    public DiffType Type { get; set; }
    public string? Description { get; set; }
    public KeyValuePair<string, string> Data { get; set; }

    public DiffBlock(DiffType type, string? description)
    {
        Type = type;
        Description = description;
    }

    public DiffBlock(DiffType type, string? description, string dataKey)
    {
        Type = type;
        Description = description;
        Data = new KeyValuePair<string, string>(dataKey, string.Empty);
    }

    public DiffBlock(DiffType type, string? description, string dataKey, string dataValue)
    {
        Type = type;
        Description = description;
        Data = new KeyValuePair<string, string>(dataKey, dataValue);
    }
}