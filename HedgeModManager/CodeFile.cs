using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HedgeModManager.Misc;
using Markdig.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace HedgeModManager
{
    public class CodeFile
    {
        public const string TagPrefix = "!!";
        public const string VersionTag = "VERSION";
        protected Version mFileVersion;

        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public List<Code> Codes { get; set; } = new List<Code>();
        public IEnumerable<Code> ExecutableCodes => Codes.Where(x => x.IsExecutable());

        public CodeFile() { }

        public CodeFile(string codeFilePath)
        {
            ParseFile(codeFilePath);
        }

        public Version FileVersion
        {
            get
            {
                if (mFileVersion == null)
                {
                    Tags.TryGetValue(VersionTag, out string v);
                    mFileVersion = string.IsNullOrEmpty(v) ? new Version(0, 0) : Version.Parse(v);
                }

                return mFileVersion;
            }
            set
            {
                mFileVersion = value;
                Tags[VersionTag] = mFileVersion.ToString();
            }
        }

        public List<CodeDiffResult> Diff(CodeFile old)
        {
            var diff = new List<CodeDiffResult>();
            var addedCodes = new List<string>();

            foreach (var code in Codes)
            {
                // Added
                if (!old.Codes.Where(x => x.Name == code.Name).Any())
                {
                    addedCodes.Add(code.Name);
                    continue;
                }
            }

            foreach (var code in old.Codes)
            {
                // Modified
                if (Codes.Where(x => x.Name == code.Name).SingleOrDefault() is Code modified)
                {
                    if (code.Lines.ToString() != modified.Lines.ToString())
                    {
                        diff.Add(new CodeDiffResult(code.Name, CodeDiffResult.CodeDiffType.Modified));
                        continue;
                    }
                }

                // Renamed
                if (Codes.Where(x => x.Lines.ToString() == code.Lines.ToString()).SingleOrDefault() is Code renamed)
                {
                    if (code.Name != renamed.Name)
                    {
                        diff.Add(new CodeDiffResult($"{code.Name} -> {renamed.Name}", CodeDiffResult.CodeDiffType.Renamed, code.Name, renamed.Name));

                        /* Remove this code from the add list
                           so we don't display it twice. */
                        if (addedCodes.Contains(renamed.Name))
                            addedCodes.Remove(renamed.Name);

                        continue;
                    }
                }

                // Removed
                if (!Codes.Where(x => x.Name == code.Name).Any())
                {
                    diff.Add(new CodeDiffResult(code.Name, CodeDiffResult.CodeDiffType.Removed));
                    continue;
                }
            }

            foreach (string code in addedCodes)
                diff.Add(new CodeDiffResult(code, CodeDiffResult.CodeDiffType.Added));

            return diff.OrderBy(x => x.Type).ToList();
        }

        public void ParseFile(string path)
        {
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    ParseFile(stream);
                }
            }
        }

        public void ParseFile(Stream stream)
        {
            var start = stream.Position;
            using (var reader = new StreamReader(stream))
            {
                var line = reader.ReadLine();

                while (line != null && line.StartsWith(TagPrefix))
                {
                    var tagName = string.Empty;
                    var tagValue = string.Empty;

                    var separatorIndex = line.IndexOf(' ');
                    if (separatorIndex < 0)
                    {
                        tagName = line.Substring(TagPrefix.Length);
                    }
                    else
                    {
                        tagName = line.Substring(TagPrefix.Length, separatorIndex - 2);
                        tagValue = line.Substring(separatorIndex + 1).Trim();
                    }

                    if (!Tags.ContainsKey(tagName))
                        Tags.Add(tagName, tagValue);
                    else
                        Tags[tagName] = tagValue;

                    line = reader.ReadLine();
                }

                stream.Position = start;
                reader.DiscardBufferedData();
                Codes.AddRange(Code.ParseFile(reader));
            }
        }

        public static CodeFile FromFile(string path)
        {
            var file = new CodeFile();
            file.ParseFile(path);
            return file;
        }

        public static CodeFile FromFiles(params string[] paths)
        {
            var file = new CodeFile();
            foreach (var path in paths)
            {
                file.ParseFile(path);
            }

            return file;
        }
    }

    public class CodeDiffResult
    {
        /// <summary>
        /// The details about this change.
        /// </summary>
        public string Changelog { get; set; }

        /// <summary>
        /// The change made to this code.
        /// </summary>
        public CodeDiffType Type { get; set; }

        /// <summary>
        /// The original name of a code before renaming.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// The new name of a code after renaming.
        /// </summary>
        public string NewName { get; set; }

        public CodeDiffResult(string changelog, CodeDiffType type)
        {
            Changelog = changelog;
            Type = type;
        }

        public CodeDiffResult(string changelog, CodeDiffType type, string originalName, string newName)
        {
            Changelog = changelog;
            Type = type;
            OriginalName = originalName;
            NewName = newName;
        }

        public override string ToString()
        {
            string key = Type switch
            {
                CodeDiffType.Added => "DiffUIAdded",
                CodeDiffType.Removed => "DiffUIRemoved",
                CodeDiffType.Renamed => "DiffUIRenamed",
                _ => "DiffUIModified",
            };

            return Lang.LocaliseFormat(key, Changelog);
        }

        public enum CodeDiffType
        {
            Added,
            Modified,
            Removed,
            Renamed
        }
    }

    public enum CodeType
    {
        Code, Patch, Library, Unknown
    }

    public class Code : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public CodeType Type { get; set; }

        public bool Enabled { get; set; }

        public StringBuilder Header { get; set; } = new StringBuilder();
        public StringBuilder Lines { get; set; } = new StringBuilder();

        protected SyntaxTree mCachedSyntaxTree;
        protected int mCachedHash;

        public override string ToString()
        {
            var sb = new StringBuilder(Header.ToString());
            {
                sb.AppendLine(Lines.ToString());
            }

            return sb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static List<Code> ParseFiles(params string[] paths)
        {
            var list = new List<Code>();

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    list.AddRange(ParseFile(path));
            }

            return list;
        }

        public static List<Code> ParseFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return ParseFile(stream);
            }
        }

        public static List<Code> ParseFile(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return ParseFile(reader);
            }
        }

        public static List<Code> ParseFile(StreamReader reader)
        {
            var codes = new List<Code>();
            Code currentCode = null;
            {
                bool isMultilineDescription = false;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                        continue;

                    var firstSpace = line.IndexOf(' ');
                    if (firstSpace > 0)
                    {
                        var type = line.Substring(0, firstSpace);

                        if (CodeTypeFromString(type, out var codeType))
                        {
                            currentCode = new Code();
                            currentCode.Type = codeType;
                            codes.Add(currentCode);

                            currentCode.Header.AppendLine(line);

                            var matches = Regex.Matches(line, "(\"[^\"]*\"|[^\"]+)(\\s+|$)");
                            var name = matches[1].Value.Trim(' ', '"');

                            currentCode.Name = name;

                            for (int i = 2; i < matches.Count; i++)
                            {
                                var match = matches[i].Value;

                                if (match.Trim().Equals("in", StringComparison.OrdinalIgnoreCase))
                                {
                                    i++;
                                    currentCode.Category = matches[i].Value.Trim(' ', '"');
                                }

                                if (match.Trim().Equals("by", StringComparison.OrdinalIgnoreCase))
                                {
                                    i++;
                                    currentCode.Author = matches[i].Value.Trim(' ', '"');
                                }

                                if (match.Trim().Equals("does", StringComparison.OrdinalIgnoreCase))
                                {
                                    i++;

                                    /* If the line ends with "does" on its own,
                                       the description will be on the next line. */
                                    if (line.EndsWith("does"))
                                        isMultilineDescription = true;
                                    else
                                        currentCode.Description = matches[i].Value.Trim(' ', '"');
                                }
                            }

                            continue;
                        }
                    }

                    if (isMultilineDescription)
                    {
                        string startDelimiter = "/*";
                        string endDelimiter = "*/";

                        bool lineContainsStartDelimiter = false;
                        bool lineContainsEndDelimiter = false;

                        currentCode.Header.AppendLine(line);

                        if (line.StartsWith(startDelimiter))
                        {
                            if (line == startDelimiter)
                                continue;

                            lineContainsStartDelimiter = true;
                        }

                        if (line.EndsWith(endDelimiter))
                        {
                            isMultilineDescription = false;

                            if (line == endDelimiter)
                                continue;

                            lineContainsEndDelimiter = true;
                        }

                        string GetDescriptionLine()
                        {
                            string result = line;

                            if (lineContainsStartDelimiter)
                            {
                                int delimiterLength = result[startDelimiter.Length].IsWhitespace()
                                                        ? startDelimiter.Length + 1
                                                        : startDelimiter.Length;

                                result = result.Remove(0, delimiterLength);
                            }

                            if (lineContainsEndDelimiter)
                                result = result.Substring(0, result.Length - endDelimiter.Length);

                            return string.IsNullOrEmpty(currentCode.Description) ? result : '\n' + result;
                        }

                        currentCode.Description += GetDescriptionLine();

                        continue;
                    }

                    currentCode?.Lines.AppendLine(line);
                }

                // Remove trailing line breaks.
                if (currentCode != null)
                    currentCode.Lines = new StringBuilder(currentCode.Lines.ToString().TrimEnd(Environment.NewLine.ToCharArray()));
            }

            return codes;
        }

        public List<string> GetReferences()
        {
            var references = new List<string>();

            var tree = ParseSyntaxTree();

            var unit = tree.GetCompilationUnitRoot();

            foreach (var reference in unit.GetReferenceDirectives())
            {
                references.Add(reference.File.ValueText);
            }

            return references;
        }

        public SyntaxTree ParseSyntaxTree()
        {
            var hash = Lines.ToString().GetHashCode();

            if (hash != mCachedHash)
            {
                var lines = Lines.ToString();
                mCachedHash = hash;

                var tokens = SyntaxFactory.ParseTokens(Lines.ToString());
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
                        return MakeSyntaxTree(lines);
                    }

                    return MakeSyntaxTree(lines.Substring(braces[0].Span.End, braces.Last().SpanStart - braces[0].Span.End));
                }

                return MakeSyntaxTree(lines);
            }

            return mCachedSyntaxTree;

            SyntaxTree MakeSyntaxTree(string body)
            {
                mCachedSyntaxTree = CSharpSyntaxTree.ParseText(body,
                    new CSharpParseOptions(kind: SourceCodeKind.Script));
                return mCachedSyntaxTree;
            }
        }

        public CompilationUnitSyntax CreateCompilationUnit()
        {
            var tree = ParseSyntaxTree();

            var unit = tree.GetCompilationUnitRoot();
            if (IsExecutable())
            {
                unit = (CompilationUnitSyntax)new OptionalColonRewriter().Visit(unit);
            }

            var classUnit = SyntaxFactory
                .ClassDeclaration(MakeInternalName())
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.UnsafeKeyword)));

            if (IsExecutable())
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

            return SyntaxFactory.CompilationUnit()
                .AddMembers(classUnit)
                .WithUsings(unit.Usings)
                .AddUsings(CodeProvider.PredefinedUsingDirectives);
        }

        public bool IsExecutable()
            => Type == CodeType.Patch || Type == CodeType.Code;

        public SyntaxTree CreateSyntaxTree()
        {
            return SyntaxFactory.SyntaxTree(CreateCompilationUnit());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string MakeInternalName()
        {
            if (Type == CodeType.Library)
            {
                return Name;
            }

            return Regex.Replace($"{Name}_{Guid.NewGuid()}", "[^a-z_0-9]", string.Empty, RegexOptions.IgnoreCase);
        }

        public static bool CodeTypeFromString(string text, out CodeType type)
        {
            switch (text)
            {
                case "Code":
                    type = CodeType.Code;
                    return true;
                case "Patch":
                    type = CodeType.Patch;
                    return true;
                case "Library":
                    type = CodeType.Library;
                    return true;
            }

            type = CodeType.Unknown;
            return false;
        }
    }
}
