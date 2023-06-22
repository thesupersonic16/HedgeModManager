namespace HedgeModManager.Text;

public ref struct LineReader
{
    public ReadOnlySpan<char> Text { get; }
    public int Position { get; private set; }
    public int Line { get; private set; }

    public LineReader(ReadOnlySpan<char> text)
    {
        Text = text;
    }

    public bool ReadLine(out ReadOnlySpan<char> line)
    {
        if (Position >= Text.Length)
        {
            line = default;
            return false;
        }

        bool crlf = false;
        var start = Position;
        var end = Position;
        var length = Text.Length;

        for (; end < length; ++end)
        {
            var c = Text[end];
            if (c == '\r' || c == '\n')
            {
                if (c == '\r' && end + 1 < length && Text[end + 1] == '\n')
                {
                    crlf = true;
                }

                break;
            }
        }

        Position = end + 1;
        if (crlf)
        {
            Position++;
        }

        line = Text.Slice(start, end - start);
        Line++;
        return true;
    }
}