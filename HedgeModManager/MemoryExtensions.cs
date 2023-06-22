namespace HedgeModManager;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;

public static class MemoryExtensions
{
    public static unsafe UnmanagedMemoryStream AsStream<T>(this ReadOnlySpan<T> memory) where T : unmanaged
    {
        fixed (T* pin = &memory.GetPinnableReference())
        {
            return new UnmanagedMemoryStream((byte*)pin, memory.Length * Unsafe.SizeOf<T>(), memory.Length * Unsafe.SizeOf<T>(), FileAccess.Read);
        }
    }

    public static unsafe UnmanagedMemoryStream AsStream<T>(this Span<T> memory) where T : unmanaged
    {
        fixed (T* pin = &memory.GetPinnableReference())
        {
            return new UnmanagedMemoryStream((byte*)pin, memory.Length * Unsafe.SizeOf<T>(), memory.Length * Unsafe.SizeOf<T>(), FileAccess.ReadWrite);
        }
    }
}