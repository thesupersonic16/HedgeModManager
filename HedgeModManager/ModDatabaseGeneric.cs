namespace HedgeModManager;
using CodeCompiler;
using Foundation;
using System.IO;
using Text;
using IO;

public class ModDatabaseGeneric : IModDatabase
{
    public const string DefaultDatabaseName = "ModsDB.ini";
    public const string MainCodesFileName = "Codes.hmm";
    public const string ExtraCodesFileName = "ExtraCodes.hmm";
    public const string CompileCodesFileName = "Codes.dll";

    public string Name { get; set; } = DefaultDatabaseName;
    public string Root { get; set; } = string.Empty;
    public List<ModGeneric> Mods { get; set; } = new();
    public List<CSharpCode> Codes { get; set; } = new();

    public async Task Save()
    {
        var ini = new Ini();

        var enabledMods = new List<string>();
        var enabledCodes = new List<string>();
        var favoriteMods = new List<string>();

        var mainSection = ini.GetOrAddValue("Main");
        var modsSection = ini.GetOrAddValue("Mods");
        var codesSection = ini.GetOrAddValue("Codes");

        foreach (var mod in Mods)
        {
            var id = Guid.NewGuid();
            if (mod.Enabled)
            {
                enabledMods.Add(id.ToString());
            }

            if (mod.Attributes.HasFlag(ModAttribute.Favorite))
            {
                favoriteMods.Add(id.ToString());
            }

            modsSection.Set(id.ToString(), Path.Combine(mod.Root, ModGeneric.ConfigName));
        }

        foreach (var code in Codes)
        {
            if (code.Enabled)
            {
                enabledCodes.Add(code.Name);
            }
        }

        mainSection.Set("ReverseLoadOrder", 1);
        mainSection.SetList("ActiveMod", enabledMods);
        mainSection.SetList("FavoriteMod", favoriteMods);
        codesSection.SetList("Code", enabledCodes);

        var codes = new List<CSharpCode>(Codes.Where(x => x.Enabled));
        foreach (var mod in Mods)
        {
            if (mod.Enabled)
            {
                codes.AddRange(mod.Codes);
            }
        }

        await using var codeStream = File.Create(Path.Combine(Root, CompileCodesFileName));
        await CodeProvider.CompileCodes(codes, codeStream);
        await File.WriteAllTextAsync(Path.Combine(Root, Name), ini.Serialize());
    }

