namespace HedgeModManager.CodeCompiler;
using System.Text;

public class SyntaxParser
{
    public static string EscapeString(string str)
    {
        var builder = new StringBuilder(str.Length);
        for (int i = 0; i < str.Length; i++)
        {
            var c = str[i];

            switch (c)
            {
                case '\n':
                    builder.Append("\\n");
                    break;

                case '\r':
                    builder.Append("\\r");
                    break;

                case '\t':
                    builder.Append("\\t");
                    break;

                case '\v':
                    builder.Append("\\v");
                    break;

                case '\b':
                    builder.Append("\\b");
                    break;

                case '\f':
                    builder.Append("\\f");
                    break;

                case '\a':
                    builder.Append("\\a");
                    break;

                case '\\':
                    builder.Append("\\\\");
                    break;

                case '"':
                    builder.Append("\\\"");
                    break;

                default:
                    builder.Append(c);
                    break;
            }
        }

        return builder.ToString();
    }

    public static FunctionCallSyntax ParseFunctionCall(ReadOnlyMemory<char> text, int offset = 0)
    {
        var syntax = new FunctionCallSyntax();
        syntax.Parse(text, offset);
        return syntax;
    }

    public static ReadOnlyMemory<char> ParseFunctionCallFast(ReadOnlyMemory<char> text, int offset)
    {
        var pos = offset;
        var token = BasicLexer.ParseToken(text, pos, true);

        var start = pos;
        pos = token.Span.End;
        if (token.IsKind(SyntaxTokenKind.IdentifierToken))
        {
            start = token.Span.Start;

            token = BasicLexer.ParseToken(text, pos, true);
            pos = token.Span.End;

            if (token.IsKind(SyntaxTokenKind.OpenParenToken))
            {
                pos = token.Span.End;
                var argDepth = 1;

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
                        return text.Slice(start, (pos - start) + 1);
                    }

                    pos = token.Span.End;
                }
            }
        }

        return ReadOnlyMemory<char>.Empty;
    }
}