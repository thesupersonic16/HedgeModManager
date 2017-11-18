using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

using SonicAudioLib.IO;
using SonicAudioLib.CriMw;

namespace SonicAudioLib.Archives
{
    public class CriCpkEntry : EntryBase
    {
        private DateTime updateDateTime;

        public string DirectoryName { get; set; }
        public string Name { get; set; }
        public uint Id { get; set; }
        public string Comment { get; set; }
        public bool IsCompressed { get; set; }
        public long UncompressedLength { get; set; }

        public DateTime UpdateDateTime
        {
            get
            {
                if (FilePath != null)
                {
                    return FilePath.LastWriteTime;
                }

                return updateDateTime;
            }

            set
            {
                updateDateTime = value;
            }
        }
    }

    public enum CriCpkMode
    {
        None = -1,
        FileNameIdAndGroup = 5,
        IdAndGroup = 4,
        FileNameAndGroup = 3,
        FileNameAndId = 2,
        FileName = 1,
        Id = 0,
    }

    public class CriCpkArchive : ArchiveBase<CriCpkEntry>
    {
        private ushort align = 1;
        private CriCpkMode mode = CriCpkMode.FileName;
        private bool enableMask = false;

        public ushort Align
        {
            get
            {
                return align;
            }

            set
            {
                if (align != 1)
                {
                    throw new NotImplementedException("Alignment is currently not implemented.");
                }

                align = value;
            }
        }

        public CriCpkMode Mode
        {
            get
            {
                return mode;
            }

            set
            {
                mode = value;
            }
        }

        public bool EnableMask
        {
            get
            {
                return enableMask;
            }

            set
            {
                enableMask = value;
            }
        }

        public string Comment { get; set; }

