using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HedgeModManager.CodeCompiler
{
    public class BasicLexer
    {
        private static bool IsAlphabet(char c)
        {
            return c is >= 'a' and <= 'z' || c is >= 'A' and <= 'Z';
        }

        private static bool IsWhitespace(char c)
        {
            return c is ' ' or '\t' or '\r' or '\n';
        }

        public static List<DirectiveSyntax> ParseDirectives(string text)
        {
            var directives = new List<DirectiveSyntax>();
            int offset = 0;
            int parserState = 0;
            int start = 0;

            while (offset < text.Length)
            {
                var token = ParseToken(text, offset);
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                if (parserState == 0)
                {
                    if (token.Kind == SyntaxKind.HashToken)
                    {
                        start = offset;
                        directives.Add(new DirectiveSyntax());
                        parserState = 1;
                    }
                }
                else if (parserState == 1)
                {
                    if (token.Kind == SyntaxKind.IdentifierToken)
                    {
                        var directive = new DirectiveSyntax { FullSpan = new TextSpan(start, 1), Name = token.Text };
                        if (directive.Name.Span.Equals("load".AsSpan(), StringComparison.Ordinal))
                        {
                            directive.Kind = SyntaxKind.LoadDirectiveTrivia;
                        }
                        else if (directive.Name.Span.Equals("lib".AsSpan(), StringComparison.Ordinal))
                        {
                            directive.Kind = SyntaxKind.ReferenceDirectiveTrivia;
                        }
                        else if (directive.Name.Span.Equals("import".AsSpan(), StringComparison.Ordinal))
                        {
                            directive.Kind = SyntaxKind.UsingDirective;
                        }

                        directives[directives.Count - 1] = directive;
                        parserState = 2;
                    }
                    else if (token.Kind == SyntaxKind.WhitespaceTrivia && token.HasNewLine)
                    {
                        directives.RemoveAt(directives.Count - 1);
                        parserState = 0;
                    }
                }
                else if (parserState == 2)
                {
                    if (token.Kind != SyntaxKind.WhitespaceTrivia)
                    {
                        var directive = directives[directives.Count - 1];
                        directive.FullSpan = new TextSpan(directive.FullSpan.Start,
                            offset + token.Span.Length - directive.FullSpan.Start);
                        directive.Value = token.ValueOrText();
                        directives[directives.Count - 1] = directive;

                        parserState = 0;
                    }
                    else if (token.Kind == SyntaxKind.WhitespaceTrivia && token.HasNewLine)
                    {
                        parserState = 0;
                    }
                }

                offset += token.Span.Length;
            }

            if (parserState != 0)
            {
                directives.RemoveAt(directives.Count - 1);
            }

            return directives;
        }

        public static Token ParseToken(string text, int offset = 0)
        {
            if (offset >= text.Length)
                return Token.Create(text, SyntaxKind.EndOfFileToken, 0, 0);

            char c = text[offset];

            if (IsWhitespace(c))
            {
                int l = 0;
                while (offset + l < text.Length && IsWhitespace(text[offset + l]))
                    l++;

                return Token.Create(text, SyntaxKind.WhitespaceTrivia, offset, l);
            }

            if (CharEquals('/') && CharEquals('/', 1))
            {
                int l = 2;
                while (offset + l < text.Length && text[offset + l] != '\r' && text[offset + l] != '\n')
                {
                    l++;
                }

                return Token.Create(text, SyntaxKind.SingleLineCommentTrivia, offset, l, 2, l - 2);
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

                return Token.Create(text, SyntaxKind.MultiLineCommentTrivia, offset, l, 2, valueEnd);
            }

            if (IsAlphabet(c))
            {
                int l = 0;
                while (offset + l < text.Length && IsAlphabet(text[offset + l]))
                    l++;

                return Token.Create(text, SyntaxKind.IdentifierToken, offset, l);
            }
            else if (c == '"')
            {
                int l = 1;
                while (offset + l < text.Length && text[offset + l] != '"' && text[offset + l - 1] != '\\')
                    l++;

                return Token.Create(text, SyntaxKind.StringLiteralToken, offset, l + 1, 1, l - 1);
            }

            switch (c)
            {
                case '#':
                    return Token.Create(text, SyntaxKind.HashToken, 0, 1);

                case '{':
                    return Token.Create(text, SyntaxKind.OpenBraceToken, 0, 1);

                case '}':
                    return Token.Create(text, SyntaxKind.CloseBraceToken, 0, 1);

                case '(':
                    return Token.Create(text, SyntaxKind.OpenParenToken, 0, 1);

                case ')':
                    return Token.Create(text, SyntaxKind.CloseParenToken, 0, 1);

                case '[':
                    return Token.Create(text, SyntaxKind.OpenBracketToken, 0, 1);

                case ']':
                    return Token.Create(text, SyntaxKind.CloseBracketToken, 0, 1);

                case ',':
                    return Token.Create(text, SyntaxKind.CommaToken, 0, 1);

                case ';':
                    return Token.Create(text, SyntaxKind.SemicolonToken, 0, 1);

                case '+':
                    return Token.Create(text, SyntaxKind.PlusToken, 0, 1);

                case '-':
                    return Token.Create(text, SyntaxKind.MinusToken, 0, 1);

                case '*':
                    return Token.Create(text, SyntaxKind.AsteriskToken, 0, 1);

                case '/':
                    return Token.Create(text, SyntaxKind.SlashToken, 0, 1);

                case '%':
                    return Token.Create(text, SyntaxKind.PercentToken, 0, 1);

                case '^':
                    return Token.Create(text, SyntaxKind.CaretToken, 0, 1);

                case '~':
                    return Token.Create(text, SyntaxKind.TildeToken, 0, 1);

                case '?':
                    return Token.Create(text, SyntaxKind.QuestionToken, 0, 1);

                case ':':
                    return Token.Create(text, SyntaxKind.ColonToken, 0, 1);

                case '&':
                    return Token.Create(text, SyntaxKind.AmpersandToken, 0, 1);

                case '|':
                    return Token.Create(text, SyntaxKind.BarToken, 0, 1);

                case '=':
                    return Token.Create(text, SyntaxKind.EqualsToken, 0, 1);

                case '<':
                    return Token.Create(text, SyntaxKind.LessThanToken, 0, 1);

                case '>':
                    return Token.Create(text, SyntaxKind.GreaterThanToken, 0, 1);

                case '!':
                    return Token.Create(text, SyntaxKind.ExclamationToken, 0, 1);

                case '.':
                    return Token.Create(text, SyntaxKind.DotToken, 0, 1);
            }

            return Token.Create(text, SyntaxKind.None, offset, 1);

            bool CharEquals(char c, int o = 0)
            {
                return offset + o < text.Length && text[offset + o] == c;
            }
        }

        public static IEnumerable<Token> ParseTokens(string text) => ParseTokens(text, _ => true);

        public static IEnumerable<Token> ParseTokens(string text, Predicate<Token> filter)
        {
            int offset = 0;
            while (offset < text.Length)
            {
                var token = ParseToken(text, offset);
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                if (filter(token))
                    yield return token;

                offset += token.Span.Length;
            }
        }

        public struct DirectiveSyntax
        {
            public ReadOnlyMemory<char> Name = ReadOnlyMemory<char>.Empty;
            public ReadOnlyMemory<char> Value = ReadOnlyMemory<char>.Empty;
            public SyntaxKind Kind = new();
            public TextSpan FullSpan = new();

            public DirectiveSyntax()
            {
            }
        }

        public struct Token
        {
            public ReadOnlyMemory<char> Text;
            public TextSpan Span;
            public TextSpan ValueSpan;
            public SyntaxKind Kind;

            public ReadOnlyMemory<char> ValueText => ValueSpan.Length > 0
                ? Text.Slice(ValueSpan.Start, ValueSpan.Length)
                : Memory<char>.Empty;

            public bool HasNewLine => Text.Span.IndexOf('\n') >= 0 || Text.Span.IndexOf('\r') >= 0;

            public static Token Create(string text, SyntaxKind kind, int o, int l)
            {
                return new Token()
                {
                    Text = o + l <= text.Length ? text.AsMemory(o, l) : Memory<char>.Empty,
                    Span = new TextSpan(o, l),
                    Kind = kind
                };
            }

            public static Token Create(string text, SyntaxKind kind, int o, int l, int vo, int vl)
            {
                return new Token()
                {
                    Text = o + l <= text.Length ? text.AsMemory(o, l) : Memory<char>.Empty,
                    Span = new TextSpan(o, l),
                    ValueSpan = new TextSpan(vo, vl),
                    Kind = kind,
                };
            }

            public bool IsKind(SyntaxKind kind)
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
}