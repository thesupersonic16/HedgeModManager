namespace HedgeModManager.Text;

public static class TextExtensions
{
    public static List<T> GetList<T>(this IniGroup group, string key)
    {
        var count = group.Get($"{key}Count", 0);
        var result = new List<T>(count);

        for (int i = 0; i < count; i++)
        {
            var value = group.Get($"{key}{i}", default(T?));
            if (value is not null)
            {
                result.Add(value);
            }
            else
            {
                break;
            }
        }

        return result;
    }

    public static void SetList<TValue>(this IniGroup group, string key, ICollection<TValue> values)
    {
        group.Set($"{key}Count", values.Count);

        // Workaround to support both List<T> and T[]
        var i = 0;
        foreach (var value in values)
        {
            group.Set($"{key}{i}", value);
            i++;
        }
    }

    public static int GetDeterministicHashCode(this string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}