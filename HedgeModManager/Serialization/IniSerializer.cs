using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace HedgeModManager.Serialization
{
    class IniSerializer
    {
        public static void Serialize(object obj, Stream stream)
        {
            var type = obj.GetType();
            var file = new IniFile();

            foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance))
            {
                IniField fieldAttribute = property.GetCustomAttribute<IniField>();
                if (fieldAttribute == null)
                    continue;

                var group = string.IsNullOrEmpty(fieldAttribute.Group) ? "Main" : fieldAttribute.Group;
                var name = string.IsNullOrEmpty(fieldAttribute.Name) ? property.Name : fieldAttribute.Name;
                var value = property.GetValue(obj);
                WriteValue(name, group, value);
            }

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance))
            {
                IniField fieldAttribute = field.GetCustomAttribute<IniField>();
                if (fieldAttribute == null)
                    continue;

                var group = string.IsNullOrEmpty(fieldAttribute.Group) ? "Main" : fieldAttribute.Group;
                var name = string.IsNullOrEmpty(fieldAttribute.Name) ? field.Name : fieldAttribute.Name;
                var value = field.GetValue(obj);
                WriteValue(name, group, value);
            }

            file.Write(stream);

            void WriteValue(string name, string group, object value)
            {
                var valueType = value.GetType();
                if (typeof(IDictionary).IsAssignableFrom(valueType))
                {
                    var dic = (IDictionary)value;
                    var keyEnum = dic.Keys.GetEnumerator();
                    var valueEnum = dic.Values.GetEnumerator();

                    keyEnum.MoveNext();
                    valueEnum.MoveNext();
                    for(int i = 0; i < dic.Count; i++)
                    {
                        var key = keyEnum.Current.ToString();
                        var val = valueEnum.Current.ToString();

                        file[group][key] = val;

                        keyEnum.MoveNext();
                        valueEnum.MoveNext();
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(valueType) && valueType != typeof(string))
                {
                    var enumrable = (IEnumerable)value;
                    int count = 0;
                    foreach (var item in enumrable)
                    {
                        file[group][$"{name}{count}"] = item.ToString();
                        count++;
                    }
                    file[group][$"{name}Count"] = count.ToString();
                }
                else if(valueType == typeof(bool))
                {
                    var b = (bool)value;
                    file[group][name] = b ? "1" : "0";
                }
                else if(valueType.IsEnum)
                {
                    file[group][name] = Enum.GetName(valueType, value);
                }
                else
                {
                    file[group][name] = value.ToString();
                }
            }
        }

        public static T Deserialize<T>(Stream stream)
        {
            var obj = Activator.CreateInstance(typeof(T));
            Deserialize(obj, stream);
            return (T)obj;
        }

        public static void Deserialize(object obj, Stream stream)
        {
            var file = new IniFile();
            file.Read(stream);
            foreach(var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance))
            {
                IniField fieldAttribute = property.GetCustomAttribute<IniField>();
                if (fieldAttribute == null)
                    continue;
                
                var group = string.IsNullOrEmpty(fieldAttribute.Group) ? "Main" : fieldAttribute.Group;
                var name = string.IsNullOrEmpty(fieldAttribute.Name) ? property.Name : fieldAttribute.Name;
                var valueType = property.PropertyType;

                property.SetValue(obj, ReadField(group, name, valueType));
            }

            foreach(var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance))
            {
                IniField fieldAttribute = field.GetCustomAttribute<IniField>();
                if (fieldAttribute == null)
                    continue;

                var group = string.IsNullOrEmpty(fieldAttribute.Group) ? "Main" : fieldAttribute.Group;
                var name = string.IsNullOrEmpty(fieldAttribute.Name) ? field.Name : fieldAttribute.Name;
                var valueType = field.FieldType;

                field.SetValue(obj, ReadField(group, name, valueType));
            }

            object ReadField(string group, string name, Type valueType)
            {
                if(typeof(IDictionary).IsAssignableFrom(valueType))
                {
                    var dic = (IDictionary)Activator.CreateInstance(valueType);
                    
                    foreach(var param in file[group].Params)
                    {
                        dic.Add(param.Key, param.Value);
                    }

                    return dic;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(valueType) && valueType != typeof(string))
                {
                    return ReadArray(group, name, valueType);
                }
                else if(valueType == typeof(bool))
                {
                    return file[group][name] == "1";
                }
                else if(valueType.IsEnum)
                {
                    return Enum.Parse(valueType, file[group][name]);
                }
                else
                {
                    return Convert.ChangeType(file[group][name], valueType);
                }
            }

            IEnumerable ReadArray(string group, string field, Type t)
            {
                var list = (IList)Activator.CreateInstance(t);
                var count = string.IsNullOrEmpty(file[group][$"{field}Count"]) ? int.MaxValue : Convert.ToInt32(file[group][$"{field}Count"]);
                for(int i = 0; i < count; i++)
                {
                    if (file[group].Params.ContainsKey($"{field}{i}"))
                        list.Add(file[group][$"{field}{i}"]);
                    else
                        break;
                }
                return list;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class IniField : Attribute
    {
        public string Name;
        public string Group;

        public IniField()
        {

        }

        public IniField(string group)
        {
            Group = group;
        }

        public IniField(string group, string name)
        {
            Group = group;
            Name = name;
        }
    }
}
