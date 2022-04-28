namespace HedgeModManager.Installer;

public class FileStorage
{
    public string BasePath { get; }

    public FileStorage(string path)
    {
        BasePath = path;
    }

    public void EnsureBase()
    {
        Directory.CreateDirectory(BasePath);
    }

    public void Delete(string name)
    {
        var path = GetFullPath(name);
        if (!File.Exists(path))
            return;

        File.Delete(path);
    }

    public string GetFullPath(string name)
        => Path.Combine(BasePath, name);

    public bool Exists(string name)
        => File.Exists(GetFullPath(name));

    public Stream Open(string name)
    {
        var path = GetFullPath(name);
        return !File.Exists(path) ? null : File.OpenRead(path);
    }

    public Stream Create(string name)
    {
        var path = GetFullPath(name);
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir!);

        return File.Create(path);
    }

    public Stream OpenOrCreate(string name)
        => Exists(name) ? Open(name) : Create(name);

    public static FileStorage FromSpecialFolder(Environment.SpecialFolder folder, string name)
    {
        var path = string.IsNullOrEmpty(name) ? Environment.GetFolderPath(folder) : Path.Combine(Environment.GetFolderPath(folder), name);
        var storage = new FileStorage(path);
        storage.EnsureBase();
        return storage;
    }
}