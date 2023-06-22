namespace HedgeModManager;
using CodeCompiler;
using Foundation;
using Text;

public class ModGeneric : IMod
{
    public const string ConfigName = "mod.ini";

    public string ID { get; set; } = string.Empty;
    public string Root { get; private set; }
    public bool Enabled { get; set; }
    public string UpdateServer { get; set; } = string.Empty;
    public ModAttribute Attributes { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public List<ModAuthor> Authors { get; set; } = new();
    public List<ModDependency> Dependencies { get; set; } = new();
    public List<CSharpCode> Codes { get; set; } = new();
    public List<BinaryCode> BinaryCodes { get; set; } = new();
    public List<string> IncludeDirectories { get; set; } = new();

    public ModGeneric()
    {
        Root = string.Empty;
    }

    public ModGeneric(string root)
    {
        Root = root;
    }

    public void Parse(Ini file)
    {
        if (file.TryGetValue("Main", out var mainSection))
        {
            ID = mainSection.Get("ID", string.Empty);
            IncludeDirectories = mainSection.GetList<string>("IncludeDir");
            UpdateServer = mainSection.Get("UpdateServer", string.Empty);

            var codeFiles = mainSection.Get("CodeFile", string.Empty);
            if (!string.IsNullOrEmpty(codeFiles))
            {
                foreach (var codeFile in codeFiles.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var codePath = Path.Combine(Root, codeFile.Trim());
                    if (File.Exists(codePath))
                    {
                        Codes.AddRange(CodeFile.FromFile(codePath).Codes);
                    }
                }
            }

            var dllFiles = mainSection.Get("DLLFile", string.Empty);
            if (!string.IsNullOrEmpty(dllFiles))
            {
                foreach (var dllFile in dllFiles.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var dllPath = Path.Combine(Root, dllFile.Trim());
                    if (File.Exists(dllPath))
                    {
                        BinaryCodes.Add(new BinaryCode(dllPath));
                    }
                }
            }
        }

        if (file.TryGetValue("Desc", out var descSection))
        {
            Title = descSection.Get("Title", string.Empty);
        }

        if (string.IsNullOrEmpty(ID))
        {
            ID = Title.GetDeterministicHashCode().ToString("X");
        }
    }

    IReadOnlyList<ICode> IMod.Codes => Codes;
}