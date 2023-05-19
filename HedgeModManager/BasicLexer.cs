using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using static System.Net.Mime.MediaTypeNames;

namespace HedgeModManager
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
                        if (directive.Name is "load")
                        {
                            directive.Kind = SyntaxKind.LoadDirectiveTrivia;
                        }
                        else if (directive.Name is "lib")
                        {
                            directive.Kind = SyntaxKind.ReferenceDirectiveTrivia;
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
                        directive.FullSpan = new TextSpan(directive.FullSpan.Start, (offset + token.Span.Length) - directive.FullSpan.Start);
                        directive.Value = string.IsNullOrEmpty(token.ValueText) ? token.Text : token.ValueText;
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

                return Token.Create(text, SyntaxKind.WhitespaceTrivia, 0, l);
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
                while (offset + l < text.Length && (text[offset + l] != '"' && text[offset + l - 1] != '\\'))
                    l++;

                return Token.Create(text, SyntaxKind.StringLiteralToken, offset, l + 1, 1, l - 1);
            }

            switch (c)
            {
                case '#':
                    return Token.Create(text, SyntaxKind.HashToken, 0, 1);

                default:
                    break;
            }

            return Token.Create(text, SyntaxKind.None, 0, 1);
        }

        public struct DirectiveSyntax
        {
            public string Name = string.Empty;
            public string Value = string.Empty;
            public SyntaxKind Kind;
            public TextSpan FullSpan;

            public DirectiveSyntax()
            {
            }
        }

        public struct Token
        {
            public string Text;
            public TextSpan Span;
            public TextSpan ValueSpan;
            public SyntaxKind Kind;

            public string ValueText => ValueSpan.Length > 0 ? Text.Substring(ValueSpan.Start, ValueSpan.Length) : string.Empty;

            public bool HasNewLine => Text.Contains('\n') || Text.Contains('\r');

            public static Token Create(string text, SyntaxKind kind, int o, int l)
            {
                return new Token()
                {
                    Text = o + l <= text.Length ? text.Substring(o, l) : string.Empty,
                    Span = new TextSpan(o, l),
                    Kind = kind
                };
            }

            public static Token Create(string text, SyntaxKind kind, int o, int l, int vo, int vl)
            {
                return new Token()
                {
                    Text = o + l <= text.Length ? text.Substring(o, l) : string.Empty,
                    Span = new TextSpan(o, l),
                    ValueSpan = new TextSpan(vo, vl),
                    Kind = kind,
                };
            }
        }
    }
}
