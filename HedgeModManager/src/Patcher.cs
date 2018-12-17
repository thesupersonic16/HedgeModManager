using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class Patcher
    {

        public static void PatchFile(byte[] patch, string filePath)
        {
            // Opens a FileStream
            using (var outStream = File.OpenWrite(filePath))
            {
                var reader = new BinaryReader(new MemoryStream(patch));
                var writer = new BinaryWriter(outStream);

                // Amount of blocks
                int blockCount = reader.ReadInt32();
                for (int i = 0; i < blockCount; ++i)
                {
                    // Seek to the start of the block is going to written at (offset)
                    writer.Seek(reader.ReadInt32(), SeekOrigin.Begin);
                    // Gets the size of the current block
                    int blockSize = reader.ReadInt32();
                    // Writes the block to the stream
                    writer.Write(reader.ReadBytes(blockSize));
                }
                // Closes the reader
                reader.Close();
            }
        }

        public static void PatchStream(byte[] patch, Stream stream)
        {
            var reader = new BinaryReader(new MemoryStream(patch));
            var writer = new BinaryWriter(stream);

            // Amount of blocks
            int blockCount = reader.ReadInt32();
            for (int i = 0; i < blockCount; ++i)
            {
                // Seek to the start of the block is going to written at (offset)
                writer.Seek(reader.ReadInt32(), SeekOrigin.Begin);
                // Gets the size of the current block
                int blockSize = reader.ReadInt32();
                // Writes the block to the stream
                writer.Write(reader.ReadBytes(blockSize));
            }
            // Closes the reader
            reader.Close();
        }

        public static bool CheckFile(byte[] patch, string filePath)
        {
            // Opens a FileStream
            using (var exeStream = File.OpenRead(filePath))
            {
                var reader = new BinaryReader(new MemoryStream(patch));
                var exeReader = new BinaryReader(exeStream);

                // Amount of blocks
                int blockCount = reader.ReadInt32();
                for (int i = 0; i < blockCount; ++i)
                {
                    // Seek to the start of the block
                    exeReader.BaseStream.Position = reader.ReadInt32();
                    // Gets the size of the current block
                    int blockSize = reader.ReadInt32();
                    // Checks the block with the exe
                    var buf = reader.ReadBytes(blockSize);
                    if (!buf.SequenceEqual(exeReader.ReadBytes(blockSize)))
                        return false;
                }
                // Closes the reader
                reader.Close();
            }
            return true;
        }

        public static bool CheckFile(byte[] patch, Stream stream)
        {
            // Opens a FileStream
            var reader = new BinaryReader(new MemoryStream(patch));
            var exeReader = new BinaryReader(stream);

            // Amount of blocks
            int blockCount = reader.ReadInt32();
            for (int i = 0; i < blockCount; ++i)
            {
                // Seek to the start of the block
                exeReader.BaseStream.Position = reader.ReadInt32();
                // Gets the size of the current block
                int blockSize = reader.ReadInt32();
                // Checks the block with the exe
                var buf = reader.ReadBytes(blockSize);
                if (!buf.SequenceEqual(exeReader.ReadBytes(blockSize)))
                    return false;
            }
            // Closes the reader
            reader.Close();
            return true;
        }


        // Sorry this is horrible, I can't even understand my own code
        public static void CreatePatch(string unModfiedPath, string modfiedPath, string outPath)
        {
            var changes = new Dictionary<int, byte[]>();
            var unmodfiedStream = File.ReadAllBytes(unModfiedPath);
            var modfiedStream = File.ReadAllBytes(modfiedPath);
            var outPatchStream = File.OpenWrite(outPath);

            int pos = -1;
            var bytes = new List<byte>();
            for (int i = 0; i < unmodfiedStream.Length; ++i)
            {
                if (unmodfiedStream[i] != modfiedStream[i])
                {
                    if (pos == -1)
                        pos = i;
                    bytes.Add(modfiedStream[i]);
                }
                else
                {
                    if (pos != -1)
                    {
                        changes.Add(pos, bytes.ToArray());
                        bytes.Clear();
                        pos = -1;
                    }
                }

            }
            outPatchStream.Write(BitConverter.GetBytes(changes.ToList().Count), 0, sizeof(int));
            for (int i = 0; i < changes.Count; ++i)
            {
                var value = changes.ToList()[i].Value;
                outPatchStream.Write(BitConverter.GetBytes(changes.ToList()[i].Key), 0, sizeof(int));
                outPatchStream.Write(BitConverter.GetBytes(value.Length), 0, sizeof(int));
                outPatchStream.Write(value, 0, value.Length);
            }
            outPatchStream.Close();
        }

        public class Patch
        {
            public string PatchName;
            public byte[] PatchEnable;
            public byte[] PatchDisable;

            public Patch(string patchName, string filenameEnable, string filenameDisable)
            {
                PatchName = patchName;
                PatchEnable = File.ReadAllBytes(filenameEnable);
                PatchDisable = File.ReadAllBytes(filenameDisable);
            }

            public Patch(string patchName, byte[] patchEnable, byte[] patchDisable)
            {
                PatchName = patchName;
                PatchEnable = patchEnable;
                PatchDisable = patchDisable;
            }

            public override string ToString()
            {
                return PatchName;
            }
        }
    }
}
