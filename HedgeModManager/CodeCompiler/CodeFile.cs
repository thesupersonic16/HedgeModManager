using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HedgeModManager.Misc;
using Markdig.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HedgeModManager.CodeCompiler
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
            var addedCodes = new List<Code>();

            string GetCodeDiffName(Code code)
            {
                return !string.IsNullOrEmpty(code.Category)
                    ? $"[{code.Category}] {code.Name}"
                    : code.Name;
            }

            foreach (var code in Codes)
            {
                if (code.Type == CodeType.Library)
                    continue;

                // Added
                if (!old.Codes.Any(x => x.Name == code.Name))
                {
                    addedCodes.Add(code);
                    continue;
                }
            }

            foreach (var code in old.Codes)
            {
                if (code.Type == CodeType.Library)
                    continue;

                // Modified
                if (Codes.SingleOrDefault(x => x.Name == code.Name) is Code modified)
                {
                    if (code.Lines.ToString() != modified.Lines.ToString())
                    {
                        diff.Add(new CodeDiffResult(GetCodeDiffName(code), CodeDiffResult.CodeDiffType.Modified));
                        continue;
                    }
                }

                // Renamed
                if (Codes.SingleOrDefault(x => x.Lines.ToString() == code.Lines.ToString()) is Code renamed)
                {
                    if (code.Name != renamed.Name)
                    {
                        diff.Add(new CodeDiffResult($"[{code.Category}] {code.Name} -> [{renamed.Category}] {renamed.Name}", CodeDiffResult.CodeDiffType.Renamed, code.Name, renamed.Name));

                        // Remove this code from the added list so we don't display it twice.
                        if (addedCodes.SingleOrDefault(x => x.Name == renamed.Name) is Code duplicate)
                            addedCodes.Remove(duplicate);

                        continue;
                    }
                }

                // Removed
                if (!Codes.Any(x => x.Name == code.Name))
                {
                    diff.Add(new CodeDiffResult(GetCodeDiffName(code), CodeDiffResult.CodeDiffType.Removed));
                    continue;
                }
            }

            foreach (var code in addedCodes)
                diff.Add(new CodeDiffResult(GetCodeDiffName(code), CodeDiffResult.CodeDiffType.Added));

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
}
