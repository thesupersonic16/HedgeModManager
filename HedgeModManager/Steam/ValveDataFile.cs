namespace HedgeModManager.Steam;
using System.Buffers.Binary;
using ValveKeyValue;

public static class ValveDataFile
{
    public const int BinaryMagicHeader = 0x564B4256; // VBKV
    public static KVSerializer TextSerializer { get; } = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
    public static KVSerializer BinarySerializer { get; } = KVSerializer.Create(KVSerializationFormat.KeyValues1Binary);
    public static KVSerializerOptions SerializerOptions { get; } = new()
    {
        HasEscapeSequences = true
    };

    public static KVObject Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 4)
        {
            return new KVObject("Error", "Invalid data length");
        }

        var magicHeader = BinaryPrimitives.ReadInt32LittleEndian(data);
        if (magicHeader == BinaryMagicHeader)
        {
            return BinarySerializer.Deserialize(data.AsStream(), SerializerOptions);
        }
        
        return TextSerializer.Deserialize(data.AsStream(), SerializerOptions);
    }

    public static KVObject FromFile(string path) => Parse(File.ReadAllBytes(path));
}