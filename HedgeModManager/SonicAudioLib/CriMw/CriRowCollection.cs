using System;
using System.Collections;
using System.Collections.Generic;

namespace SonicAudioLib.CriMw
{
    public class CriRowCollection : IEnumerable<CriRow>
    {
        private CriTable parent;
        private List<CriRow> rows = new List<CriRow>();

        public CriRow this[int index]
        {
            get
            {
                return rows[index];
            }
        }

        public int Count
        {
            get
            {
                return rows.Count;
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

        public void Add(CriRow criRow)
        {
            criRow.Parent = parent;
            rows.Add(criRow);
        }

        public CriRow Add(params object[] objs)
        {
            CriRow criRow = parent.NewRow();

            object[] objects = new object[criRow.FieldCount];
            Array.Copy(objs, objects, Math.Min(objs.Length, objects.Length));

            for (int i = 0; i < criRow.FieldCount; i++)
            {
                criRow[i] = objects[i];
            }

            Add(criRow);
            return criRow;
        }

        internal void Clear()
        {
            rows.Clear();
        }

        public IEnumerator<CriRow> GetEnumerator()
        {
            return ((IEnumerable<CriRow>)rows).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CriRow>)rows).GetEnumerator();
        }

        public CriRowCollection(CriTable parent)
        {
            this.parent = parent;
        }
    }
}