        public override void Read(Stream source)
        {
            using (CriTableReader reader = CriCpkSection.Open(source, source.Position, "CPK "))
            {
                reader.Read();

                bool isLatestVersion = reader.ContainsField("CpkMode");

                if (isLatestVersion)
                {
                    mode = (CriCpkMode)reader.GetUInt32("CpkMode");
                }

                else
                {
                    bool tocEnabled = reader.GetUInt64("TocOffset") > 0;
                    bool itocEnabled = reader.GetUInt64("ItocOffset") > 0;

                    if (tocEnabled && !itocEnabled)
                    {
                        mode = CriCpkMode.FileName;
                    }

                    else if (!tocEnabled && itocEnabled)
                    {
                        mode = CriCpkMode.Id;
                    }

                    else if (tocEnabled && itocEnabled)
                    {
                        mode = CriCpkMode.FileNameAndId;
                    }

                    else
                    {
                        mode = CriCpkMode.None;
                    }
                }

                // No need to waste time, stop right there.
                if (mode == CriCpkMode.None)
                {
                    return;
                }

                long tocPosition = (long)reader.GetUInt64("TocOffset");
                long itocPosition = (long)reader.GetUInt64("ItocOffset");
                long etocPosition = (long)reader.GetUInt64("EtocOffset");
                long contentPosition = (long)reader.GetUInt64("ContentOffset");

                align = reader.GetUInt16("Align");

                if (mode == CriCpkMode.FileName || mode == CriCpkMode.FileNameAndId)
                {
                    using (CriTableReader tocReader = CriCpkSection.Open(source, tocPosition, "TOC "))
                    using (CriTableReader etocReader = CriCpkSection.Open(source, etocPosition, "ETOC"))
                    {
                        while (tocReader.Read())
                        {
                            CriCpkEntry entry = new CriCpkEntry();
                            entry.DirectoryName = tocReader.GetString("DirName");
                            entry.Name = tocReader.GetString("FileName");
                            entry.Length = tocReader.GetUInt32("FileSize");
                            entry.Position = (long)tocReader.GetUInt64("FileOffset");
                            entry.Id = isLatestVersion ? tocReader.GetUInt32("ID") : tocReader.GetUInt32("Info");
                            entry.Comment = tocReader.GetString("UserString");
                            entry.UncompressedLength = tocReader.GetUInt32("ExtractSize");
                            entry.IsCompressed = entry.Length != entry.UncompressedLength;

                            if (contentPosition < tocPosition)
                            {
                                entry.Position += contentPosition;
                            }

                            else
                            {
                                entry.Position += tocPosition;
                            }

                            entry.Position = Helpers.Align(entry.Position, align);

                            etocReader.MoveToRow(tocReader.CurrentRow);
                            entry.UpdateDateTime = DateTimeFromCpkDateTime(etocReader.GetUInt64("UpdateDateTime"));

                            entries.Add(entry);
                        }
                    }

                    if (mode == CriCpkMode.FileNameAndId && isLatestVersion)
                    {
                        using (CriTableReader itocReader = CriCpkSection.Open(source, itocPosition, "ITOC"))
                        {
                            while (itocReader.Read())
                            {
                                entries[itocReader.GetInt32("TocIndex")].Id = (uint)itocReader.GetInt32("ID");
                            }
                        }
                    }
                }

                else if (mode == CriCpkMode.Id)
                {
                    using (CriTableReader itocReader = CriCpkSection.Open(source, itocPosition, "ITOC"))
                    {
                        while (itocReader.Read())
                        {
                            if (itocReader.GetUInt32("FilesL") > 0)
                            {
                                using (CriTableReader dataReader = itocReader.GetTableReader("DataL"))
                                {
                                    while (dataReader.Read())
                                    {
                                        CriCpkEntry entry = new CriCpkEntry();
                                        entry.Id = dataReader.GetUInt16("ID");
                                        entry.Length = dataReader.GetUInt16("FileSize");
                                        entry.UncompressedLength = dataReader.GetUInt16("ExtractSize");
                                        entry.IsCompressed = entry.Length != entry.UncompressedLength;

                                        entries.Add(entry);
                                    }
                                }
                            }

                            if (itocReader.GetUInt32("FilesH") > 0)
                            {
                                using (CriTableReader dataReader = itocReader.GetTableReader("DataH"))
                                {
                                    while (dataReader.Read())
                                    {
                                        CriCpkEntry entry = new CriCpkEntry();
                                        entry.Id = dataReader.GetUInt16("ID");
                                        entry.Length = dataReader.GetUInt32("FileSize");
                                        entry.UncompressedLength = dataReader.GetUInt32("ExtractSize");
                                        entry.IsCompressed = entry.Length != entry.UncompressedLength;

                                        entries.Add(entry);
                                    }
                                }
                            }
                        }
                    }

                    long entryPosition = contentPosition;
                    foreach (CriCpkEntry entry in entries.OrderBy(entry => entry.Id))
                    {
                        entryPosition = Helpers.Align(entryPosition, align);

                        entry.Position = entryPosition;
                        entryPosition += entry.Length;
                    }
                }

                else
                {
                    throw new NotImplementedException($"Unimplemented CPK mode ({mode})");
                }

                Comment = reader.GetString("Comment");
            }
        }