    public void LoadDatabase(string path, bool scan = true)
    {
        Name = PathEx.GetFileName(path).ToString();
        Root = PathEx.GetDirectoryName(path).ToString();
        LoadCodes(Root);

        if (!File.Exists(path))
        {
            if (scan)
            {
                LoadDirectory(Root);
            }

            return;
        }

        var ini = Ini.FromFile(path);
        var parsed = Parse(ini);
        var enabledKeys = parsed.Enabled;
        var favoriteKeys = parsed.Favorites;

        Codes.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        for (int i = parsed.Codes.Count - 1; i >= 0; i--)
        {
            var enabledCode = parsed.Codes[i];
            var codeIdx = Codes.FindIndex(c => c.Name == enabledCode);
            if (codeIdx != -1)
            {
                var code = Codes[codeIdx];
                code.Enabled = true;
                Codes.RemoveAt(codeIdx);
                Codes.Insert(0, code);
            }
        }

        if (scan)
        {
            LoadDirectory(Root);
            var lookup = new Dictionary<string, ModGeneric>(Mods.Count, StringComparer.OrdinalIgnoreCase);

            foreach (var mod in Mods)
            {
                lookup.Add(PathEx.GetFileName(mod.Root).ToString(), mod);
            }

            NormalizeList(enabledKeys);
            NormalizeList(favoriteKeys);

            var enabledCount = 0;
            for (int i = enabledKeys.Count - 1; i >= 0; i--)
            {
                var enabledKey = enabledKeys[i];
                if (lookup.TryGetValue(enabledKey, out var mod))
                {
                    Mods.Remove(mod);
                    mod.Enabled = true;

                    Mods.Insert(0, mod);
                    enabledCount++;
                }
            }

            for (int i = favoriteKeys.Count - 1; i >= 0; i--)
            {
                var favoriteKey = favoriteKeys[i];
                if (lookup.TryGetValue(favoriteKey, out var mod))
                {
                    mod.Attributes |= ModAttribute.Favorite;
                    if (!mod.Enabled)
                    {
                        Mods.Remove(mod);
                        Mods.Insert(enabledCount, mod);
                    }
                }
            }
        }

        void NormalizeList(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = PathEx.GetDirectoryNameOnly(list[i]).ToString();
            }
        }
    }

    public int LoadDirectory(string directory, bool append = false)
    {
        var counter = 0;
        if (!append)
        {
            Mods.Clear();
        }

        if (!Directory.Exists(directory))
        {
            return counter;
        }

        foreach (var dir in Directory.EnumerateDirectories(directory))
        {
            var modPath = Path.Combine(dir, ModGeneric.ConfigName);
            if (!File.Exists(modPath))
            {
                continue;
            }

            var mod = new ModGeneric(dir);
            mod.Parse(Ini.FromFile(modPath));
            Mods.Add(mod);
            counter++;
        }

        return counter;
    }

    public void LoadCodes(string directory, bool append = false)
    {
        if (!append)
        {
            Codes.Clear();
        }

        var codesPath = Path.Combine(directory, MainCodesFileName);
        var extraCodesPath = Path.Combine(directory, ExtraCodesFileName);
        LoadSingleCodeFile(codesPath);
        LoadSingleCodeFile(extraCodesPath);
    }

    public void LoadSingleCodeFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Codes.AddRange(CodeFile.FromFile(path).Codes);
            }
        }
        catch
        {
            // ignore
        }
    }

    /// <summary>
    /// Parse an ini file and load mods from it
    /// </summary>
    /// <param name="ini">File to parse</param>
    /// <returns>List of values of mods that are supposed to be</returns>
    public ParseReport Parse(Ini ini)
    {
        var report = new ParseReport();
        var enabledMods = new HashSet<string>();
        var favoriteMods = new HashSet<string>();

        if (ini.TryGetValue("Main", out var mainSection))
        {
            foreach (var mod in mainSection.GetList<string>("ActiveMod"))
            {
                enabledMods.Add(mod);
            }

            foreach (var mod in mainSection.GetList<string>("FavoriteMod"))
            {
                favoriteMods.Add(mod);
            }
        }

        if (ini.TryGetValue("Codes", out var codesSection))
        {
            report.Codes = codesSection.GetList<string>("Code");
        }

        if (ini.TryGetValue("Mods", out var modsGroup))
        {
            foreach (var mod in modsGroup)
            {
                var path = mod.Value.ToString();
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (enabledMods.Contains(mod.Key))
                {
                    report.Enabled.Add(path);
                }

                if (favoriteMods.Contains(mod.Key))
                {
                    report.Favorites.Add(path);
                }

                if (!File.Exists(path))
                {
                    continue;
                }

                var m = new ModGeneric(Path.GetDirectoryName(path) ?? string.Empty);
                m.Parse(Ini.FromFile(path));
                m.Enabled = enabledMods.Contains(mod.Key);
                if (favoriteMods.Contains(mod.Key))
                {
                    m.Attributes |= ModAttribute.Favorite;
                }

                Mods.Add(m);
            }
        }

        return report;
    }

    IReadOnlyList<ICode> IModDatabase.Codes => Codes;
    IReadOnlyList<IMod> IModDatabase.Mods => Mods;

    public struct ParseReport
    {
        public List<string> Enabled = new();
        public List<string> Favorites = new();
        public List<string> Codes = new();

        public ParseReport()
        {
        }
    }
}