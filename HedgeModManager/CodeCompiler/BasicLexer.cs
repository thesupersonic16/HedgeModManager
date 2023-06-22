namespace HedgeModManager.CodeCompiler;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

public class BasicLexer
{
    private static bool IsIdentifier(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }

    public static bool IsNumeric(char c)
    {
        return c is >= '0' and <= '9';
    }

    private static bool IsWhitespace(char c)
    {
        return c is ' ' or '\t' or '\r' or '\n';
    }

    public static Token ParseToken(ReadOnlyMemory<char> text, int offset = 0, bool ignoreWhitespace = false)
    {
        if (offset >= text.Length)
            return Token.Create(text, SyntaxTokenKind.EndOfFileToken, 0, 0);

        var span = text.Span;
        char c = span[offset];

        if (IsWhitespace(c))
        {
            int l = 0;
            while (offset + l < text.Length && IsWhitespace(span[offset + l]))
            {
                l++;
            }

            if (!ignoreWhitespace)
            {
                return Token.Create(text, SyntaxTokenKind.WhitespaceTrivia, offset, l);
            }

            offset += l;
            if (offset >= text.Length)
            {
                return Token.Create(text, SyntaxTokenKind.EndOfFileToken, 0, 0);
            }

            c = span[offset];
        }

        if (CharEquals('/') && CharEquals('/', 1))
        {
            int l = 2;
            while (offset + l < text.Length && span[offset + l] != '\r' && span[offset + l] != '\n')
            {
                l++;
            }

            return Token.Create(text, SyntaxTokenKind.SingleLineCommentTrivia, offset, l, 2, l - 2);
        }
        else if (CharEquals('/') && CharEquals('*', 1))
        {
            var l = 2;

            while (offset + l < text.Length && (!CharEquals('*', l) || !CharEquals('/', l + 1)))
            {
                l++;
            }

            var valueEnd = l - 2;
            if (CharEquals('*', l) && CharEquals('/', l + 1))
            {
                l += 2;
            }

            return Token.Create(text, SyntaxTokenKind.MultiLineCommentTrivia, offset, l, 2, valueEnd);
        }

        if (IsIdentifier(c))
        {
            int l = 1;
            while (offset + l < text.Length && (IsIdentifier(span[offset + l]) || IsNumeric(span[offset + l])))
            {
                l++;
            }

            return Token.Create(text, SyntaxTokenKind.IdentifierToken, offset, l);
        }
        else if (IsNumeric(c))
        {
            int l = 1;
            while (offset + l < text.Length && IsNumeric(span[offset + l]))
            {
                l++;
            }

            return Token.Create(text, SyntaxTokenKind.NumericLiteralToken, offset, l);
        }
        else if (c == '"')
        {
            int l = 1;
            int vl = 0;
            while (offset + l < text.Length)
            {
                if (CharEquals('"', l) && !CharEquals('\\', l - 1))
                {
                    l++;
                    break;
                }

                vl++;
                l++;
            }

            return Token.Create(text, SyntaxTokenKind.StringLiteralToken, offset, l, 1, vl);
        }

        switch (c)
        {
            case '#':
                return Token.Create(text, SyntaxTokenKind.HashToken, offset, 1);

            case '{':
                return Token.Create(text, SyntaxTokenKind.OpenBraceToken, offset, 1);

            case '}':
                return Token.Create(text, SyntaxTokenKind.CloseBraceToken, offset, 1);

            case '(':
                return Token.Create(text, SyntaxTokenKind.OpenParenToken, offset, 1);

            case ')':
                return Token.Create(text, SyntaxTokenKind.CloseParenToken, offset, 1);

            case '[':
                return Token.Create(text, SyntaxTokenKind.OpenBracketToken, offset, 1);

            case ']':
                return Token.Create(text, SyntaxTokenKind.CloseBracketToken, offset, 1);

            case ',':
                return Token.Create(text, SyntaxTokenKind.CommaToken, offset, 1);

            case ';':
                return Token.Create(text, SyntaxTokenKind.SemicolonToken, offset, 1);

            case '+':
                return Token.Create(text, SyntaxTokenKind.PlusToken, offset, 1);

            case '-':
                return Token.Create(text, SyntaxTokenKind.MinusToken, offset, 1);

            case '*':
                return Token.Create(text, SyntaxTokenKind.AsteriskToken, offset, 1);

            case '/':
                return Token.Create(text, SyntaxTokenKind.SlashToken, offset, 1);

            case '%':
                return Token.Create(text, SyntaxTokenKind.PercentToken, offset, 1);

            case '^':
                return Token.Create(text, SyntaxTokenKind.CaretToken, offset, 1);

            case '~':
                return Token.Create(text, SyntaxTokenKind.TildeToken, offset, 1);

            case '?':
                return Token.Create(text, SyntaxTokenKind.QuestionToken, offset, 1);

            case ':':
                return Token.Create(text, SyntaxTokenKind.ColonToken, offset, 1);

            case '&':
                return Token.Create(text, SyntaxTokenKind.AmpersandToken, offset, 1);

            case '|':
                return Token.Create(text, SyntaxTokenKind.BarToken, offset, 1);

            case '=':
            {
                if (CharEquals('=', 1))
                {
                    return Token.Create(text, SyntaxTokenKind.EqualsEqualsToken, offset, 2);
                }

                return Token.Create(text, SyntaxTokenKind.EqualsToken, offset, 1);
            }

            case '<':
            {
                if (CharEquals('=', 1))
                {
                    return Token.Create(text, SyntaxTokenKind.LessEqualsToken, offset, 2);
                }

                return Token.Create(text, SyntaxTokenKind.LessThanToken, offset, 1);
            }

            case '>':
            {
                if (CharEquals('=', 1))
                {
                    return Token.Create(text, SyntaxTokenKind.GreaterEqualsToken, offset, 2);
                }

                return Token.Create(text, SyntaxTokenKind.GreaterThanToken, offset, 1);
            }

            case '!':
                return Token.Create(text, SyntaxTokenKind.ExclamationToken, offset, 1);

            case '.':
                return Token.Create(text, SyntaxTokenKind.DotToken, offset, 1);
        }

        return Token.Create(text, SyntaxTokenKind.None, offset, 1);

        bool CharEquals(char c, int o = 0)
        {
            return offset + o < text.Length && text.Span[offset + o] == c;
        }
    }

