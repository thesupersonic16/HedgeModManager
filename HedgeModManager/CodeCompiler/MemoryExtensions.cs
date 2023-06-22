namespace HedgeModManager.CodeCompiler;

public static class MemoryExtensions
{
    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> memory)
    {
        return memory.Trim(' ', '\t', '\r', '\n');
    }

    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> memory, params char[] trimChars)
    {
        var start = 0;
        var end = memory.Length - 1;

        for (int i = 0; i < memory.Length; i++)
        {
            if (trimChars.Contains(memory.Span[i]))
            {
                start++;
            }
            else
            {
                break;
            }
        }

        for (int i = memory.Length - 1; i >= start; i--)
        {
            if (trimChars.Contains(memory.Span[i]))
            {
                end--;
            }
            else
            {
                break;
            }
        }

        return memory.Slice(start, end - start + 1);
    }
}