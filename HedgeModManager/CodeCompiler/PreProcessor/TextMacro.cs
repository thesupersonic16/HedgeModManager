namespace HedgeModManager.CodeCompiler.PreProcessor;

public class TextMacro
{
    public List<string>? Args { get; set; }
    public string Value { get; set; }
    public bool IsFunction { get; set; }

    public TextMacro() : this(string.Empty)
    {

    }

    public TextMacro(string value)
    {
        Value = value;
    }

    public TextMacro(List<string> args, string value)
    {
        Args = args;
        Value = value;
        IsFunction = true;
    }

    public override string ToString()
    {
        return Value;
    }

    public static TextMacro FromText(ReadOnlyMemory<char> text, out string name)
    {
        var macro = new TextMacro();
        name = string.Empty;

        if (!BasicLexer.FunctionLike(text, ref name).IsKind(SyntaxTokenKind.None))
        {
            macro.IsFunction = true;
            macro.Args = SyntaxParser.ParseFunctionCall(text).Arguments;
        }
        else
        {
            name = text.ToString();
            macro.IsFunction = false;
        }

        return macro;
    }
}