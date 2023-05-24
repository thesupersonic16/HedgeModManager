using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HedgeModManager
{
    public class SyntaxTreeEx : CSharpSyntaxTree
    {
        public List<BasicLexer.DirectiveSyntax> PreprocessorDirectives { get; set; } = new();
        private CSharpSyntaxTree mBaseSyntaxTree;
        
        private SyntaxTreeEx(CSharpSyntaxTree baseTree)
        {
            mBaseSyntaxTree = baseTree;
        }

        private static string ProcessText(string text, out List<BasicLexer.DirectiveSyntax> directives)
        {
            directives = BasicLexer.ParseDirectives(text);
            var body = new StringBuilder(text);

            foreach (var directive in directives)
            {
                if (directive.Kind == SyntaxKind.LoadDirectiveTrivia)
                {
                    for (int i = 0; i < directive.FullSpan.Length; i++)
                    {
                        body[directive.FullSpan.Start + i] = ' ';
                    }
                }
            }

            var tokens = SyntaxFactory.ParseTokens(body.ToString(), options: new CSharpParseOptions(kind: SourceCodeKind.Script, documentationMode: DocumentationMode.Parse));
            using var enumerator = tokens.GetEnumerator();
            enumerator.MoveNext();

            var first = enumerator.Current;
            if (first.IsKind(SyntaxKind.OpenBraceToken))
            {
                var braces = new List<SyntaxToken>(32) { first };
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsKind(SyntaxKind.OpenBraceToken) ||
                        enumerator.Current.IsKind(SyntaxKind.CloseBraceToken))
                    {
                        braces.Add(enumerator.Current);
                    }
                }

                if (braces.Count == 0 || braces.Count % 2 != 0 || !braces.Last().IsKind(SyntaxKind.CloseBraceToken))
                {
                    return body.ToString();
                }

                return body.ToString(braces[0].Span.End, braces.Last().SpanStart - braces[0].Span.End);
            }

            return body.ToString();
        }

        public static SyntaxTreeEx Parse(string text)
        {
            var tree = new SyntaxTreeEx(ParseText(ProcessText(text, out var directives), new CSharpParseOptions(kind: SourceCodeKind.Script)) as CSharpSyntaxTree);
            tree.PreprocessorDirectives = directives;
            return tree;
        }

        public override bool TryGetText(out SourceText text)
        {
            return mBaseSyntaxTree.TryGetText(out text);
        }

        public override SourceText GetText(CancellationToken cancellationToken = new CancellationToken())
        {
            return mBaseSyntaxTree.GetText(cancellationToken);
        }

        public override CSharpSyntaxNode GetRoot(CancellationToken cancellationToken = new CancellationToken())
        {
            return mBaseSyntaxTree.GetRoot(cancellationToken);
        }

        public override bool TryGetRoot(out CSharpSyntaxNode root)
        {
            return mBaseSyntaxTree.TryGetRoot(out root);
        }

        public override SyntaxReference GetReference(SyntaxNode node)
        {
            return mBaseSyntaxTree.GetReference(node);
        }

        public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
        {
            return mBaseSyntaxTree.WithRootAndOptions(root, options);
        }

        public override SyntaxTree WithFilePath(string path)
        {
            return mBaseSyntaxTree.WithFilePath(path);
        }

        public override string FilePath => mBaseSyntaxTree.FilePath;

        public override bool HasCompilationUnitRoot => mBaseSyntaxTree.HasCompilationUnitRoot;

        public override CSharpParseOptions Options => mBaseSyntaxTree.Options;

        public override int Length => mBaseSyntaxTree.Length;

        public override Encoding Encoding => mBaseSyntaxTree.Encoding;
    }
}
