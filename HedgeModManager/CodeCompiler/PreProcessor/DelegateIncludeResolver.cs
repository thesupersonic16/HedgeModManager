namespace HedgeModManager.CodeCompiler.PreProcessor;

public class DelegateIncludeResolver : IIncludeResolver
{
    public Func<string, string> Resolver { get; set; }

    public DelegateIncludeResolver(Func<string, string> resolver)
    {
        Resolver = resolver;
    }

    public string Resolve(string name)
    {
        return Resolver(name);
    }

    public static implicit operator DelegateIncludeResolver(Func<string, string> resolver)
    {
        return new DelegateIncludeResolver(resolver);
    }
}