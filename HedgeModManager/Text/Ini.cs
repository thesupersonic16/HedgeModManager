namespace HedgeModManager.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Text;

public class Ini
{
    public Dictionary<string, IniGroup> Groups { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public IniGroup GetOrAddValue(string key)
    {
        if (!Groups.TryGetValue(key, out var value))
        {
            value = new IniGroup();
            Groups.Add(key, value);
        }

        return value;
    }

    public bool TryGetValue(string key, out IniGroup value)
    {
        return Groups.TryGetValue(key, out value!);
    }

    public void Parse(ReadOnlySpan<char> text)
    {
        var reader = new LineReader(text);
        var currentGroup = new IniGroup();

        while (reader.ReadLine(out var line))
        {
            // A=B is at least 3 characters long
            if (line.Length < 3 || line[0] == ';')
            {
                continue;
            }

            // Skip whitespace
            line = line.Trim(' ');

            switch (line[0])
            {
                case ';':
                    continue;

                case '[':
                    {
                        var groupEnd = line.IndexOf(']');
                        if (groupEnd > 0)
                        {
                            var groupName = line[1..groupEnd].ToString();
                            if (!Groups.TryGetValue(groupName, out currentGroup))
                            {
                                currentGroup = new IniGroup();
                                Groups.Add(groupName, currentGroup);
                            }
                        }

                        break;
                    }

                default:
                    {
                        var setterIdx = line.IndexOf('=');
                        if (setterIdx < 0)
                        {
                            continue;
                        }

                        var key = line[..setterIdx].Trim(' ');
                        var value = line[(setterIdx + 1)..].Trim(' ');

                        if (value.IsEmpty)
                        {
                            currentGroup.Set(key.ToString(), string.Empty);
                            continue;
                        }

                        if (value[0] == '"')
                        {
                            currentGroup.Set(key.ToString(), value.Trim('"').ToString());
                            continue;
                        }

                        if (int.TryParse(value, NumberStyles.Any, null, out var intVal))
                        {
                            currentGroup.Set(key.ToString(), intVal);
                        }
                        else if (float.TryParse(value, NumberStyles.Any, null, out var floatVal))
                        {
                            currentGroup.Set(key.ToString(), floatVal);
                        }
                        else if (bool.TryParse(value, out var boolVal))
                        {
                            currentGroup.Set(key.ToString(), boolVal);
                        }
                        else
                        {
                            currentGroup.Set(key.ToString(), value.ToString());
                        }

                        break;
                    }
            }
        }
    }

    public string Serialize()
    {
        var builder = new StringBuilder();
        foreach (var group in Groups)
        {
            builder.AppendLine($"[{group.Key}]");
            foreach (var property in group.Value.Properties)
            {
                if (property.Value is string strVal)
                {
                    builder.AppendLine($"{property.Key}=\"{strVal}\"");
                    continue;
                }
                builder.AppendLine($"{property.Key}={property.Value}");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    public static Ini FromFile(string path)
    {
        var ini = new Ini();
        ini.Parse(File.ReadAllText(path));
        return ini;
    }
}

public class IniGroup : IDictionary<string, object>
{
    public Dictionary<string, object> Properties { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public ICollection<string> Keys => Properties.Keys;
    public ICollection<object> Values => Properties.Values;
    public int Count => Properties.Count;
    public bool IsReadOnly => false;

    public T Get<T>(string key, T? defaultValue = default)
    {
        return Properties.TryGetValue(key, out var value) ? (T)Convert.ChangeType(value, typeof(T)) : defaultValue!;
    }

    public void Set(string key, object value)
    {
        ref var valueRef = ref CollectionsMarshal.GetValueRefOrAddDefault(Properties, key, out _);
        valueRef = value;
    }

    public void Add(string key, object value)
    {
        Set(key, value);
    }

    public bool ContainsKey(string key)
    {
        return Properties.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return Properties.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return Properties.TryGetValue(key, out value!);
    }

    public void Add(KeyValuePair<string, object> item)
    {
        Set(item.Key, item.Value);
    }

    public void Clear()
    {
        Properties.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return Properties.Contains(item);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return Properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Properties).GetEnumerator();
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        (Properties as IDictionary<string, object>).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return Properties.Remove(item.Key);
    }

    public object this[string key]
    {
        get => Get<object>(key, string.Empty);
        set => Set(key, value);
    }
}