using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;

using SonicAudioLib.IO;

namespace SonicAudioLib.CriMw.Serialization
{
    public static class CriTableSerializer
    {
        public static byte[] Serialize<T>(List<T> objects, CriTableWriterSettings settings)
        {
            using (MemoryStream destination = new MemoryStream())
            {
                Serialize(destination, objects, settings);
                return destination.ToArray();
            }
        }

        public static void Serialize<T>(string destinationFileName, List<T> objects, CriTableWriterSettings settings)
        {
            Serialize(destinationFileName, objects, settings, 4096);
        }

        public static void Serialize<T>(string destinationFileName, List<T> objects, CriTableWriterSettings settings, int bufferSize)
        {
            using (Stream destination = File.Create(destinationFileName, bufferSize))
            {
                Serialize(destination, objects, settings);
            }
        }

        public static void Serialize<T>(Stream destination, List<T> objects, CriTableWriterSettings settings)
        {
            Serialize(destination, typeof(T), objects, settings);
        }

        public static void Serialize(Stream destination, Type type, ICollection objects, CriTableWriterSettings settings)
        {
            ArrayList arrayList = null;

            if (objects != null)
            {
                arrayList = new ArrayList(objects);
            }

            CriTableWriter tableWriter = CriTableWriter.Create(destination, settings);

            string tableName = type.Name;
            CriSerializableAttribute serAttribute = type.GetCustomAttribute<CriSerializableAttribute>();
            if (serAttribute != null && !string.IsNullOrEmpty(serAttribute.TableName))
            {
                tableName = serAttribute.TableName;
            }

            tableWriter.WriteStartTable(tableName);

            SortedList<int, PropertyInfo> sortedProperties = new SortedList<int, PropertyInfo>();

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                // Add the properties in order
                CriIgnoreAttribute ignoreAttribute = propertyInfo.GetCustomAttribute<CriIgnoreAttribute>();
                if (ignoreAttribute != null)
                {
                    continue;
                }

                // Ignore the properties that are not supportable
                if (propertyInfo.PropertyType != typeof(FileInfo) &&
                    propertyInfo.PropertyType != typeof(Stream) &&
                    propertyInfo.PropertyType != typeof(bool) &&    
                    !propertyInfo.PropertyType.IsEnum &&
                    !CriField.FieldTypes.Contains(propertyInfo.PropertyType))
                {
                    continue;
                }

                CriFieldAttribute fieldAttribute = propertyInfo.GetCustomAttribute<CriFieldAttribute>();

                int order = ushort.MaxValue;
                if (fieldAttribute != null)
                {
                    order = fieldAttribute.Order;
                }

                while (sortedProperties.ContainsKey(order))
                {
                    order++;
                }

                sortedProperties.Add(order, propertyInfo);
            }

            tableWriter.WriteStartFieldCollection();
            foreach (var keyValuePair in sortedProperties)
            {
                PropertyInfo propertyInfo = keyValuePair.Value;
                CriFieldAttribute fieldAttribute = propertyInfo.GetCustomAttribute<CriFieldAttribute>();

                string fieldName = propertyInfo.Name;
                Type fieldType = propertyInfo.PropertyType;
                object defaultValue = null;
    
                if (fieldType == typeof(FileInfo) || fieldType == typeof(Stream))
                {
                    fieldType = typeof(byte[]);
                }

                else if (fieldType == typeof(bool))
                {
                    fieldType = typeof(byte);
                }

                else if (fieldType.IsEnum)
                {
                    fieldType = Enum.GetUnderlyingType(propertyInfo.PropertyType);
                }

                if (fieldAttribute != null)
                {
                    if (!string.IsNullOrEmpty(fieldAttribute.FieldName))
                    {
                        fieldName = fieldAttribute.FieldName;   
                    }
                }

                bool useDefaultValue = false;

                if (arrayList != null && arrayList.Count > 1)
                {
                    useDefaultValue = true;

                    defaultValue = propertyInfo.GetValue(arrayList[0]);

                    for (int i = 1; i < arrayList.Count; i++)
                    {
                        object objectValue = propertyInfo.GetValue(arrayList[i]);
                        if (defaultValue != null)
                        {
                            if (!defaultValue.Equals(objectValue))
                            {
                                useDefaultValue = false;
                                defaultValue = null;
                                break;
                            }
                        }
                        else if (objectValue != null)
                        {
                            useDefaultValue = false;
                            defaultValue = null;
                            break;
                        }
                    }
                }

                else if (arrayList == null || (arrayList != null && (arrayList.Count == 0 || (arrayList.Count == 1 && propertyInfo.GetValue(arrayList[0]) is null))))
                {
                    useDefaultValue = true;
                    defaultValue = null;
                }

                if (defaultValue is bool boolean)
                {
                    defaultValue = (byte)(boolean ? 1 : 0);
                }

                else if (defaultValue is Enum)
                {
                    defaultValue = Convert.ChangeType(defaultValue, fieldType);
                }

                if (useDefaultValue)
                {
                    tableWriter.WriteField(fieldName, fieldType, defaultValue);
                }

                else
                {
                    tableWriter.WriteField(fieldName, fieldType);
                }
            }

