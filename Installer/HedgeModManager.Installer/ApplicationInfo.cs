namespace HedgeModManager.Installer;
using Microsoft.Win32;
using System.Dynamic;

public class ApplicationInfo : DynamicObject
{
    public const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

    public string ID { get; set; }

    public string DisplayName
    {
        get => GetParameter<string>(nameof(DisplayName));
        set => SetParameter(nameof(DisplayName), value);
    }

    public string DisplayVersion
    {
        get => GetParameter<string>(nameof(DisplayVersion));
        set => SetParameter(nameof(DisplayVersion), value);
    }

    public string DisplayIcon
    {
        get => GetParameter<string>(nameof(DisplayIcon));
        set => SetParameter(nameof(DisplayIcon), value);
    }

    public string Publisher
    {
        get => GetParameter<string>(nameof(Publisher));
        set => SetParameter(nameof(Publisher), value);
    }

    public string InstallDate
    {
        get => GetParameter<string>(nameof(InstallDate));
        set => SetParameter(nameof(InstallDate), value);
    }

    public string UninstallAction
    {
        get => GetParameter<string>("UninstallString");
        set => SetParameter("UninstallString", value);
    }

    public string QuietUninstallAction
    {
        get => GetParameter<string>("QuietUninstallString");
        set => SetParameter("QuietUninstallString", value);
    }

    public int? Language
    {
        get => GetParameter<int?>(nameof(Language));
        set => SetParameter(nameof(Language), value);
    }

    public bool NoModify
    {
        get => Convert.ToBoolean(GetParameter<int>(nameof(NoModify)));
        set => SetParameter(nameof(NoModify), Convert.ToInt32(value));
    }

    public bool NoRepair
    {
        get => Convert.ToBoolean(GetParameter<int>(nameof(NoRepair)));
        set => SetParameter(nameof(NoRepair), Convert.ToInt32(value));
    }

    public bool IsUserLocal { get; set; }

    public Dictionary<string, object> Parameters { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

    public ApplicationInfo()
    {
        NoModify = true;
        NoRepair = true;
    }

    public ApplicationInfo(string id, bool userLocal) : this()
    {
        ID = id;
        IsUserLocal = userLocal;
    }

    public T GetParameter<T>(string name, T defaultValue = default)
    {
        if (!Parameters.TryGetValue(name, out var value))
            return defaultValue;

        return (T)value;
    }

    public void SetParameter(string name, object value)
    {
        if (value is null)
        {
            Parameters.Remove(name);
            return;
        }

        if (Parameters.ContainsKey(name))
        {
            Parameters[name] = value;
            return;
        }

        Parameters.Add(name, value);
    }

    public void Read(RegistryKey key)
    {
        ID = Path.GetFileName(key.Name);

        foreach (var name in key.GetValueNames())
        {
            if (Parameters.ContainsKey(name))
                Parameters.Remove(name);

            Parameters.Add(name, key.GetValue(name));
        }
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(ID))
            throw new Exception("ID must not be null or empty.");

        using var baseKey = IsUserLocal ? Registry.CurrentUser : Registry.ClassesRoot;
        using var appKey = baseKey.CreateSubKey($"{RegistryPath}\\{ID}");

        foreach (var parameter in Parameters)
        {
            appKey.SetValue(parameter.Key, parameter.Value);
        }
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        return Parameters.Keys;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return Parameters.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        SetParameter(binder.Name, value);
        return true;
    }

    public static ApplicationInfo Open(string id, bool userLocal)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        using var baseKey = userLocal ? Registry.CurrentUser : Registry.ClassesRoot;
        using var appKey = baseKey.OpenSubKey($"{RegistryPath}\\{id}", false);
        if (appKey == null)
            return null;

        var info = new ApplicationInfo
        {
            IsUserLocal = userLocal
        };

        info.Read(appKey);
        return info;
    }

    public static void Delete(string id, bool userLocal)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        using var baseKey = userLocal ? Registry.CurrentUser : Registry.ClassesRoot;
        baseKey.DeleteSubKey($"{RegistryPath}\\{id}");
    }
}