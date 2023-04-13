using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace HMMCodes
{
    public static unsafe class MemoryService
    {
        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpAddress,
                IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public static long ModuleBase = (long)GetModuleHandle(null);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        public static dynamic MemoryProvider;

        public static void RegisterProvider(object provider)
        {
            MemoryProvider = provider;
        }

        public static long ASLR(long address)
             => ModuleBase + (address - (IntPtr.Size == 8 ? 0x140000000 : 0x400000));

        public static void Write(IntPtr address, IntPtr dataPtr, IntPtr length)
            => MemoryProvider.WriteMemory(address, dataPtr, length);

        public static void Write<T>(IntPtr address, T data)
            => MemoryProvider.WriteMemory<T>(address, data);

        public static void Write<T>(long address, params T[] data)
            => MemoryProvider.WriteMemory<T>((IntPtr)address, data);

        public static void Write<T>(long address, T data)
            => Write<T>((IntPtr)address, data);

        public static char[] Read(IntPtr address, IntPtr length)
            => MemoryProvider.ReadMemory(address, length);

        public static T Read<T>(IntPtr address) where T : unmanaged
            => MemoryProvider.ReadMemory<T>(address);

        public static T Read<T>(long address) where T : unmanaged
            => Read<T>((IntPtr)address);

        public static byte[] Assemble(string source)
            => MemoryProvider.AssembleInstructions(source);

        public static long GetPointer(long address, params long[] offsets)
        {
            if (address == 0)
                return 0;

            var result = (long)(*(void**)address);

            if (result == 0)
                return 0;

            if (offsets.Length > 0)
            {
                for (int i = 0; i < offsets.Length - 1; i++)
                {
                    result = (long)((void*)(result + offsets[i]));
                    result = (long)(*(void**)result);
                    if (result == 0)
                        return 0;
                }

                return result + offsets[offsets.Length - 1];
            }

            return result;
        }

        public static void WriteProtected(IntPtr address, IntPtr dataPtr, IntPtr length)
        {
            VirtualProtect((IntPtr)address, length, 0x04, out uint oldProtect);
            Write(address, dataPtr, length);
            VirtualProtect((IntPtr)address, length, oldProtect, out _);
        }

        public static void WriteProtected<T>(long address, T data) where T : unmanaged
        {
            VirtualProtect((IntPtr)address, (IntPtr)sizeof(T), 0x04, out uint oldProtect);
            Write<T>(address, data);
            VirtualProtect((IntPtr)address, (IntPtr)sizeof(T), oldProtect, out _);
        }

        public static void WriteProtected<T>(long address, params T[] data) where T : unmanaged
        {
            VirtualProtect((IntPtr)address, (IntPtr)(sizeof(T) * data.Length), 0x04, out uint oldProtect);
            Write<T>(address, data);
            VirtualProtect((IntPtr)address, (IntPtr)(sizeof(T) * data.Length), oldProtect, out _);
        }

        public static void WriteAsmHook(string instructions, long address, HookBehavior behavior = HookBehavior.After, HookParameter parameter = HookParameter.Jump)
            => MemoryProvider.WriteASMHook(instructions, (IntPtr)address, (int)behavior, (int)parameter);

        public static void WriteAsmHook(string instructions, long address, HookParameter parameter, HookBehavior behavior)
            => WriteAsmHook(instructions, address, behavior, parameter);

        public static void WriteAsmHook(long address, HookBehavior behavior, HookParameter parameter, params string[] instructions)
            => WriteAsmHook(string.Join("\r\n", instructions), address, behavior, parameter);

        public static void WriteAsmHook(long address, HookBehavior behavior, params string[] instructions)
            => WriteAsmHook(string.Join("\r\n", instructions), address, behavior, HookParameter.Jump);

        public static IntPtr ScanSignature(byte[] pattern, string mask)
            => MemoryProvider.ScanSignature(pattern, mask);

        public static uint NopInstructions(long address, uint count)
            => MemoryProvider.NopInstructions((IntPtr)address, count);

        public static uint NopInstruction(long address)
            => NopInstructions(address, 1);

        public static void WriteNop(long address, long count)
        {
            for (long i = 0; i < count; i++)
                WriteProtected<byte>(address + i, 0x90);
        }

        public static byte[] MakePatternFromString(string pattern)
        {
            byte[] patBytes = new byte[pattern.Length];

            for (int i = 0; i < patBytes.Length; i++)
                patBytes[i] = (byte)pattern[i];

            return patBytes;
        }

        public static long ScanSignature(params string[] sigs)
        {
            if (sigs.Length % 2 != 0)
                return 0;

            for (int i = 0; i < sigs.Length - 1; i++)
            {
                var result = ScanSignature(MakePatternFromString(sigs[i * 2 + 0]), sigs[i * 2 + 1]).ToInt64();
                if (result != 0)
                    return result;
            }
            return 0;
        }

        public static bool IsKeyDown(Keys key)
            => GetAsyncKeyState(key) > 0;
    }

    public enum HookBehavior
    {
        After,
        Before,
        Replace
    }

    public enum HookParameter
    {
        Jump = 0,
        Call = 1,
    }
}