        public override void Write(Stream destination)
        {
            string GetToolVersion()
            {
                AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();
                return $"{assemblyName.Name}, {assemblyName.Version.ToString()}";
            }

            DataPool vldPool = new DataPool(Align, 2048);
            vldPool.ProgressChanged += OnProgressChanged;

            using (CriCpkSection cpkSection = new CriCpkSection(destination, "CPK ", enableMask))
            {
                cpkSection.Writer.WriteStartTable("CpkHeader");

                cpkSection.Writer.WriteField("UpdateDateTime", typeof(ulong));
                cpkSection.Writer.WriteField("FileSize", typeof(ulong));
                cpkSection.Writer.WriteField("ContentOffset", typeof(ulong));
                cpkSection.Writer.WriteField("ContentSize", typeof(ulong));

                if (mode == CriCpkMode.FileName || mode == CriCpkMode.FileNameAndId)
                {
                    cpkSection.Writer.WriteField("TocOffset", typeof(ulong));
                    cpkSection.Writer.WriteField("TocSize", typeof(ulong));
                    cpkSection.Writer.WriteField("TocCrc", typeof(uint), null);
                    cpkSection.Writer.WriteField("EtocOffset", typeof(ulong));
                    cpkSection.Writer.WriteField("EtocSize", typeof(ulong));
                }

                else
                {
                    cpkSection.Writer.WriteField("TocOffset", typeof(ulong), null);
                    cpkSection.Writer.WriteField("TocSize", typeof(ulong), null);
                    cpkSection.Writer.WriteField("TocCrc", typeof(uint), null);
                    cpkSection.Writer.WriteField("EtocOffset", typeof(ulong), null);
                    cpkSection.Writer.WriteField("EtocSize", typeof(ulong), null);
                }

                if (mode == CriCpkMode.Id || mode == CriCpkMode.FileNameAndId)
                {
                    cpkSection.Writer.WriteField("ItocOffset", typeof(ulong));
                    cpkSection.Writer.WriteField("ItocSize", typeof(ulong));
                    cpkSection.Writer.WriteField("ItocCrc", typeof(uint), null);
                }

                else
                {
                    cpkSection.Writer.WriteField("ItocOffset", typeof(ulong), null);
                    cpkSection.Writer.WriteField("ItocSize", typeof(ulong), null);
                    cpkSection.Writer.WriteField("ItocCrc", typeof(uint), null);
                }

                cpkSection.Writer.WriteField("GtocOffset", typeof(ulong), null);
                cpkSection.Writer.WriteField("GtocSize", typeof(ulong), null);
                cpkSection.Writer.WriteField("GtocCrc", typeof(uint), null);

                cpkSection.Writer.WriteField("EnabledPackedSize", typeof(ulong));
                cpkSection.Writer.WriteField("EnabledDataSize", typeof(ulong));
                cpkSection.Writer.WriteField("TotalDataSize", typeof(ulong), null);

                cpkSection.Writer.WriteField("Tocs", typeof(uint), null);
                cpkSection.Writer.WriteField("Files", typeof(uint));
                cpkSection.Writer.WriteField("Groups", typeof(uint));
                cpkSection.Writer.WriteField("Attrs", typeof(uint));
                cpkSection.Writer.WriteField("TotalFiles", typeof(uint), null);
                cpkSection.Writer.WriteField("Directories", typeof(uint), null);
                cpkSection.Writer.WriteField("Updates", typeof(uint), null);

                cpkSection.Writer.WriteField("Version", typeof(ushort));
                cpkSection.Writer.WriteField("Revision", typeof(ushort));
                cpkSection.Writer.WriteField("Align", typeof(ushort));
                cpkSection.Writer.WriteField("Sorted", typeof(ushort));
                cpkSection.Writer.WriteField("EID", typeof(ushort), null);

                cpkSection.Writer.WriteField("CpkMode", typeof(uint));
                cpkSection.Writer.WriteField("Tvers", typeof(string));

                if (!string.IsNullOrEmpty(Comment))
                {
                    cpkSection.Writer.WriteField("Comment", typeof(string));
                }

                else
                {
                    cpkSection.Writer.WriteField("Comment", typeof(string), null);
                }

                cpkSection.Writer.WriteField("Codec", typeof(uint));
                cpkSection.Writer.WriteField("DpkItoc", typeof(uint));

                MemoryStream tocMemoryStream = null;
                MemoryStream itocMemoryStream = null;
                MemoryStream etocMemoryStream = null;

                if (mode == CriCpkMode.FileName || mode == CriCpkMode.FileNameAndId)
                {
                    tocMemoryStream = new MemoryStream();
                    etocMemoryStream = new MemoryStream();

                    var orderedEntries = entries.OrderBy(entry => entry.Name).ToList();

                    using (CriCpkSection tocSection = new CriCpkSection(tocMemoryStream, "TOC ", enableMask))
                    using (CriCpkSection etocSection = new CriCpkSection(etocMemoryStream, "ETOC", enableMask))
                    {
                        tocSection.Writer.WriteStartTable("CpkTocInfo");

                        tocSection.Writer.WriteField("DirName", typeof(string));
                        tocSection.Writer.WriteField("FileName", typeof(string));
                        tocSection.Writer.WriteField("FileSize", typeof(uint));
                        tocSection.Writer.WriteField("ExtractSize", typeof(uint));
                        tocSection.Writer.WriteField("FileOffset", typeof(ulong));
                        tocSection.Writer.WriteField("ID", typeof(uint));
                        tocSection.Writer.WriteField("UserString", typeof(string));

                        etocSection.Writer.WriteStartTable("CpkEtocInfo");

                        etocSection.Writer.WriteField("UpdateDateTime", typeof(ulong));
                        etocSection.Writer.WriteField("LocalDir", typeof(string));

                        foreach (CriCpkEntry entry in orderedEntries)
                        {
                            tocSection.Writer.WriteRow(true,
                                (entry.DirectoryName).Replace('\\', '/'),
                                entry.Name,
                                (uint)entry.Length,
                                (uint)entry.Length,
                                (ulong)(vldPool.Length - 2048),
                                entry.Id,
                                entry.Comment);

                            etocSection.Writer.WriteRow(true,
                                CpkDateTimeFromDateTime(entry.UpdateDateTime),
                                entry.DirectoryName);

                            vldPool.Put(entry.FilePath);
                        }

                        tocSection.Writer.WriteEndTable();
                        etocSection.Writer.WriteEndTable();
                    }

                    if (mode == CriCpkMode.FileNameAndId)
                    {
                        itocMemoryStream = new MemoryStream();

                        using (CriCpkSection itocSection = new CriCpkSection(itocMemoryStream, "ITOC", enableMask))
                        {
                            itocSection.Writer.WriteStartTable("CpkExtendId");

                            itocSection.Writer.WriteField("ID", typeof(int));
                            itocSection.Writer.WriteField("TocIndex", typeof(int));

                            foreach (CriCpkEntry entry in orderedEntries)
                            {
                                itocSection.Writer.WriteRow(true,
                                    (int)entry.Id,
                                    orderedEntries.IndexOf(entry));
                            }

                            itocSection.Writer.WriteEndTable();
                        }
                    }
                }

                else if (mode == CriCpkMode.Id)
                {
                    itocMemoryStream = new MemoryStream();

                    using (CriCpkSection itocSection = new CriCpkSection(itocMemoryStream, "ITOC", enableMask))
                    {
                        itocSection.Writer.WriteStartTable("CpkItocInfo");

                        itocSection.Writer.WriteField("FilesL", typeof(uint));
                        itocSection.Writer.WriteField("FilesH", typeof(uint));
                        itocSection.Writer.WriteField("DataL", typeof(byte[]));
                        itocSection.Writer.WriteField("DataH", typeof(byte[]));

                        List<CriCpkEntry> filesL = entries.Where(entry => entry.Length < ushort.MaxValue).ToList();
                        List<CriCpkEntry> filesH = entries.Where(entry => entry.Length > ushort.MaxValue).ToList();

                        itocSection.Writer.WriteStartRow();

                        using (MemoryStream dataMemoryStream = new MemoryStream())
                        using (CriTableWriter dataWriter = CriTableWriter.Create(dataMemoryStream))
                        {
                            dataWriter.WriteStartTable("CpkItocL");

                            dataWriter.WriteField("ID", typeof(ushort));
                            dataWriter.WriteField("FileSize", typeof(ushort));
                            dataWriter.WriteField("ExtractSize", typeof(ushort));

                            foreach (CriCpkEntry entry in filesL)
                            {
                                dataWriter.WriteRow(true,
                                    (ushort)entry.Id,
                                    (ushort)entry.Length,
                                    (ushort)entry.Length);
                            }

                            dataWriter.WriteEndTable();

                            itocSection.Writer.WriteValue("DataL", dataMemoryStream.ToArray());
                        }

                        using (MemoryStream dataMemoryStream = new MemoryStream())
                        using (CriTableWriter dataWriter = CriTableWriter.Create(dataMemoryStream))
                        {
                            dataWriter.WriteStartTable("CpkItocH");

                            dataWriter.WriteField("ID", typeof(ushort));
                            dataWriter.WriteField("FileSize", typeof(uint));
                            dataWriter.WriteField("ExtractSize", typeof(uint));

                            foreach (CriCpkEntry entry in filesH)
                            {
                                dataWriter.WriteRow(true,
                                    (ushort)entry.Id,
                                    (uint)entry.Length,
                                    (uint)entry.Length);
                            }

                            dataWriter.WriteEndTable();

                            itocSection.Writer.WriteValue("DataH", dataMemoryStream.ToArray());
                        }

                        itocSection.Writer.WriteValue("FilesL", (uint)filesL.Count);
                        itocSection.Writer.WriteValue("FilesH", (uint)filesH.Count);
                        itocSection.Writer.WriteEndRow();
                        itocSection.Writer.WriteEndTable();
                    }

                    foreach (CriCpkEntry entry in entries.OrderBy(entry => entry.Id))
                    {
                        vldPool.Put(entry.FilePath);
                    }
                }

                else
                {
                    throw new NotImplementedException($"Unimplemented CPK mode ({mode})");
                }

                cpkSection.Writer.WriteStartRow();
                cpkSection.Writer.WriteValue("UpdateDateTime", CpkDateTimeFromDateTime(DateTime.Now));

                cpkSection.Writer.WriteValue("ContentOffset", (ulong)2048);
                cpkSection.Writer.WriteValue("ContentSize", (ulong)(vldPool.Length - 2048));

                if (tocMemoryStream != null)
                {
                    cpkSection.Writer.WriteValue("TocOffset", (ulong)vldPool.Put(tocMemoryStream));
                    cpkSection.Writer.WriteValue("TocSize", (ulong)tocMemoryStream.Length);
                }

                if (itocMemoryStream != null)
                {
                    cpkSection.Writer.WriteValue("ItocOffset", (ulong)vldPool.Put(itocMemoryStream));
                    cpkSection.Writer.WriteValue("ItocSize", (ulong)itocMemoryStream.Length);
                }

                if (etocMemoryStream != null)
                {
                    cpkSection.Writer.WriteValue("EtocOffset", (ulong)vldPool.Put(etocMemoryStream));
                    cpkSection.Writer.WriteValue("EtocSize", (ulong)etocMemoryStream.Length);
                }

                long totalDataSize = 0;
                foreach (CriCpkEntry entry in entries)
                {
                    totalDataSize += entry.Length;
                }

                cpkSection.Writer.WriteValue("EnabledPackedSize", totalDataSize);
                cpkSection.Writer.WriteValue("EnabledDataSize", totalDataSize);

                cpkSection.Writer.WriteValue("Files", (uint)entries.Count);

                cpkSection.Writer.WriteValue("Version", (ushort)7);
                cpkSection.Writer.WriteValue("Revision", (ushort)2);
                cpkSection.Writer.WriteValue("Align", Align);
                cpkSection.Writer.WriteValue("Sorted", (ushort)1);

                cpkSection.Writer.WriteValue("CpkMode", (uint)mode);
                cpkSection.Writer.WriteValue("Tvers", GetToolVersion());
                cpkSection.Writer.WriteValue("Comment", Comment);

                cpkSection.Writer.WriteValue("FileSize", (ulong)vldPool.Length);

                cpkSection.Writer.WriteEndRow();
                cpkSection.Writer.WriteEndTable();
            }

            DataStream.Pad(destination, 2042);
            DataStream.WriteCString(destination, "(c)CRI", 6);

            vldPool.Write(destination);
        }

