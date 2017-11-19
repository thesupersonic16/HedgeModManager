using SonicAudioLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;

namespace SonicAudioLib.CriMw
{
    public class CriTableReader : IDisposable
    {
        private List<CriTableField> fields;
        private Stream source;
        private CriTableHeader header;
        private Encoding encoding;
        private long rowIndex = -1;
        private long headerPosition;
        private bool leaveOpen;

        public object this[int fieldIndex]
        {
            get
            {
                return GetValue(fieldIndex);
            }
        }

        public object this[string fieldName]
        {
            get
            {
                return GetValue(fieldName);
            }
        }

        public ushort NumberOfFields
        {
            get
            {
                return header.FieldCount;
            }
        }

        public uint NumberOfRows
        {
            get
            {
                return header.RowCount;
            }
        }

        public string TableName
        {
            get
            {
                return header.TableName;
            }
        }

        public long CurrentRow
        {
            get
            {
                return rowIndex;
            }
        }

        public Stream SourceStream
        {
            get
            {
                return source;
            }
        }

        public Encoding EncodingType
        {
            get
            {
                return encoding;
            }
        }

        private void ReadTable()
        {
            headerPosition = source.Position;

            if (!(header.Signature = DataStream.ReadBytes(source, 4)).SequenceEqual(CriTableHeader.SignatureBytes))
            {
                MemoryStream unmaskedSource = new MemoryStream();

                CriTableMasker.FindKeys(header.Signature, out uint x, out uint m);

                source.Seek(headerPosition, SeekOrigin.Begin);
                CriTableMasker.Mask(source, unmaskedSource, source.Length, x, m);

                // Close the old stream
                if (!leaveOpen)
                {
                    source.Close();
                }

                source = unmaskedSource;
                source.Seek(4, SeekOrigin.Begin);
            }

            header.Length = DataStream.ReadUInt32BE(source) + 0x8;
            header.UnknownByte = DataStream.ReadByte(source);
            header.EncodingType = DataStream.ReadByte(source);

            if (header.UnknownByte != 0)
            {
                throw new InvalidDataException($"Invalid byte ({header.UnknownByte}. Please report this error with the file(s).");
            }

            switch (header.EncodingType)
            {
                case CriTableHeader.EncodingTypeShiftJis:
                    encoding = Encoding.GetEncoding("shift-jis");
                    break;

                case CriTableHeader.EncodingTypeUtf8:
                    encoding = Encoding.UTF8;
                    break;

                default:
                    throw new InvalidDataException($"Unknown encoding type ({header.EncodingType}). Please report this error with the file(s).");
            }

            header.RowsPosition = (ushort)(DataStream.ReadUInt16BE(source) + 0x8);
            header.StringPoolPosition = DataStream.ReadUInt32BE(source) + 0x8;
            header.DataPoolPosition = DataStream.ReadUInt32BE(source) + 0x8;
            header.TableName = ReadString();
            header.FieldCount = DataStream.ReadUInt16BE(source);
            header.RowLength = DataStream.ReadUInt16BE(source);
            header.RowCount = DataStream.ReadUInt32BE(source);

            uint offset = 0;
            for (ushort i = 0; i < header.FieldCount; i++)
            {
                CriTableField field = new CriTableField();
                field.Flag = (CriFieldFlag)DataStream.ReadByte(source);

                if (field.Flag.HasFlag(CriFieldFlag.Name))
                {
                    field.Name = ReadString();
                }

                if (field.Flag.HasFlag(CriFieldFlag.DefaultValue))
                {
                    if (field.Flag.HasFlag(CriFieldFlag.Data))
                    {
                        field.Position = DataStream.ReadUInt32BE(source);
                        field.Length = DataStream.ReadUInt32BE(source);
                    }

                    else
                    {
                        field.Value = ReadValue(field.Flag);
                    }
                }

                // Not even per row, and not even constant value? Then there's no storage.
                else if (!field.Flag.HasFlag(CriFieldFlag.RowStorage) && !field.Flag.HasFlag(CriFieldFlag.DefaultValue))
                {
                    field.Value = CriField.NullValues[(byte)field.Flag & 0x0F];
                }

                // Row storage, calculate the offset
                if (field.Flag.HasFlag(CriFieldFlag.RowStorage))
                {
                    field.Offset = offset;

                    switch (field.Flag & CriFieldFlag.TypeMask)
                    {
                        case CriFieldFlag.Byte:
                        case CriFieldFlag.SByte:
                            offset += 1;
                            break;
                        case CriFieldFlag.Int16:
                        case CriFieldFlag.UInt16:
                            offset += 2;
                            break;
                        case CriFieldFlag.Int32:
                        case CriFieldFlag.UInt32:
                        case CriFieldFlag.Single:
                        case CriFieldFlag.String:
                            offset += 4;
                            break;
                        case CriFieldFlag.Int64:
                        case CriFieldFlag.UInt64:
                        case CriFieldFlag.Double:
                        case CriFieldFlag.Data:
                            offset += 8;
                            break;
                    }
                }

                fields.Add(field);
            }
        }

