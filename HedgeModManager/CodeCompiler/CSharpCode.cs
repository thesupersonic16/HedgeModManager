namespace HedgeModManager.CodeCompiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Text;
using Foundation;

public class CSharpCode : ICode
{
    private string mBody = string.Empty;
    private SyntaxTreeEx? mCachedSyntaxTree;

    public string ID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public CodeType Type { get; set; }

    public bool Enabled { get; set; }

    public List<BasicLexer.Token> Header { get; set; } = new List<BasicLexer.Token>();

    public string Body
    {
        get => mBody;
        set
        {
            mBody = value;
            mCachedSyntaxTree = null;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        {
            foreach (var token in Header)
            {
                sb.Append(token.Text);
                sb.Append(' ');
            }

            sb.AppendLine();
            sb.AppendLine(Body);
        }

        return sb.ToString();
    }

    public static List<CSharpCode> ParseFiles(params string[] paths)
    {
        var list = new List<CSharpCode>();

        foreach (var path in paths)
        {
            if (File.Exists(path))
                list.AddRange(ParseFile(path));
        }

        return list;
    }

    public static List<CSharpCode> ParseFile(string path)
    {
        using var stream = File.OpenRead(path);
        return Parse(stream);
    }

    public static unsafe List<CSharpCode> Parse(string text)
    {
        fixed (char* data = text)
        {
            using var reader = new StreamReader(new UnmanagedMemoryStream((byte*)data, text.Length * sizeof(char)), Encoding.Unicode);
            return Parse(reader);
        }
    }

    public static List<CSharpCode> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return Parse(reader);
    }

