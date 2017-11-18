using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

using System.Linq;
using SonicAudioLib.IO;
using SonicAudioLib.FileBases;

namespace SonicAudioLib.CriMw
{
    public class CriTableWriter : IDisposable
    {
        public enum Status
        {
            Begin,
            Start,
            FieldCollection,
            Row,
            Idle,
            End,
        }

        private CriTableWriterSettings settings;
        private List<CriTableField> fields;
        private Stream destination;
        private CriTableHeader header;
        private DataPool vldPool;
        private StringPool stringPool;
        private uint headerPosition;

        private Status status = Status.Begin;

        public Status CurrentStatus
        {
            get
            {
                return status;
            }
        }

        public Stream DestinationStream
        {
            get
            {
                return destination;
            }
        }

        public void WriteStartTable()
        {
            WriteStartTable("(no name)");
        }

        public void WriteStartTable(string tableName)
        {
            if (status != Status.Begin)
            {
                throw new InvalidOperationException("Attempted to start table when the status wasn't Begin");
            }

            status = Status.Start;

            headerPosition = (uint)destination.Position;

            if (settings.PutBlankString)
            {
                stringPool.Put(StringPool.AdxBlankString);
            }

            header.TableNamePosition = (uint)stringPool.Put(tableName);

            var buffer = new byte[32];
            destination.Write(buffer, 0, 32);
        }

        public void WriteEndTable()
        {
            if (status == Status.FieldCollection)
            {
                WriteEndFieldCollection();
            }

            if (status == Status.Row)
            {
                WriteEndRow();
            }

            status = Status.End;

            destination.Seek(headerPosition + header.RowsPosition + (header.RowLength * header.RowCount), SeekOrigin.Begin);

            stringPool.Write(destination);
            header.StringPoolPosition = (uint)stringPool.Position - headerPosition;

            DataStream.Pad(destination, vldPool.Align);

            vldPool.Write(destination);
            header.DataPoolPosition = (uint)vldPool.Position - headerPosition;

            DataStream.Pad(destination, vldPool.Align);

            long previousPosition = destination.Position;

            header.Length = (uint)destination.Position - headerPosition;

            if (settings.EncodingType == Encoding.GetEncoding("shift-jis"))
            {
                header.EncodingType = CriTableHeader.EncodingTypeShiftJis;
            }

            else if (settings.EncodingType == Encoding.UTF8)
            {
                header.EncodingType = CriTableHeader.EncodingTypeUtf8;
            }

            destination.Seek(headerPosition, SeekOrigin.Begin);

            destination.Write(CriTableHeader.SignatureBytes, 0, 4);
            DataStream.WriteUInt32BE(destination, header.Length - 8);
            DataStream.WriteByte(destination, header.UnknownByte);
            DataStream.WriteByte(destination, header.EncodingType);
            DataStream.WriteUInt16BE(destination, (ushort)(header.RowsPosition - 8));
            DataStream.WriteUInt32BE(destination, header.StringPoolPosition - 8);
            DataStream.WriteUInt32BE(destination, header.DataPoolPosition - 8);
            DataStream.WriteUInt32BE(destination, header.TableNamePosition);
            DataStream.WriteUInt16BE(destination, header.FieldCount);
            DataStream.WriteUInt16BE(destination, header.RowLength);
            DataStream.WriteUInt32BE(destination, header.RowCount);

            if (settings.EnableMask)
            {
                destination.Seek(headerPosition, SeekOrigin.Begin);
                CriTableMasker.Mask(destination, header.Length, settings.MaskXor, settings.MaskXorMultiplier);
            }

            destination.Seek(previousPosition, SeekOrigin.Begin);
        }

        public void WriteStartFieldCollection()
        {
            if (status != Status.Start)
            {
                throw new InvalidOperationException("Attempted to start field collection when the status wasn't Start");
            }

            status = Status.FieldCollection;
        }

        public void WriteField(string fieldName, Type fieldType, object defaultValue)
        {
            if (status != Status.FieldCollection)
            {
                WriteStartFieldCollection();
            }

            CriFieldFlag fieldFlag = (CriFieldFlag)Array.IndexOf(CriField.FieldTypes, fieldType);

            if (!string.IsNullOrEmpty(fieldName))
            {
                fieldFlag |= CriFieldFlag.Name;
            }
            
            if (defaultValue != null)
            {
                fieldFlag |= CriFieldFlag.DefaultValue;
            }

            CriTableField field = new CriTableField
            {
                Flag = fieldFlag,
                Name = fieldName,
                Value = defaultValue
            };

            DataStream.WriteByte(destination, (byte)field.Flag);

            if (!string.IsNullOrEmpty(fieldName))
            {
                WriteString(field.Name);
            }

            if (defaultValue != null)
            {
                WriteValue(defaultValue);
            }

            fields.Add(field);
            header.FieldCount++;
        }

