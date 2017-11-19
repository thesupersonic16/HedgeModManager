using System;
using System.IO;
using System.Linq;

using SonicAudioLib.IO;
using SonicAudioLib.FileBases;
using System.Xml;
using System.Xml.Linq;

namespace SonicAudioLib.CriMw
{
    public class CriTable : FileXmlBase
    {
        private CriFieldCollection fields;
        private CriRowCollection rows;
        private string tableName = "(no name)";
        private CriTableWriterSettings writerSettings;

        public CriFieldCollection Fields
        {
            get
            {
                return fields;
            }
        } 

        public CriRowCollection Rows
        {
            get
            {
                return rows;
            }
        }

        public string TableName
        {
            get
            {
                return tableName;
            }

            set
            {
                tableName = value;
            }
        }

        public CriTableWriterSettings WriterSettings
        {
            get
            {
                return writerSettings;
            }

            set
            {
                writerSettings = value;
            }
        }

        public void Clear()
        {
            rows.Clear();
            fields.Clear();
        }

        public CriRow NewRow()
        {
            CriRow criRow = new CriRow(this);

            foreach (CriField criField in fields)
            {
                criRow.Records.Add(new CriRowRecord { Field = criField, Value = criField.DefaultValue });
            }

            return criRow;
        }

        public override void Read(Stream source)
        {
            using (CriTableReader reader = CriTableReader.Create(source))
            {
                tableName = reader.TableName;

                for (int i = 0; i < reader.NumberOfFields; i++)
                {
                    fields.Add(reader.GetFieldName(i), reader.GetFieldType(i), reader.GetFieldValue(i));
                }

                while (reader.Read())
                {
                    rows.Add(reader.GetValueArray());
                }
            }
        }

        public override void Write(Stream destination)
        {
            using (CriTableWriter writer = CriTableWriter.Create(destination, writerSettings))
            {
                writer.WriteStartTable(tableName);

                writer.WriteStartFieldCollection();
                foreach (CriField criField in fields)
                {
                    bool useDefaultValue = false;
                    object defaultValue = null;

                    if (rows.Count > 1)
                    {
                        useDefaultValue = true;
                        defaultValue = rows[0][criField];

                        if (rows.Any(row => !row[criField].Equals(defaultValue)))
                        {
                            useDefaultValue = false;
                        }
                    }

                    else if (rows.Count == 0)
                    {
                        useDefaultValue = true;
                    }

                    if (useDefaultValue)
                    {
                        writer.WriteField(criField.FieldName, criField.FieldType, defaultValue);
                    }

                    else
                    {
                        writer.WriteField(criField.FieldName, criField.FieldType);
                    }
                }
                writer.WriteEndFieldCollection();

                foreach (CriRow criRow in rows)
                {
                    writer.WriteRow(true, criRow.GetValueArray());
                }

                writer.WriteEndTable();
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            var document = XDocument.Load(reader);

            foreach (XElement element in document.Root.Element(nameof(Fields)).Elements(nameof(CriField)))
            {
                fields.Add(
                    element.Element(nameof(CriField.FieldName)).Value,
                    Type.GetType(element.Element(nameof(CriField.FieldType)).Value)
                    );
            }

            foreach (XElement element in document.Root.Element(nameof(Rows)).Elements(nameof(CriRow)))
            {
                CriRow row = NewRow();

                foreach (CriRowRecord record in row.Records)
                {
                    if (record.Field.FieldType == typeof(byte[]))
                    {
                        record.Value = Convert.FromBase64String(element.Element(record.Field.FieldName).Value);
                    }

                    else
                    {
                        record.Value = Convert.ChangeType(element.Element(record.Field.FieldName).Value, record.Field.FieldType);
                    }
                }

                rows.Add(row);
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            var document = new XDocument(new XElement(nameof(CriTable)));
            document.Root.Add(new XElement(nameof(TableName), TableName));

            var fieldsElement = new XElement(nameof(Fields));

            foreach (CriField field in fields)
            {
                var fieldElement = new XElement(nameof(CriField));

                fieldElement.Add(
                    new XElement(nameof(field.FieldName), field.FieldName),
                    new XElement(nameof(field.FieldType), field.FieldType.Name)
                    );

                fieldsElement.Add(fieldElement);
            }

            document.Root.Add(fieldsElement);

            var rowsElement = new XElement(nameof(Rows));

            foreach (CriRow row in rows)
            {
                var rowElement = new XElement(nameof(CriRow));

                foreach (CriRowRecord record in row.Records)
                {
                    if (record.Value is byte[] bytes)
                    {
                        rowElement.Add(new XElement(record.Field.FieldName, Convert.ToBase64String(bytes)));
                    }

                    else
                    {
                        rowElement.Add(new XElement(record.Field.FieldName, record.Value));
                    }
                }

                rowsElement.Add(rowElement);
            }

            document.Root.Add(rowsElement);
            document.Save(writer);
        }

        public CriTable()
        {
            fields = new CriFieldCollection(this);
            rows = new CriRowCollection(this);
            writerSettings = new CriTableWriterSettings();
        }

        public CriTable(string tableName) : this()
        {
            this.tableName = tableName;
        }
    }
}