        public string GetFieldName(int fieldIndex)
        {
            return fields[fieldIndex].Name;
        }

        public Type GetFieldType(int fieldIndex)
        {
            return CriField.FieldTypes[(byte)fields[fieldIndex].Flag & 0x0F];
        }

        public Type GetFieldType(string fieldName)
        {
            return CriField.FieldTypes[(byte)fields[GetFieldIndex(fieldName)].Flag & 0x0F];
        }

        public object GetFieldValue(int fieldIndex)
        {
            return fields[fieldIndex].Value;
        }

        internal CriFieldFlag GetFieldFlag(string fieldName)
        {
            return fields[GetFieldIndex(fieldName)].Flag;
        }

        internal CriFieldFlag GetFieldFlag(int fieldIndex)
        {
            return fields[fieldIndex].Flag;
        }

        public object GetFieldValue(string fieldName)
        {
            return fields[GetFieldIndex(fieldName)].Value;
        }

        public CriField GetField(int fieldIndex)
        {
            return new CriField(GetFieldName(fieldIndex), GetFieldType(fieldIndex), GetFieldValue(fieldIndex));
        }

        public CriField GetField(string fieldName)
        {
            return new CriField(fieldName, GetFieldType(fieldName), GetFieldValue(fieldName));
        }

        public int GetFieldIndex(string fieldName)
        {
            return fields.FindIndex(field => field.Name == fieldName);
        }

        public bool ContainsField(string fieldName)
        {
            return fields.Exists(field => field.Name == fieldName);
        }
        
        private void GoToValue(int fieldIndex)
        {
            source.Seek(headerPosition + header.RowsPosition + (header.RowLength * rowIndex) + fields[fieldIndex].Offset, SeekOrigin.Begin);
        }

        public bool Read()
        {
            if (rowIndex + 1 >= header.RowCount)
            {
                return false;
            }

            rowIndex++;
            return true;
        }

        public bool MoveToRow(long rowIndex)
        {
            if (rowIndex >= header.RowCount)
            {
                return false;
            }

            this.rowIndex = rowIndex;
            return true;
        }

        public object[] GetValueArray()
        {
            object[] values = new object[header.FieldCount];

            for (int i = 0; i < header.FieldCount; i++)
            {
                if (fields[i].Flag.HasFlag(CriFieldFlag.Data))
                {
                    values[i] = GetData(i);
                }

                else
                {
                    values[i] = GetValue(i);
                }
            }

            return values;
        }

        public IEnumerable GetValues()
        {
            for (int i = 0; i < header.FieldCount; i++)
            {
                yield return GetValue(i);
            }
        }

        public object GetValue(int fieldIndex)
        {
            if (fieldIndex < 0 || fieldIndex >= fields.Count)
            {
                return null;
            }

            if (!fields[fieldIndex].Flag.HasFlag(CriFieldFlag.RowStorage))
            {
                if (fields[fieldIndex].Flag.HasFlag(CriFieldFlag.Data))
                {
                    return new SubStream(source, 0, 0);
                }

                return fields[fieldIndex].Value;
            }

            GoToValue(fieldIndex);
            return ReadValue(fields[fieldIndex].Flag);
        }

