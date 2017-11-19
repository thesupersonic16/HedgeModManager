using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SonicAudioLib.CriMw
{
    [StructLayout(LayoutKind.Sequential)]
    struct CriTableHeader
    {
        public static readonly byte[] SignatureBytes = { 0x40, 0x55, 0x54, 0x46 };

        public const byte EncodingTypeShiftJis = 0;
        public const byte EncodingTypeUtf8 = 1;

        public byte[] Signature;
        public uint Length;
        public byte UnknownByte;
        public byte EncodingType;
        public ushort RowsPosition;
        public uint StringPoolPosition;
        public uint DataPoolPosition;
        public uint TableNamePosition;
        public string TableName;
        public ushort FieldCount;
        public ushort RowLength;
        public uint RowCount;
    }

    [Flags]
    enum CriFieldFlag : byte
    {
        Name = 16,
        DefaultValue = 32,
        RowStorage = 64,
        
        Byte = 0,
        SByte = 1,
        UInt16 = 2,
        Int16 = 3,
        UInt32 = 4,
        Int32 = 5,
        UInt64 = 6,
        Int64 = 7,
        Single = 8,
        Double = 9,
        String = 10,
        Data = 11,
        Guid = 12,

        TypeMask = 15,
    };

    [StructLayout(LayoutKind.Sequential)]
    struct CriTableField
    {
        public CriFieldFlag Flag;
        public string Name;
        public uint Position;
        public uint Length;
        public uint Offset;
        public object Value;
    }

    static class CriTableMasker
    {
        public static void FindKeys(byte[] signature, out uint xor, out uint xorMultiplier)
        {
            for (byte x = 0; x <= byte.MaxValue; x++)
            {
                // Find XOR using first byte
                if ((signature[0] ^ x) == CriTableHeader.SignatureBytes[0])
                {
                    // Matched the first byte, try finding the multiplier with the second byte
                    for (byte m = 0; m <= byte.MaxValue; m++)
                    {
                        // Matched the second byte, now make sure the other bytes match as well
                        if ((signature[1] ^ (byte)(x * m)) == CriTableHeader.SignatureBytes[1])
                        {
                            byte _x = (byte)(x * m);

                            bool allMatches = true;
                            for (int i = 2; i < 4; i++)
                            {
                                _x *= m;

                                if ((signature[i] ^ _x) != CriTableHeader.SignatureBytes[i])
                                {
                                    allMatches = false;
                                    break;
                                }
                            }

                            // All matches, return the xor and multiplier
                            if (allMatches)
                            {
                                xor = x;
                                xorMultiplier = m;
                                return;
                            }
                        }
                    }
                }
            }

            throw new InvalidDataException("'@UTF' signature could not be found.");
        }

        public static void Mask(Stream source, Stream destination, long length, uint xor, uint xorMultiplier)
        {
            uint currentXor = xor;
            long currentPosition = source.Position;

            while (source.Position < currentPosition + length)
            {
                byte maskedByte = (byte)(source.ReadByte() ^ currentXor);
                currentXor *= xorMultiplier;

                destination.WriteByte(maskedByte);
            }
        }

        public static void Mask(Stream source, long length, uint xor, uint xorMultiplier)
        {
            if (source.CanRead && source.CanWrite)
            {
                uint currentXor = xor;
                long currentPosition = source.Position;

                while (source.Position < currentPosition + length)
                {
                    byte maskedByte = (byte)(source.ReadByte() ^ currentXor);
                    currentXor *= xorMultiplier;

                    source.Position--;
                    source.WriteByte(maskedByte);
                }
            }
        }
    }
}
