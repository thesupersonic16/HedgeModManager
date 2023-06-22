namespace HedgeModManager.CodeCompiler.PreProcessor;

public class MemoryLineReader
{
    public ReadOnlyMemory<char> Text { get; }
    public int Position { get; private set; }
    public long Line { get; private set; }

    public MemoryLineReader(string text)
    {
        Text = text.AsMemory();
    }

    public bool ReadLine(out ReadOnlyMemory<char> line)
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
            var c = Text.Span[end];
            if (c == '\r' || c == '\n')
            {
                if (c == '\r' && end + 1 < length && Text.Span[end + 1] == '\n')
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