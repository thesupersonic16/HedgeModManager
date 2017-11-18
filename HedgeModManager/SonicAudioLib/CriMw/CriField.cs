using System;
using System.ComponentModel;

namespace SonicAudioLib.CriMw
{
    public class CriField
    {
        public static readonly Type[] FieldTypes =
        {
            typeof(byte),
            typeof(sbyte),
            typeof(ushort),
            typeof(short),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(string),
            typeof(byte[]),
            typeof(Guid),
        };

        public static object[] NullValues =
        {
            (byte)0,
            (sbyte)0,
            (ushort)0,
            (short)0,
            (uint)0,
            (int)0,
            (ulong)0,
            (long)0,
            (float)0.0f,
            (double)0.0f,
            (string)string.Empty,
            (byte[])new byte[0],
            (Guid)Guid.Empty,
        };

        private Type fieldType;
        private string fieldName;
        private object defaultValue;
        private CriTable parent;

        public int FieldTypeIndex
        {
            get
            {
                return Array.IndexOf(FieldTypes, fieldType);
            }
        }

        public Type FieldType
        {
            get
            {
                return fieldType;
            }
        }

        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }

            set
            {
                defaultValue = ConvertObject(value);
            }
        }

        public string FieldName
        {
            get
            {
                return fieldName;
            }
        }

        public object ConvertObject(object obj)
        {
            if (obj == null)
            {
                return NullValues[FieldTypeIndex];
            }

            Type typ = obj.GetType();

            if (typ == fieldType)
            {
                return obj;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldType);

            if (typeConverter.CanConvertFrom(typ))
            {
                return typeConverter.ConvertFrom(obj);
            }

            return DefaultValue;
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

        public CriField(string name, Type type)
        {
            fieldName = name;
            fieldType = type;
        }

        public CriField(string name, Type type, object defaultValue)
        {
            fieldName = name;
            fieldType = type;
            this.defaultValue = ConvertObject(defaultValue);
        }
    }
}
