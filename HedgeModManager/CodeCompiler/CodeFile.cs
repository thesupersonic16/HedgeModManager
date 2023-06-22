namespace HedgeModManager.CodeCompiler;
using Diagnostics;
using Foundation;

public class CodeFile
{
    public const string TagPrefix = "!!";
    public const string VersionTag = "VERSION";
    protected Version? mFileVersion;

    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    public List<CSharpCode> Codes { get; set; } = new List<CSharpCode>();
    public IEnumerable<CSharpCode> ExecutableCodes => Codes.Where(x => x.IsExecutable());

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

    public Diff CalculateDiff(CodeFile old)
    {
        var diff = new Diff();
        var addedCodes = new List<CSharpCode>();

        string GetCodeDiffName(CSharpCode code)
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
            if (old.Codes.All(x => x.Name != code.Name))
            {
                addedCodes.Add(code);
                continue;
            }
        }

        foreach (var code in old.Codes)
        {
            if (code.Type == CodeType.Library)
            {
                continue;
            }

            // Modified
            if (Codes.SingleOrDefault(x => x.Name == code.Name) is { } modified)
            {
                if (code.Body != modified.Body)
                {
                    diff.Modified(GetCodeDiffName(code));
                    continue;
                }
            }

            // Renamed
            if (Codes.SingleOrDefault(x => x.Body == code.Body) is { } renamed)
            {
                if (code.Name != renamed.Name)
                {
                    diff.Renamed($"{GetCodeDiffName(code)} -> {GetCodeDiffName(renamed)}", code.Name, renamed.Name);

                    // Remove this code from the added list so we don't display it twice.
                    if (addedCodes.SingleOrDefault(x => x.Name == renamed.Name) is { } duplicate)
                    {
                        addedCodes.Remove(duplicate);
                    }

                    continue;
                }
            }

            // Removed
            if (Codes.All(x => x.Name != code.Name))
            {
                diff.Removed(GetCodeDiffName(code));
                continue;
            }
        }

        foreach (var code in addedCodes)
        {
            diff.Added(GetCodeDiffName(code));
        }
        
        return diff;
    }

    public void ParseFile(string path)
    {
        if (File.Exists(path))
        {
            using var stream = File.OpenRead(path);
            Parse(stream);
        }
    }

    public void Parse(Stream stream)
    {
        var start = stream.Position;
        using var reader = new StreamReader(stream);

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
        Codes.AddRange(CSharpCode.Parse(reader));
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