using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace SLWModLoader
{
    internal class IniParameter
    {
        private string key;
        private string value;

        public string Key
        {
            get { return key; }
            set
            {
                key = value.Replace("\"", "");
            }
        }

        public string Value
        {
            get { return value; }
            set
            {
                this.value = value.Replace("\"", "");
            }
        }

        public IniParameter()
        {
            Key = string.Empty;
            Value = string.Empty;
        }

        public IniParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class IniGroup
    {
        private List<IniParameter> parameters;
        public string GroupName { get; set; }
        public int ParameterCount => parameters.Count;

        public IniGroup()
        {
            parameters = new List<IniParameter>();
        }

        public IniGroup(string groupName) : this()
        {
            GroupName = groupName;
        }

        public void AddParameter(string key, string value)
        {
            parameters.Add(new IniParameter(key, value));
        }

        public void AddParameter(string key)
        {
            parameters.Add(new IniParameter(key, string.Empty));
        }

        public void AddParameter(string key, object value, Type type)
        {
            parameters.Add(new IniParameter(key, TypeDescriptor.GetConverter(type).ConvertToString(value)));
        }

        public void RemoveParameter(string key)
        {
            if (ContainsParameter(key))
            {
                parameters.RemoveAt(GetIndexOfParameter(key));
            }
        }

        public void RemoveParameterAt(int index)
        {
            if (index < parameters.Count)
            {
                parameters.RemoveAt(index);
            }
        }

        public void RemoveAllParameters()
        {
            parameters.Clear();
        }

        public bool ContainsParameter(string key)
        {
            return parameters.Any(t => t.Key == key);
        }

        private int GetIndexOfParameter(string key)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Key == key)
                    return i;
            }

            return -1;
        }

        public string this[string key]
        {
            // Return empty string instead of throwing exception
            get { return ContainsParameter(key) ? parameters[GetIndexOfParameter(key)].Value : string.Empty; }
            set
            {
                if (ContainsParameter(key))
                    parameters[GetIndexOfParameter(key)].Value = value;
            }
        }

        public object this[string key, Type type]
        {
            get
            {
                return ContainsParameter(key) ? TypeDescriptor.GetConverter(type).ConvertFrom(parameters[GetIndexOfParameter(key)].Value) : string.Empty;
            }

            set
            {
                if (ContainsParameter(key))
                    parameters[GetIndexOfParameter(key)].Value = TypeDescriptor.GetConverter(type).ConvertToString(value);
            }
        }

        internal IniParameter this[int index] => index < parameters.Count ? parameters.ElementAt(index) : new IniParameter();

        public void Clear()
        {
            parameters.Clear();
        }
    }

    public class IniFile
    {
        private List<IniGroup> groups;

        public string IniPath { get; set; }
        public string IniName { get; set; }

        public IniFile()
        {
            IniPath = string.Empty;
            IniName = string.Empty;

            groups = new List<IniGroup>();
        }

        public IniFile(string filename) : this()
        {
            IniPath = filename;
            IniName = Path.GetFileNameWithoutExtension(filename);

            using (TextReader textReader = File.OpenText(filename))
                Read(textReader);
        }

        public void AddGroup(IniGroup iniGroup)
        {
            if (groups.Any(t => t.GroupName == iniGroup.GroupName))
            {
                string message = $"{IniName} already contains a group named {iniGroup.GroupName}";
                throw new Exception(message);
            }

            groups.Add(iniGroup);
        }

        public void AddGroup(string name)
        {
            IniGroup iniGroup = new IniGroup(name);
            AddGroup(iniGroup);
        }

        public IniGroup this[string groupName]
        {
            get
            {
                foreach (IniGroup group in groups)
                    if (group.GroupName == groupName)
                        return group;

                string message = $"{IniName} does not have a group named {groupName}";
                throw new Exception(message);
            }
        }

        public void Read(TextReader textReader)
        {
            while (textReader.Peek() != -1)
            {
                string nextLine = textReader.ReadLine();

                bool hasBracketAtStart = false;

                int secondBracketPosition = -1;
                int equalsPosition = -1;

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < nextLine.Length; i++)
                {
                    switch (nextLine[i])
                    {
                        case '[':
                            if (i == 0)
                                hasBracketAtStart = true;

                            goto default;

                        case ']':
                            if (hasBracketAtStart)
                                secondBracketPosition = stringBuilder.Length;

                            goto default;

                        case '\\':
                            if (i + 1 == nextLine.Length)
                                goto default;

                            i++;
                            switch (nextLine[i])
                            {
                                case 'n':
                                    stringBuilder.Append('\n');
                                    break;

                                case 'r':
                                    stringBuilder.Append('\r');
                                    break;

                                default:
                                    stringBuilder.Append(nextLine[i]);
                                    break;
                            }

                            break;

                        case '=':
                            equalsPosition = stringBuilder.Length;
                            goto default;

                        case '#':
                        case ';':
                            i = nextLine.Length;
                            break;

                        default:
                            stringBuilder.Append(nextLine[i]);
                            break;
                    }
                }

                string stringResult = stringBuilder.ToString();

                if (hasBracketAtStart && secondBracketPosition != -1)
                {
                    IniGroup iniGroup = new IniGroup(stringResult.Substring(1, secondBracketPosition - 1));
                    groups.Add(iniGroup);
                }

                else if (equalsPosition != -1)
                {
                    string parameterKey = stringResult.Substring(0, equalsPosition);
                    string parameterValue = stringResult.Substring(equalsPosition + 1);

                    groups[groups.Count - 1].AddParameter(parameterKey, parameterValue);
                }
            }
        }

        public void Write(TextWriter textWriter)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                string line1 = $"[{groups[i].GroupName}]";
                textWriter.WriteLine(line1);

                for (int j = 0; j < groups[i].ParameterCount; j++)
                {
                    string line2 = $"{groups[i][j].Key}={groups[i][j].Value}";
                    textWriter.WriteLine(line2);
                }
            }
        }

        public void Save(string filename)
        {
            using (TextWriter textWriter = File.CreateText(filename))
            {
                Write(textWriter);
            }
        }

        public void Save()
        {
            Save(IniPath);
        }
    }
}
