namespace HedgeModManager.CodeCompiler;
using System.Text;
using PreProcessor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

public class SyntaxTreeEx : CSharpSyntaxTree
{
    public TextProcessor PreProcessor { get; private init; } = null!;
    public List<BasicLexer.DirectiveSyntax> PreprocessorDirectives => PreProcessor.Directives;
    private readonly CSharpSyntaxTree mBaseSyntaxTree;

    private SyntaxTreeEx(CSharpSyntaxTree baseTree)
    {
        mBaseSyntaxTree = baseTree;
    }

    private static string ProcessText(string text, out TextProcessor preprocessor)
    {
        preprocessor = new TextProcessor();
        var body = new StringBuilder(preprocessor.Process(text));

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
        var tree = new SyntaxTreeEx(ParseText(ProcessText(text, out var processor), new CSharpParseOptions(kind: SourceCodeKind.Script)) as CSharpSyntaxTree)
        {
            PreProcessor = processor
        };
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