        public void WriteField(string fieldName, Type fieldType)
        {
            if (status != Status.FieldCollection)
            {
                WriteStartFieldCollection();
            }

            CriFieldFlag fieldFlag = (CriFieldFlag)Array.IndexOf(CriField.FieldTypes, fieldType) | CriFieldFlag.RowStorage;

            if (!string.IsNullOrEmpty(fieldName))
            {
                fieldFlag |= CriFieldFlag.Name;
            }

            CriTableField field = new CriTableField
            {
                Flag = fieldFlag,
                Name = fieldName
            };

            DataStream.WriteByte(destination, (byte)field.Flag);

            if (!string.IsNullOrEmpty(fieldName))
            {
                WriteString(field.Name);
            }

            field.Offset = header.RowLength;
            switch (field.Flag & CriFieldFlag.TypeMask)
            {
                case CriFieldFlag.Byte:
                case CriFieldFlag.SByte:
                    header.RowLength += 1;
                    break;
                case CriFieldFlag.Int16:
                case CriFieldFlag.UInt16:
                    header.RowLength += 2;
                    break;
                case CriFieldFlag.Int32:
                case CriFieldFlag.UInt32:
                case CriFieldFlag.Single:
                case CriFieldFlag.String:
                    header.RowLength += 4;
                    break;
                case CriFieldFlag.Int64:
                case CriFieldFlag.UInt64:
                case CriFieldFlag.Double:
                case CriFieldFlag.Data:
                    header.RowLength += 8;
                    break;
            }

            fields.Add(field);
            header.FieldCount++;
        }

        public void WriteField(CriField criField)
        {
            WriteField(criField.FieldName, criField.FieldType);
        }

        public void WriteEndFieldCollection()
        {
            if (status != Status.FieldCollection)
            {
                throw new InvalidOperationException("Attempted to end field collection when the status wasn't FieldCollection");
            }

            status = Status.Idle;

            header.RowsPosition = (ushort)(destination.Position - headerPosition);
        }

        public void WriteStartRow()
        {
            if (status == Status.FieldCollection)
            {
                WriteEndFieldCollection();
            }

            if (status != Status.Idle)
            {
                throw new InvalidOperationException("Attempted to start row when the status wasn't Idle");
            }

            status = Status.Row;

            header.RowCount++;

            destination.Seek(headerPosition + header.RowsPosition + (header.RowCount * header.RowLength), SeekOrigin.Begin);
            byte[] buffer = new byte[header.RowLength];
            destination.Write(buffer, 0, buffer.Length);
        }

        public void WriteValue(int fieldIndex, object rowValue)
        {
            if (fieldIndex >= fields.Count || fieldIndex < 0 || !fields[fieldIndex].Flag.HasFlag(CriFieldFlag.RowStorage) || rowValue == null)
            {
                return;
            }

            GoToValue(fieldIndex);
            WriteValue(rowValue);
        }

        public void WriteValue(string fieldName, object rowValue)
        {
            WriteValue(fields.FindIndex(field => field.Name == fieldName), rowValue);
        }

        private void GoToValue(int fieldIndex)
        {
            destination.Seek(headerPosition + header.RowsPosition + (header.RowLength * (header.RowCount - 1)) + fields[fieldIndex].Offset, SeekOrigin.Begin);
        }

        public void WriteEndRow()
        {
            if (status != Status.Row)
            {
                throw new InvalidOperationException("Attempted to end row when the status wasn't Row");
            }

            status = Status.Idle;
        }

        public void WriteRow(bool close, params object[] rowValues)
        {
            WriteStartRow();

            for (int i = 0; i < Math.Min(rowValues.Length, fields.Count); i++)
            {
                WriteValue(i, rowValues[i]);
            }

            if (close)
            {
                WriteEndRow();
            }
        }

        private void WriteString(string value)
        {
            if (settings.RemoveDuplicateStrings && stringPool.ContainsString(value))
            {
                DataStream.WriteUInt32BE(destination, (uint)stringPool.GetStringPosition(value));
            }

            else
            {
                DataStream.WriteUInt32BE(destination, (uint)stringPool.Put(value));
            }
        }