        public CriCpkEntry GetById(uint id)
        {
            return entries.FirstOrDefault(entry => (entry.Id == id));
        }

        public CriCpkEntry GetByName(string name)
        {
            return entries.FirstOrDefault(entry => (entry.Name == name));
        }

        public CriCpkEntry GetByPath(string path)
        {
            string correctedPath = path.Replace('\\', '/');

            return entries.FirstOrDefault(entry =>
            {
                string search = string.Empty;
                if (!string.IsNullOrEmpty(entry.DirectoryName))
                {
                    search += $"{entry.DirectoryName.Replace('\\', '/')}/";
                }

                search += entry.Name;

                return search == correctedPath;
            });
        }

        public static void Decompress(byte[] source, byte[] destination)
        {
            if (Encoding.ASCII.GetString(source, 0, 8) != "CRILAYLA")
            {
                throw new InvalidDataException("'CRILAYLA' signature could not be found.");
            }

            int uncompressedLength = BitConverter.ToInt32(source, 8);
            int compressedLength = BitConverter.ToInt32(source, 12);

            Array.Copy(source, 16 + compressedLength, destination, 0, 256);

            int srcEnd = 16 + compressedLength - 1;
            int dstEnd = 256 + uncompressedLength - 1;
            int dstOut = 0;

            int bitData = 0;
            int bitNum = 0;

            int ReadBits(int count)
            {
                if (bitNum < count)
                {
                    int byteNum = ((24 - bitNum) >> 3) + 1;
                    bitNum += byteNum * 8;

                    while (byteNum > 0)
                    {
                        bitData = source[srcEnd--] | bitData << 8;
                        byteNum--;
                    }
                }

                bitNum -= count;
                return (bitData >> bitNum) & ((1 << count) - 1);
            }

            while (dstOut < uncompressedLength)
            {
                if (ReadBits(1) != 0)
                {
                    int position = dstEnd - dstOut + ReadBits(13) + 3;
                    int length = 3 + ReadBits(2);

                    if (length == 3 + 3)
                    {
                        length += ReadBits(3);

                        if (length == 3 + 3 + 7)
                        {
                            length += ReadBits(5);

                            if (length == 3 + 3 + 7 + 31)
                            {
                                int bits;

                                do
                                {
                                    bits = ReadBits(8);
                                    length += bits;
                                } while (bits == 255);
                            }
                        }
                    }

                    dstOut += length;
                    Array.Copy(destination, position - length + 1, destination, dstEnd - dstOut + 1, length);
                }

                else
                {
                    destination[dstEnd - dstOut++] = (byte)ReadBits(8);
                }
            }
        }

