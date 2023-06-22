namespace HedgeModManager.IO;

public static class PathEx
{
    public static ReadOnlySpan<char> DirectorySeparatorChars => new[] { '\\', '/' };

    public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> text)
    {
        var index = text.LastIndexOfAny(DirectorySeparatorChars);
        if (index == -1)
        {
            return text;
        }

        return text[(index + 1)..];
    }

    public static ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> text)
    {
        var index = text.LastIndexOfAny(DirectorySeparatorChars);
        if (index == -1)
        {
            return ReadOnlySpan<char>.Empty;
        }

        return text[..index];
    }

    public static ReadOnlySpan<char> GetDirectoryNameOnly(ReadOnlySpan<char> text)
    {
        var index = text.LastIndexOfAny(DirectorySeparatorChars);
        if (index == -1)
        {
            return ReadOnlySpan<char>.Empty;
        }
        return GetFileName(text[..index]);
    }
}