            tableWriter.WriteEndFieldCollection();

            // Time for objects.
            if (arrayList != null)
            {
                foreach (object obj in arrayList)
                {
                    tableWriter.WriteStartRow();

                    int index = 0;
                    foreach (PropertyInfo propertyInfo in sortedProperties.Values)
                    {
                        object value = propertyInfo.GetValue(obj);
                        Type propertyType = propertyInfo.PropertyType;

                        if (value is bool boolean)
                        {
                            value = (byte)(boolean ? 1 : 0);
                        }

                        else if (value is Enum)
                        {
                            value = Convert.ChangeType(value, Enum.GetUnderlyingType(propertyType));
                        }

                        tableWriter.WriteValue(index, value);
                        index++;
                    }

                    tableWriter.WriteEndRow();
                }
            }

            tableWriter.WriteEndTable();
            tableWriter.Dispose();
        }

        public static List<T> Deserialize<T>(byte[] sourceByteArray)
        {
            return Deserialize(sourceByteArray, typeof(T)).OfType<T>().ToList();
        }

        public static List<T> Deserialize<T>(string sourceFileName)
        {
            return Deserialize<T>(sourceFileName, 4096);
        }

        public static List<T> Deserialize<T>(string sourceFileName, int bufferSize)
        {
            return Deserialize(sourceFileName, typeof(T), bufferSize).OfType<T>().ToList();
        }

        public static List<T> Deserialize<T>(Stream source)
        {
            return Deserialize(source, typeof(T)).OfType<T>().ToList();
        }

        public static ArrayList Deserialize(byte[] sourceByteArray, Type type)
        {
            if (sourceByteArray == null || sourceByteArray.Length == 0)
            {
                return new ArrayList();
            }

            using (MemoryStream source = new MemoryStream(sourceByteArray))
            {
                return Deserialize(source, type);
            }
        }

        public static ArrayList Deserialize(string sourceFileName, Type type, int bufferSize)
        {
            if (!File.Exists(sourceFileName))
            {
                return new ArrayList();
            }

            using (Stream source = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize))
            {
                return Deserialize(source, type);
            }
        }

        public static ArrayList Deserialize(Stream source, Type type)
        {
            ArrayList arrayList = new ArrayList();

            using (CriTableReader tableReader = CriTableReader.Create(source, true))
            {
                IEnumerable<PropertyInfo> propertyInfos = type.GetProperties().Where(property =>
                {
                    if (property.GetCustomAttribute<CriIgnoreAttribute>() == null)
                    {
                        string fieldName = property.Name;

                        if (property.GetCustomAttribute<CriFieldAttribute>() != null)
                        {
                            fieldName = property.GetCustomAttribute<CriFieldAttribute>().FieldName;
                        }

                        return tableReader.ContainsField(fieldName);
                    }

                    return false;
                });

                while (tableReader.Read())
                {
                    object obj = Activator.CreateInstance(type);

                    // I hope this is faster than the old method lol
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        string fieldName = propertyInfo.Name;

                        if (propertyInfo.GetCustomAttribute<CriFieldAttribute>() != null)
                        {
                            fieldName = propertyInfo.GetCustomAttribute<CriFieldAttribute>().FieldName;
                        }

                        object value = tableReader.GetValue(fieldName);

                        if (value is SubStream substream)
                        {
                            value = substream.ToArray();
                        }

                        else if (value is byte boolean && propertyInfo.PropertyType == typeof(bool))
                        {
                            value = boolean == 1;
                        }

                        else if (propertyInfo.PropertyType.IsEnum)
                        {
                            value = Convert.ChangeType(value, Enum.GetUnderlyingType(propertyInfo.PropertyType));
                        }

                        value = Convert.ChangeType(value, propertyInfo.PropertyType);

                        propertyInfo.SetValue(obj, value);
                    }

                    arrayList.Add(obj);
                }
            }

            return arrayList;
        }
    }
}