        private DateTime DateTimeFromCpkDateTime(ulong dateTime)
        {
            return dateTime > 0 ? 
                new DateTime(
                    (int)(dateTime >> 48),          // Year (ushort)
                    (int)(dateTime >> 40) & 0xFF,   // Month (byte)
                    (int)(dateTime >> 32) & 0xFF,   // Day (byte)
                    (int)(dateTime >> 24) & 0xFF,   // Hour (byte)
                    (int)(dateTime >> 16) & 0xFF,   // Minute (byte)
                    (int)(dateTime >> 8) & 0xFF)    // Second (byte)
                : new DateTime();
        }

        private ulong CpkDateTimeFromDateTime(DateTime dateTime)
        {
            return 
                (ulong)dateTime.Year << 48 | 
                (ulong)dateTime.Month << 40 |
                (ulong)dateTime.Day << 32 |
                (ulong)dateTime.Hour << 24 |
                (ulong)dateTime.Minute << 16 |
                (ulong)dateTime.Second << 8;
        }

        private class CriCpkSection : IDisposable
        {
            private Stream destination;
            private long headerPosition;

            private CriTableWriter writer;

            public CriTableWriter Writer
            {
                get
                {
                    return writer;
                }
            }

            public void Dispose()
            {
                writer.Dispose();

                long position = destination.Position;
                uint length = (uint)(position - (headerPosition) - 16);

                destination.Seek(headerPosition + 8, SeekOrigin.Begin);
                DataStream.WriteUInt32(destination, length);

                destination.Seek(position, SeekOrigin.Begin);
            }

            public static CriTableReader Open(Stream source, long position, string expectedSignature)
            {
                source.Seek(position, SeekOrigin.Begin);

                if (DataStream.ReadCString(source, 4) != expectedSignature)
                {
                    throw new InvalidDataException($"'{expectedSignature}' signature could not be found.");
                }

                uint flag = DataStream.ReadUInt32(source);
                uint tableLength = DataStream.ReadUInt32(source);
                uint unknown = DataStream.ReadUInt32(source);

                return CriTableReader.Create(new SubStream(source, source.Position, tableLength));
            }

            public CriCpkSection(Stream destination, string signature, bool enableMask)
            {
                this.destination = destination;
                headerPosition = destination.Position;

                DataStream.WriteCString(destination, signature, 4);
                DataStream.WriteUInt32(destination, 0xFF);
                destination.Seek(8, SeekOrigin.Current);

                writer = CriTableWriter.Create(destination, new CriTableWriterSettings()
                {
                    LeaveOpen = true,
                    EnableMask = enableMask,
                    MaskXor = 25951,
                    MaskXorMultiplier = 16661
                });
            }
        }
    }
}