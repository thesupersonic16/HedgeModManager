using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicAudioLib.CriMw.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class CriSerializableAttribute : Attribute
    {
        private string tableName;

        public string TableName
        {
            get
            {
                return tableName;
            }
        }

        public CriSerializableAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        public CriSerializableAttribute() { }
    }
}
