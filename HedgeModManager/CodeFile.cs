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

namespace HedgeModManager
{
    public class CodeFile
    {
        public const string TagPrefix = "!!";
        public const string VersionTag = "VERSION";
        protected Version mFileVersion;

        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public List<Code> Codes { get; set; } = new List<Code>();

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
            var diff       = new List<CodeDiffResult>();
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
            Type      = type;
        }

        public CodeDiffResult(string changelog, CodeDiffType type, string originalName, string newName)
        {
            Changelog    = changelog;
            Type         = type;
            OriginalName = originalName;
            NewName      = newName;
        }

        public override string ToString()
        {
            string key = Type switch
            {
                CodeDiffType.Added   => "DiffUIAdded",
                CodeDiffType.Removed => "DiffUIRemoved",
                CodeDiffType.Renamed => "DiffUIRenamed",
                _                    => "DiffUIModified",
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

    public class Code : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public bool IsPatch { get; set; }

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

                    bool isCode = line.StartsWith("Code");
                    bool isPatch = line.StartsWith("Patch");
                    if (isPatch || isCode)
                    {
                        currentCode = new Code();
                        codes.Add(currentCode);

                        currentCode.Header.AppendLine(line);

                        var matches = Regex.Matches(line, "(\"[^\"]*\"|[^\"]+)(\\s+|$)");
                        currentCode.IsPatch = isPatch;
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

                    if (isMultilineDescription)
                    {
                        string startDelimiter = "/*";
                        string endDelimiter   = "*/";

                        bool lineContainsStartDelimiter = false;
                        bool lineContainsEndDelimiter   = false;

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
                .ClassDeclaration(Regex.Replace($"{Name}_{Guid.NewGuid()}", "[^a-z_0-9]", string.Empty, RegexOptions.IgnoreCase))
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