        private void WriteValue(object val)
        {
            switch (val)
            {
                case byte value:
                    DataStream.WriteByte(destination, value);
                    break;

                case sbyte value:
                    DataStream.WriteSByte(destination, value);
                    break;

                case ushort value:
                    DataStream.WriteUInt16BE(destination, value);
                    break;

                case short value:
                    DataStream.WriteInt16BE(destination, value);
                    break;

                case uint value:
                    DataStream.WriteUInt32BE(destination, value);
                    break;

                case int value:
                    DataStream.WriteInt32BE(destination, value);
                    break;

                case ulong value:
                    DataStream.WriteUInt64BE(destination, value);
                    break;

                case long value:
                    DataStream.WriteInt64BE(destination, value);
                    break;

                case float value:
                    DataStream.WriteSingleBE(destination, value);
                    break;

                case double value:
                    DataStream.WriteDoubleBE(destination, value);
                    break;

                case string value:
                    WriteString(value);
                    break;

                case byte[] value:
                    DataStream.WriteUInt32BE(destination, (uint)vldPool.Put(value));
                    DataStream.WriteUInt32BE(destination, (uint)value.Length);
                    break;

                case Guid value:
                    destination.Write(value.ToByteArray(), 0, 16);
                    break;

                case Stream value:
                    DataStream.WriteUInt32BE(destination, (uint)vldPool.Put(value));
                    DataStream.WriteUInt32BE(destination, (uint)value.Length);
                    break;

                case FileInfo value:
                    DataStream.WriteUInt32BE(destination, (uint)vldPool.Put(value));
                    DataStream.WriteUInt32BE(destination, (uint)value.Length);
                    break;
            }
        }

        public void Dispose()
        {
            if (status != Status.End)
            {
                WriteEndTable();
            }

            fields.Clear();
            stringPool.Clear();
            vldPool.Clear();

            if (!settings.LeaveOpen)
            {
                destination.Close();
            }
        }

        public static CriTableWriter Create(string destinationFileName)
        {
            return Create(destinationFileName, new CriTableWriterSettings());
        }

        public static CriTableWriter Create(string destinationFileName, CriTableWriterSettings settings)
        {
            Stream destination = File.Create(destinationFileName);
            return new CriTableWriter(destination, settings);
        }

        public static CriTableWriter Create(Stream destination)
        {
            return new CriTableWriter(destination, new CriTableWriterSettings());
        }

        public static CriTableWriter Create(Stream destination, CriTableWriterSettings settings)
        {
            return new CriTableWriter(destination, settings);
        }

        private CriTableWriter(Stream destination, CriTableWriterSettings settings)
        {
            this.destination = destination;
            this.settings = settings;

            header = new CriTableHeader();
            fields = new List<CriTableField>();
            stringPool = new StringPool(settings.EncodingType);
            vldPool = new DataPool(settings.Align);
        }
    }

    public class CriTableWriterSettings
    {
        private uint align = 1;
        private bool putBlankString = true;
        private bool leaveOpen = false;
        private Encoding encodingType = Encoding.GetEncoding("shift-jis");
        private bool removeDuplicateStrings = true;
        private bool enableMask = false;

        public uint Align
        {
            get
            {
                return align;
            }

            set
            {
                if (value <= 0)
                {
                    value = 1;
                }

                align = value;
            }
        }

        public bool PutBlankString
        {
            get
            {
                return putBlankString;
            }

            set
            {
                putBlankString = value;
            }
        }

        public bool LeaveOpen
        {
            get
            {
                return leaveOpen;
            }

            set
            {
                leaveOpen = value;
            }
        }

        public Encoding EncodingType
        {
            get
            {
                return encodingType;
            }

            set
            {
                if (value != Encoding.UTF8 || value != Encoding.GetEncoding("shift-jis"))
                {
                    return;
                }

                encodingType = value;
            }
        }

        public bool RemoveDuplicateStrings
        {
            get
            {
                return removeDuplicateStrings;
            }

            set
            {
                removeDuplicateStrings = value;
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

        public uint MaskXor { get; set; }
        public uint MaskXorMultiplier { get; set; }

        public static CriTableWriterSettings AdxSettings
        {
            get
            {
                return new CriTableWriterSettings()
                {
                    Align = 8,
                    PutBlankString = true,
                    RemoveDuplicateStrings = true,
                };
            }
        }

        public static CriTableWriterSettings Adx2Settings
        {
            get
            {
                return new CriTableWriterSettings()
                {
                    Align = 32,
                    PutBlankString = false,
                    RemoveDuplicateStrings = false,
                };
            }
        }
    }
}
