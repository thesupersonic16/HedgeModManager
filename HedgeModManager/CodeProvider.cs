using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using HedgeModManager.Misc;
using HedgeModManager.Properties;
using Markdig.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace HedgeModManager
{
    public class CodeProvider
    {
        public static string CodesTextPath => Path.Combine(App.ModsDbPath, "Codes.hmm");
        public static string ExtraCodesTextPath => Path.Combine(App.ModsDbPath, "ExtraCodes.hmm");

        public static string CompiledCodesPath => Path.Combine(App.ModsDbPath, "Codes.dll");

        public static MethodDeclarationSyntax LoaderExecutableMethod =
            SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)), "IsLoaderExecutable")
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

        public static UsingDirectiveSyntax[] PredefinedUsingDirectives =
        {
            SyntaxFactory.UsingDirective(
                SyntaxFactory.Token(SyntaxKind.UsingKeyword),
                SyntaxFactory.Token(SyntaxKind.None), null,
                SyntaxFactory.ParseName("System"), SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.Token(SyntaxKind.UsingKeyword),
                SyntaxFactory.Token(SyntaxKind.None), null,
                SyntaxFactory.ParseName("HMMCodes"), SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.Token(SyntaxKind.UsingKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword), null,
                SyntaxFactory.ParseName("HMMCodes.MemoryService"), SyntaxFactory.Token(SyntaxKind.SemicolonToken))
        };

        public static SyntaxTree[] PredefinedClasses =
        {
            CSharpSyntaxTree.ParseText(Resources.MemoryService),
            CSharpSyntaxTree.ParseText(Resources.Keys)
        };

        public static void CompileCodes(IEnumerable<CodeFile> sources, string assemblyPath, params string[] loadPaths)
        {
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);
            var trees = from source in sources select source.CreateSyntaxTree();

            var loads = GetLoadAssemblies(sources, loadPaths);

            loads.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            loads.Add(MetadataReference.CreateFromFile(typeof(Binder).Assembly.Location));
            loads.Add(MetadataReference.CreateFromFile(typeof(Component).Assembly.Location));
            loads.Add(MetadataReference.CreateFromFile(typeof(DynamicAttribute).Assembly.Location));

            var compiler = CSharpCompilation.Create("HMMCodes", trees, loads, options).AddSyntaxTrees(PredefinedClasses);

            using (var stream = File.Create(assemblyPath))
            {
                var result = compiler.Emit(stream);
                if (!result.Success)
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("Error compiling codes");
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        if(diagnostic.Severity == DiagnosticSeverity.Error)
                        {
                            builder.AppendLine(diagnostic.ToString());
                        }
                    }
                    throw new Exception(builder.ToString());
                }
            }
        }

        public static List<MetadataReference> GetLoadAssemblies(IEnumerable<CodeFile> sources, params string[] lookupPaths)
        {
            var meta = new List<MetadataReference>();

            var trees = from source in sources select source.ParseSyntaxTree();
            var basePath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var wpfPath = Path.Combine(basePath, "WPF");

            foreach (var tree in trees)
            {
                var unit = tree.GetCompilationUnitRoot();
                var loads = unit.GetLoadDirectives();
                foreach (var load in loads)
                {
                    var value = load.File.ValueText;
                    var path = Path.Combine(basePath, value);
                    if (File.Exists(path))
                    {
                        meta.Add(MetadataReference.CreateFromFile(path));
                        continue;
                    }

                    path = Path.Combine(wpfPath, value);
                    if (File.Exists(path))
                    {
                        meta.Add(MetadataReference.CreateFromFile(path));
                        continue;
                    }

                    foreach (var lookupPath in lookupPaths)
                    {
                        path = Path.Combine(lookupPath, value);
                        if (File.Exists(path))
                        {
                            meta.Add(MetadataReference.CreateFromFile(path));
                            break;
                        }
                    }
                }
            }
            
            return meta;
        }
    }

    public class CodeFile
    {
        public string Name { get; set; }
        
        public string Author { get; set; }

        public bool IsPatch { get; set; }

        public bool Enabled { get; set; }

        public StringBuilder Lines { get; set; } = new StringBuilder();

        protected SyntaxTree mCachedSyntaxTree;
        protected int mCachedHash;

        public static List<CodeFile> ParseFiles(params string[] paths)
        {
            var list = new List<CodeFile>();

            foreach (var path in paths)
            {
                if(File.Exists(path))
                    list.AddRange(ParseFile(path));
            }

            return list;
        }

        public static List<CodeFile> ParseFile(string path)
        {
            var codes = new List<CodeFile>();
            CodeFile currentCode = null;
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    
                    if(line == null)
                        continue;

                    bool isCode = line.StartsWith("Code");
                    bool isPatch = line.StartsWith("Patch");
                    if (isPatch || isCode)
                    {
                        currentCode = new CodeFile();
                        codes.Add(currentCode);

                        var matches = Regex.Matches(line, "(\"[^\"]*\"|[^\"]+)(\\s+|$)");
                        currentCode.IsPatch = isPatch;
                        var name = matches[1].Value.Trim(' ', '"');

                        currentCode.Name = name;

                        for (int i = 2; i < matches.Count; i++)
                        {
                            var match = matches[i].Value;

                            if (match.Trim().Equals("by", StringComparison.OrdinalIgnoreCase))
                            {
                                i++;
                                currentCode.Author = matches[i].Value.Trim(' ', '"');
                            }
                        }
                        continue;
                    }

                    currentCode?.Lines.AppendLine(line);
                }
            }

            return codes;
        }

        public SyntaxTree ParseSyntaxTree()
        {
            var hash = Lines.ToString().GetHashCode();

            if (hash != mCachedHash)
            {
                mCachedHash = hash;
                mCachedSyntaxTree = CSharpSyntaxTree.ParseText(Lines.ToString(),
                    new CSharpParseOptions(kind: SourceCodeKind.Script));
            }

            return mCachedSyntaxTree;
        }

        public CompilationUnitSyntax CreateCompilationUnit()
        {
            var tree = ParseSyntaxTree();

            var unit = tree.GetCompilationUnitRoot();
            unit = (CompilationUnitSyntax)new OptionalColonRewriter().Visit(unit);

            var allowedMembers = new List<StatementSyntax>();
            var disallowedMembers = new List<MemberDeclarationSyntax>();

            foreach (MemberDeclarationSyntax member in unit.Members)
            {
                if (member is GlobalStatementSyntax globalStatement)
                {
                    allowedMembers.Add(globalStatement.Statement);
                }
                else if (member is FieldDeclarationSyntax fieldDeclaration)
                {
                    if (!member.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        allowedMembers.Add(SyntaxFactory.LocalDeclarationStatement(member.Modifiers, fieldDeclaration.Declaration));
                    }
                    else
                    {
                        disallowedMembers.Add(member);
                    }
                }
                else
                {
                    disallowedMembers.Add(member);
                }
            }

            var funcUnit = SyntaxFactoryEx.MethodDeclaration(IsPatch ? "Init" : "OnFrame", "void",
                SyntaxFactory.Block(allowedMembers), "public");
            
            var classUnit = SyntaxFactory
                .ClassDeclaration(Regex.Replace(Name, "[^a-z]", string.Empty, RegexOptions.IgnoreCase))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.UnsafeKeyword)))
                .WithMembers(SyntaxFactory.List(disallowedMembers))
                .AddMembers(funcUnit)
                .AddMembers(CodeProvider.LoaderExecutableMethod);

            return SyntaxFactory.CompilationUnit()
                .AddMembers(classUnit)
                .WithUsings(unit.Usings)
                .AddUsings(CodeProvider.PredefinedUsingDirectives);
        }

        public SyntaxTree CreateSyntaxTree()
        {
            return SyntaxFactory.SyntaxTree(CreateCompilationUnit());
        }
    }

    public class OptionalColonRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxToken VisitToken(SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.SemicolonToken) && token.IsMissing)
            {
                return SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            }
            return base.VisitToken(token);
        }
    }
}
