namespace HedgeModManager.CodeCompiler.PreProcessor;

public interface IIncludeResolver
{
    public string? Resolve(string name);
}