    public static Token FunctionLike(ReadOnlyMemory<char> text, ref string name, int offset = 0)
    {
        var pos = offset;
        var token = ParseToken(text, pos, true);
        pos += token.Span.Length;

        if (token.IsKind(SyntaxTokenKind.IdentifierToken))
        {
            var parenToken = ParseToken(text, pos, true);
            pos += parenToken.Span.Length;

            if (parenToken.IsKind(SyntaxTokenKind.OpenParenToken))
            {
                if (!Unsafe.IsNullRef(ref name))
                {
                    name = token.Text.ToString();
                }

                return token;
            }
        }

        return default;
    }

    public static IEnumerable<Token> ParseTokens(string text) => ParseTokens(text.AsMemory(), _ => true);

    public static IEnumerable<Token> ParseTokens(ReadOnlyMemory<char> text, Predicate<Token> filter)
    {
        int offset = 0;
        while (offset < text.Length)
        {
            var token = ParseToken(text, offset);
            if (token.Kind == SyntaxTokenKind.EndOfFileToken)
                break;

            if (filter(token))
                yield return token;

            offset += token.Span.Length;
        }
    }

    public static string FilterComments(ReadOnlyMemory<char> text)
    {
        var sb = new StringBuilder();
        foreach (var token in ParseTokens(text,
                     t => t.Kind != SyntaxTokenKind.SingleLineCommentTrivia &&
                          t.Kind != SyntaxTokenKind.MultiLineCommentTrivia))
        {
            sb.Append(token.Text);
        }

        return sb.ToString();
    }

    public struct DirectiveSyntax
    {
        public ReadOnlyMemory<char> Name = ReadOnlyMemory<char>.Empty;
        public ReadOnlyMemory<char> Value = ReadOnlyMemory<char>.Empty;
        public SyntaxTokenKind Kind = new();
        public TextSpan FullSpan = new();

        public DirectiveSyntax(ReadOnlyMemory<char> name)
        {
            Name = name;

            var span = name.Span;
            if (span.Equals("load".AsSpan(), StringComparison.Ordinal))
            {
                Kind = SyntaxTokenKind.LoadDirectiveTrivia;
            }
            else if (span.Equals("lib".AsSpan(), StringComparison.Ordinal))
            {
                Kind = SyntaxTokenKind.LibDirectiveTrivia;
            }
            else if (span.Equals("import".AsSpan(), StringComparison.Ordinal))
            {
                Kind = SyntaxTokenKind.ImportDirectiveTrivia;
            }
        }
    }

    public struct Token
    {
        public ReadOnlyMemory<char> Text;
        public TextSpan Span;
        public TextSpan ValueSpan;
        public SyntaxTokenKind Kind;

        public ReadOnlyMemory<char> ValueText => ValueSpan.Length > 0
            ? Text.Slice(ValueSpan.Start, ValueSpan.Length)
            : Memory<char>.Empty;

        public bool HasNewLine => Text.Span.IndexOf('\n') >= 0 || Text.Span.IndexOf('\r') >= 0;

        public static Token Create(ReadOnlyMemory<char> text, SyntaxTokenKind kind, int o, int l)
        {
            return new Token()
            {
                Text = o + l <= text.Length ? text.Slice(o, l) : Memory<char>.Empty,
                Span = new TextSpan(o, l),
                Kind = kind
            };
        }

        public static Token Create(ReadOnlyMemory<char> text, SyntaxTokenKind kind, int o, int l, int vo, int vl)
        {
            return new Token()
            {
                Text = o + l <= text.Length ? text.Slice(o, l) : Memory<char>.Empty,
                Span = new TextSpan(o, l),
                ValueSpan = new TextSpan(vo, vl),
                Kind = kind,
            };
        }

        public bool IsTrivia()
        {
            return Kind is > SyntaxTokenKind.BeginTriviaCount and < SyntaxTokenKind.EndTriviaCount;
        }

        public bool IsKind(SyntaxTokenKind kind)
        {
            return Kind == kind;
        }

        public ReadOnlyMemory<char> ValueOrText()
        {
            return ValueSpan.Length > 0 ? ValueText : Text;
        }

        public override string ToString()
        {
            return Text.ToString();
        }
    }
}