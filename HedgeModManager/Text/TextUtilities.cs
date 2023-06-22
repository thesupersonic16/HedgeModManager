namespace HedgeModManager.Text;

public static class TextUtilities
{
    public static List<ReadOnlyMemory<char>> CommandLineToArgs(ReadOnlyMemory<char> line)
    {
        var args = new List<ReadOnlyMemory<char>>();

        var i = 0;
        var t = Token(i);
        while (i < line.Length)
        {
            if (t.Length > 0)
            {
                if (t.Span.StartsWith("\""))
                {
                    t = t.Slice(1);
                }

                if (t.Span.EndsWith("\""))
                {
                    t = t.Slice(0, t.Length - 1);
                }

                args.Add(t);
            }

            i += t.Length + 1;
            t = Token(i);
        }

        return args;

        ReadOnlyMemory<char> Token(int pos)
        {
            var len = 0;
            var inQuotes = false;

            for (int i = pos; i < line.Length; i++)
            {
                if (line.Span[i] == '"')
                {
                    inQuotes = !inQuotes;
                }

                if (line.Span[i] == ' ' && !inQuotes)
                {
                    len = i - pos;
                    break;
                }
            }

            if (len == 0)
            {
                len = line.Length - pos;
            }

            if (len < 0)
            {
                return ReadOnlyMemory<char>.Empty;
            }

            return line.Slice(pos, len);
        }
    }
}