        public object GetValue(string fieldName)
        {
            return GetValue(GetFieldIndex(fieldName));
        }

        public T GetValue<T>(int fieldIndex)
        {
            return (T)GetValue(fieldIndex);
        }

        public T GetValue<T>(string fieldName)
        {
            return (T)GetValue(fieldName);
        }

        public byte GetByte(int fieldIndex)
        {
            return (byte)GetValue(fieldIndex);
        }

        public byte GetByte(string fieldName)
        {
            return (byte)GetValue(fieldName);
        }

        public sbyte GetSByte(int fieldIndex)
        {
            return (sbyte)GetValue(fieldIndex);
        }

        public sbyte GetSByte(string fieldName)
        {
            return (sbyte)GetValue(fieldName);
        }

        public ushort GetUInt16(int fieldIndex)
        {
            return (ushort)GetValue(fieldIndex);
        }

        public ushort GetUInt16(string fieldName)
        {
            return (ushort)GetValue(fieldName);
        }

        public short GetInt16(int fieldIndex)
        {
            return (short)GetValue(fieldIndex);
        }

        public short GetInt16(string fieldName)
        {
            return (short)GetValue(fieldName);
        }

        public uint GetUInt32(int fieldIndex)
        {
            return (uint)GetValue(fieldIndex);
        }

        public uint GetUInt32(string fieldName)
        {
            return (uint)GetValue(fieldName);
        }

        public int GetInt32(int fieldIndex)
        {
            return (int)GetValue(fieldIndex);
        }

        public int GetInt32(string fieldName)
        {
            return (int)GetValue(fieldName);
        }

        public ulong GetUInt64(int fieldIndex)
        {
            return (ulong)GetValue(fieldIndex);
        }

        public ulong GetUInt64(string fieldName)
        {
            return (ulong)GetValue(fieldName);
        }

        public long GetInt64(int fieldIndex)
        {
            return (long)GetValue(fieldIndex);
        }

        public long GetInt64(string fieldName)
        {
            return (long)GetValue(fieldName);
        }

        public float GetSingle(int fieldIndex)
        {
            return (float)GetValue(fieldIndex);
        }

        public float GetSingle(string fieldName)
        {
            return (float)GetValue(fieldName);
        }

        public double GetDouble(int fieldIndex)
        {
            return (double)GetValue(fieldIndex);
        }

        public double GetDouble(string fieldName)
        {
            return (double)GetValue(fieldName);
        }

        public string GetString(int fieldIndex)
        {
            return (string)GetValue(fieldIndex);
        }

        public string GetString(string fieldName)
        {
            return (string)GetValue(fieldName);
        }

        public SubStream GetSubStream(int fieldIndex)
        {
            return (SubStream)GetValue(fieldIndex);
        }

        public SubStream GetSubStream(string fieldName)
        {
            return (SubStream)GetValue(fieldName);
        }

        public byte[] GetData(int fieldIndex)
        {
            return GetSubStream(fieldIndex).ToArray();
        }

        public byte[] GetData(string fieldName)
        {
            return GetData(GetFieldIndex(fieldName));
        }

        public CriTableReader GetTableReader(string fieldName)
        {
            return new CriTableReader(GetSubStream(fieldName), false);
        }

        public CriTableReader GetTableReader(int fieldIndex)
        {
            return new CriTableReader(GetSubStream(fieldIndex), false);
        }

        public uint GetLength(int fieldIndex)
        {
            if (fieldIndex < 0 || fieldIndex >= fields.Count)
            {
                return 0;
            }

            if (!fields[fieldIndex].Flag.HasFlag(CriFieldFlag.RowStorage))
            {
                return fields[fieldIndex].Length;
            }

            GoToValue(fieldIndex);

            source.Seek(4, SeekOrigin.Current);
            return DataStream.ReadUInt32BE(source);
        }

        public uint GetLength(string fieldName)
        {
            return GetLength(GetFieldIndex(fieldName));
        }

        public uint GetPosition(int fieldIndex)
        {
            if (fieldIndex < 0 || fieldIndex >= fields.Count)
            {
                return 0;
            }

            if (!fields[fieldIndex].Flag.HasFlag(CriFieldFlag.RowStorage))
            {
                return fields[fieldIndex].Position;
            }

            GoToValue(fieldIndex);
            return (uint)(headerPosition + header.DataPoolPosition + DataStream.ReadUInt32BE(source));
        }

