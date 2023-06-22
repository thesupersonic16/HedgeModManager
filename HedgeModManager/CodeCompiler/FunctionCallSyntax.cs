namespace HedgeModManager.CodeCompiler;

public class FunctionCallSyntax
{
    public BasicLexer.Token NameToken;
    public List<string> Arguments = new List<string>();
    public ReadOnlyMemory<char> WholeText { get; private set; }

    public FunctionCallSyntax()
    {

    }

    public FunctionCallSyntax(BasicLexer.Token nameToken)
    {
        NameToken = nameToken;
    }

    public void Parse(ReadOnlyMemory<char> text, int offset = 0)
    {
        var pos = offset;
        var token = BasicLexer.ParseToken(text, pos, true);

        pos = token.Span.End;
        if (token.IsKind(SyntaxTokenKind.IdentifierToken))
        {
            NameToken = token;
            token = BasicLexer.ParseToken(text, pos, true);
            pos = token.Span.End;

            if (token.IsKind(SyntaxTokenKind.OpenParenToken))
            {
                pos = token.Span.End;
                var argDepth = 1;

                var argStart = pos;
                while (!token.IsKind(SyntaxTokenKind.EndOfFileToken))
                {
                    token = BasicLexer.ParseToken(text, pos, true);
                    if (token.IsKind(SyntaxTokenKind.OpenParenToken))
                    {
                        argDepth++;
                    }
                    else if (token.IsKind(SyntaxTokenKind.CloseParenToken))
                    {
                        argDepth--;
                    }

                    if (argDepth <= 0)
                    {
                        var arg = text.Slice(argStart, pos - argStart).Trim();
                        if (arg.Length > 0)
                        {
                            Arguments.Add(arg.ToString());
                        }
                        break;
                    }

                    if (token.IsKind(SyntaxTokenKind.CommaToken) && argDepth == 1)
                    {
                        Arguments.Add(text.Slice(argStart, pos - argStart).Trim().ToString());
                        argStart = pos + 1;
                    }

                    token = BasicLexer.ParseToken(text, pos, true);
                    pos = token.Span.End;
                }
            }

            WholeText = text.Slice(offset, (pos - offset) + 1);
        }
    }

    public override string ToString()
    {
        if (Arguments.Count == 0)
        {
            return $"{NameToken}()";
        }
        return $"{NameToken}({string.Join(", ", Arguments)})";
    }
}