    public static List<CSharpCode> Parse(StreamReader reader)
    {
        var codes = new List<CSharpCode>();
        CSharpCode? currentCode = null;
        {
            bool isMultilineDescription = false;
            var lineBuilder = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line == null)
                    continue;

                var firstSpace = line.IndexOf(' ');
                if (firstSpace > 0)
                {
                    var type = line.AsSpan(0, firstSpace);

                    if (CodeTypeFromString(type, out var codeType))
                    {
                        // Parse description of the last code
                        if (currentCode != null)
                        {
                            if (isMultilineDescription)
                            {
                                currentCode.Description = DescriptionFromBody(currentCode);
                            }

                            currentCode.Body = lineBuilder.ToString();
                        }

                        lineBuilder.Clear();
                        isMultilineDescription = false;
                        currentCode = new CSharpCode
                        {
                            Type = codeType
                        };

                        codes.Add(currentCode);

                        var tokens = BasicLexer.ParseTokens(line.AsMemory(), x => !x.IsKind(SyntaxTokenKind.WhitespaceTrivia)).ToList();
                        currentCode.Header = tokens;
                        currentCode.Name = tokens[1].ValueOrText().ToString();

                        for (int i = 2; i < tokens.Count; i++)
                        {
                            var text = tokens[i].ValueOrText();

                            if (text.Span.Equals("in".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            {
                                i++;
                                currentCode.Category = tokens[i].ValueOrText().ToString();
                            }

                            if (text.Span.Equals("by".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            {
                                i++;
                                currentCode.Author = tokens[i].ValueOrText().ToString();
                            }

                            if (text.Span.Equals("does".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            {
                                i++;

                                /* If the line ends with "does" on its own,
                                   the description will be on the next line. */
                                if (i >= tokens.Count)
                                    isMultilineDescription = true;
                                else
                                    currentCode.Description = tokens[i].ValueOrText().ToString();
                            }
                        }

                        continue;
                    }
                }

                lineBuilder.AppendLine(line);
                // currentCode?.Lines.AppendLine(line);
            }

            // Parse the last one
            if (currentCode != null)
            {
                if (isMultilineDescription)
                {
                    currentCode.Description = DescriptionFromBody(currentCode);
                }

                currentCode.Body = lineBuilder.ToString();
            }
        }

        return codes;

        string DescriptionFromBody(CSharpCode code)
        {
            var body = code.Body;
            var offset = 0;
            var commentToken = BasicLexer.ParseToken(body.AsMemory(), offset);
            while (commentToken.IsKind(SyntaxTokenKind.WhitespaceTrivia) && !commentToken.IsKind(SyntaxTokenKind.EndOfFileToken))
            {
                offset += commentToken.Span.Length;
                commentToken = BasicLexer.ParseToken(body.AsMemory(), offset);
            }

            if (commentToken.IsKind(SyntaxTokenKind.SingleLineCommentTrivia) || commentToken.IsKind(SyntaxTokenKind.MultiLineCommentTrivia))
            {
                return commentToken.ValueOrText().ToString().Trim('\r', '\n', ' ');
            }

            return string.Empty;
        }
    }

    public List<string> GetReferences()
    {
        var references = new List<string>();

        var tree = ParseSyntaxTree();

        foreach (var reference in tree.PreprocessorDirectives.Where(x => x.Kind == SyntaxTokenKind.LibDirectiveTrivia))
        {
            references.Add(reference.Value.ToString());
        }

        return references;
    }

    public List<string> GetImports()
    {
        var imports = new List<string>();

        var tree = ParseSyntaxTree();

        foreach (var import in tree.PreprocessorDirectives.Where(x => x.Kind == SyntaxTokenKind.ImportDirectiveTrivia))
        {
            imports.Add(import.Value.ToString());
        }

        return imports;
    }

    public SyntaxTreeEx ParseSyntaxTree()
    {
        if (mCachedSyntaxTree == null)
        {
            mCachedSyntaxTree = SyntaxTreeEx.Parse(Body);
            return mCachedSyntaxTree;
        }

        return mCachedSyntaxTree;
    }

    public CompilationUnitSyntax CreateCompilationUnit()
    {
        var tree = ParseSyntaxTree();

        var unit = tree.GetCompilationUnitRoot();
        if (this.IsExecutable())
        {
            unit = (CompilationUnitSyntax)new OptionalColonRewriter().Visit(unit);
        }

        var libNamespace = string.Empty;
        var name = string.Empty;

        if (Type == CodeType.Library)
        {
            var dotIdx = Name.LastIndexOf('.');
            if (dotIdx > 0)
            {
                name = Name.Substring(dotIdx + 1);
                libNamespace = Name.Substring(0, dotIdx);
            }
            else
            {
                name = Name;
            }
        }
        else
        {
            name = Regex.Replace($"{Name}_{Guid.NewGuid()}", "[^a-z_0-9]", string.Empty, RegexOptions.IgnoreCase);
        }

        var classUnit = SyntaxFactory
            .ClassDeclaration(name)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.UnsafeKeyword)));

        if (this.IsExecutable())
        {
            var localMembers = new List<StatementSyntax>();
            var globalMembers = new List<MemberDeclarationSyntax>();

            foreach (var member in unit.Members)
            {
                if (member is GlobalStatementSyntax globalStatement)
                {
                    localMembers.Add(globalStatement.Statement);
                }
                else if (member is FieldDeclarationSyntax fieldDeclaration)
                {
                    if (!member.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        localMembers.Add(SyntaxFactory.LocalDeclarationStatement(member.Modifiers, fieldDeclaration.Declaration));
                    }
                    else
                    {
                        globalMembers.Add(member);
                    }
                }
                else
                {
                    globalMembers.Add(member);
                }
            }

            var funcUnit = SyntaxFactoryEx.MethodDeclaration(Type == CodeType.Patch ? "Init" : "OnFrame", "void",
                SyntaxFactory.Block(localMembers), "public");

            classUnit = classUnit
                .WithMembers(SyntaxFactory.List(globalMembers))
                .AddMembers(CodeProvider.LoaderExecutableMethod)
                .AddMembers(funcUnit);
        }
        else if (Type == CodeType.Library)
        {
            var filteredMembers = new List<MemberDeclarationSyntax>(unit.Members.Count);

            foreach (var member in unit.Members)
            {
                if (member is MethodDeclarationSyntax method)
                {
                    method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                    filteredMembers.Add(method);
                    continue;
                }

                if (member is FieldDeclarationSyntax field)
                {
                    field = field.WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
                    filteredMembers.Add(field);
                    continue;
                }

                if (member is PropertyDeclarationSyntax property)
                {
                    property = property.WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
                    filteredMembers.Add(property);
                    continue;
                }

                filteredMembers.Add(member);
            }

            classUnit = classUnit.WithMembers(SyntaxFactory.List(filteredMembers));
        }

        var compileUnit = SyntaxFactory.CompilationUnit()
            .AddMembers(MakeRootMember())
            .WithUsings(unit.Usings)
            .AddUsings(CodeProvider.PredefinedUsingDirectives);

        foreach (var reference in GetReferences())
        {
            var dotIndex = reference.LastIndexOf('.');
            if (dotIndex < 0)
                continue;

            var namespaceRef = reference.Substring(0, dotIndex);
            if (string.IsNullOrEmpty(namespaceRef))
            {
                continue;
            }
                
            compileUnit = compileUnit.AddUsings(MakeUsingDirective(namespaceRef));
        }

        foreach (var import in GetImports())
        {
            compileUnit = compileUnit.AddUsings(MakeUsingDirective(import, true));
        }

        return compileUnit;


        MemberDeclarationSyntax MakeRootMember()
        {
            if (string.IsNullOrEmpty(libNamespace))
            {
                return classUnit;
            }

            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(libNamespace)).AddMembers(classUnit);
        }

        UsingDirectiveSyntax MakeUsingDirective(string usingName, bool isStatic = false)
        {
            var names = usingName.Split('.');
            NameSyntax nameSyntax = SyntaxFactory.IdentifierName(names[0]);

            for (var i = 1; i < names.Length; i++)
            {
                if (string.IsNullOrEmpty(names[i]))
                    continue;

                nameSyntax = SyntaxFactory.QualifiedName(nameSyntax, SyntaxFactory.IdentifierName(names[i]));
            }

            if (isStatic)
            {
                return SyntaxFactory.UsingDirective(SyntaxFactory.Token(SyntaxKind.StaticKeyword), null,
                    nameSyntax);
            }

            return SyntaxFactory.UsingDirective(nameSyntax);
        }
    }

    public SyntaxTree CreateSyntaxTree()
    {
        return SyntaxFactory.SyntaxTree(CreateCompilationUnit());
    }

    public static bool CodeTypeFromString(ReadOnlySpan<char> text, out CodeType type)
    {
        if (text.Equals("Code".AsSpan(), StringComparison.Ordinal))
        {
            type = CodeType.Code;
            return true;
        }
        else if (text.Equals("Patch".AsSpan(), StringComparison.Ordinal))
        {
            type = CodeType.Patch;
            return true;
        }
        else if (text.Equals("Library".AsSpan(), StringComparison.Ordinal))
        {
            type = CodeType.Library;
            return true;
        }

        type = CodeType.Unknown;
        return false;
    }
}