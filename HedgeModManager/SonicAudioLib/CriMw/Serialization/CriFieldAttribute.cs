using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicAudioLib.CriMw.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CriFieldAttribute : Attribute
    {
        private string fieldName;
        private ushort order = ushort.MaxValue;

        public string FieldName
        {
            get
            {
                return fieldName;
            }
        }

        public ushort Order
        {
            get
            {
                return order;
            }
        }

        public CriFieldAttribute(ushort order)
        {
            this.order = order;
        }

        public CriFieldAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }

        public CriFieldAttribute(string fieldName, ushort order)
        {
            this.fieldName = fieldName;
            this.order = order;
        }

        public CriFieldAttribute() { }
    }
}
