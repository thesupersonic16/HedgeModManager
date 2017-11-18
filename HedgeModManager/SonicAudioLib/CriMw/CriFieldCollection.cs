using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicAudioLib.CriMw
{
    public class CriFieldCollection : IEnumerable<CriField>
    {
        private CriTable parent;
        private List<CriField> fields = new List<CriField>();

        public CriField this[int index]
        {
            get
            {
                return fields[index];
            }
        }

        public CriField this[string name]
        {
            get
            {
                return fields.FirstOrDefault(field => field.FieldName == name);
            }
        }

        public int Count
        {
            get
            {
                return fields.Count;
            }
        }

        public CriTable Parent
        {
            get
            {
                return parent;
            }

            internal set
            {
                parent = value;
            }
        }

        public void Add(CriField criField)
        {
            criField.Parent = parent;
            fields.Add(criField);
        }

        public CriField Add(string name, Type type)
        {
            CriField criField = new CriField(name, type);
            Add(criField);

            return criField;
        }

        public CriField Add(string name, Type type, object defaultValue)
        {
            CriField criField = new CriField(name, type, defaultValue);
            Add(criField);

            return criField;
        }

        public void Insert(int index, CriField criField)
        {
            if (index >= fields.Count || index < 0)
            {
                fields.Add(criField);
            }

            else
            {
                fields.Insert(index, criField);
            }
        }

        public void Remove(CriField criField)
        {
            fields.Remove(criField);

            // Update the rows
            foreach (CriRow criRow in parent.Rows)
            {
                criRow.Records.RemoveAll(record => record.Field == criField);
            }
        }

        public void RemoveAt(int index)
        {
            Remove(fields[index]);
        }

        internal void Clear()
        {
            fields.Clear();
        }

        public IEnumerator<CriField> GetEnumerator()
        {
            return ((IEnumerable<CriField>)fields).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CriField>)fields).GetEnumerator();
        }

        public CriFieldCollection(CriTable parent)
        {
            this.parent = parent;
        }
    }
}
