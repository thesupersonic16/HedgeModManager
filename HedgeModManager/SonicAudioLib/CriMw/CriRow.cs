using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicAudioLib.CriMw
{
    internal class CriRowRecord
    {
        public CriField Field { get; set; }
        public object Value { get; set; }
    }

    public class CriRow : IEnumerable
    {
        private List<CriRowRecord> records = new List<CriRowRecord>();
        private CriTable parent;

        public object this[CriField criField]
        {
            get
            {
                return this[records.FindIndex(record => record.Field == criField)];
            }

            set
            {
                this[records.FindIndex(record => record.Field == criField)] = value;
            }
        }

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= records.Count)
                {
                    return null;
                }

                return records[index].Value;
            }

            set
            {
                if (index < 0 || index >= records.Count)
                {
                    return;
                }

                records[index].Value = value;
            }
        }

        public object this[string name]
        {
            get
            {
                return this[records.FindIndex(record => record.Field.FieldName == name)];
            }

            set
            {
                this[records.FindIndex(record => record.Field.FieldName == name)] = value;
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

        internal List<CriRowRecord> Records
        {
            get
            {
                return records;
            }
        }

        public int FieldCount
        {
            get
            {
                return records.Count;
            }
        }

        public T GetValue<T>(CriField criField)
        {
            return (T)this[criField];
        }

        public T GetValue<T>(string fieldName)
        {
            return (T)this[fieldName];
        }

        public T GetValue<T>(int fieldIndex)
        {
            return (T)this[fieldIndex];
        }

        public object[] GetValueArray()
        {
            object[] values = new object[records.Count];
            
            for (int i = 0; i < records.Count; i++)
            {
                values[i] = records[i].Value;
            }

            return values;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var record in records)
            {
                yield return record.Value;
            }

            yield break;
        }

        internal CriRow(CriTable parent)
        {
            this.parent = parent;
        }
    }
}
