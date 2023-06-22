namespace HedgeModManager.CodeCompiler.PreProcessor;

public class DefaultIncludeResolver : IIncludeResolver
{
    public static readonly DefaultIncludeResolver Instance = new DefaultIncludeResolver();

    public string? Resolve(string name)
    {
        return !File.Exists(name) ? null : File.ReadAllText(name);
    }
}