        public uint GetPosition(string fieldName)
        {
            return GetPosition(GetFieldIndex(fieldName));
        }
        
        public bool GetBoolean(int fieldIndex)
        {
            return (byte)GetValue(fieldIndex) > 0;
        }

        public bool GetBoolean(string fieldName)
        {
            return (byte)GetValue(fieldName) > 0;
        }

        public Guid GetGuid(int fieldIndex)
        {
            return (Guid)GetValue(fieldIndex);
        }

        public Guid GetGuid(string fieldName)
        {
            return (Guid)GetValue(fieldName);
        }

        private string ReadString()
        {
            uint stringPosition = DataStream.ReadUInt32BE(source);
            long previousPosition = source.Position;

            source.Seek(headerPosition + header.StringPoolPosition + stringPosition, SeekOrigin.Begin);
            string readString = DataStream.ReadCString(source, encoding);

            source.Seek(previousPosition, SeekOrigin.Begin);

            if (readString == StringPool.AdxBlankString || (readString == header.TableName && stringPosition == 0))
            {
                return string.Empty;
            }

            return readString;
        }

        private object ReadValue(CriFieldFlag fieldFlag)
        {
            switch (fieldFlag & CriFieldFlag.TypeMask)
            {
                case CriFieldFlag.Byte:
                    return DataStream.ReadByte(source);
                case CriFieldFlag.SByte:
                    return DataStream.ReadSByte(source);
                case CriFieldFlag.UInt16:
                    return DataStream.ReadUInt16BE(source);
                case CriFieldFlag.Int16:
                    return DataStream.ReadInt16BE(source);
                case CriFieldFlag.UInt32:
                    return DataStream.ReadUInt32BE(source);
                case CriFieldFlag.Int32:
                    return DataStream.ReadInt32BE(source);
                case CriFieldFlag.UInt64:
                    return DataStream.ReadUInt64BE(source);
                case CriFieldFlag.Int64:
                    return DataStream.ReadInt64BE(source);
                case CriFieldFlag.Single:
                    return DataStream.ReadSingleBE(source);
                case CriFieldFlag.Double:
                    return DataStream.ReadDoubleBE(source);
                case CriFieldFlag.String:
                    return ReadString();
                case CriFieldFlag.Data:
                    {
                        uint position = DataStream.ReadUInt32BE(source);
                        uint length = DataStream.ReadUInt32BE(source);

                        // Some ACB files have the length info set to zero for UTF table fields, so find the correct length
                        if (position > 0 && length == 0)
                        {
                            source.Seek(headerPosition + header.DataPoolPosition + position, SeekOrigin.Begin);

                            if (DataStream.ReadBytes(source, 4).SequenceEqual(CriTableHeader.SignatureBytes))
                            {
                                length = DataStream.ReadUInt32BE(source) + 8;
                            }
                        }

                        return new SubStream(source, headerPosition + header.DataPoolPosition + position, length);
                    }
                case CriFieldFlag.Guid:
                    return new Guid(DataStream.ReadBytes(source, 16));
            }

            return null;
        }

        public void Dispose()
        {
            fields.Clear();

            if (!leaveOpen)
            {
                source.Close();
            }
        }

        public static CriTableReader Create(byte[] sourceByteArray)
        {
            Stream source = new MemoryStream(sourceByteArray);
            return Create(source);
        }

        public static CriTableReader Create(string sourceFileName)
        {
            Stream source = File.OpenRead(sourceFileName);
            return Create(source);
        }

        public static CriTableReader Create(Stream source)
        {
            return Create(source, false);
        }

        public static CriTableReader Create(Stream source, bool leaveOpen)
        {
            return new CriTableReader(source, leaveOpen);
        }

        private CriTableReader(Stream source, bool leaveOpen)
        {
            this.source = source;
            header = new CriTableHeader();
            fields = new List<CriTableField>();
            this.leaveOpen = leaveOpen;

            ReadTable();
        }   
    }
}
