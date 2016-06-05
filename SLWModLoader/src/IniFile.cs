using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace SLWModLoader
{
    public class IniGroup
    {
        private Dictionary<string, string> parameters;
        public string groupName { get; set; }

        public IniGroup()
        {
            parameters = new Dictionary<string, string>();
        }

        public IniGroup(string groupName) : this()
        {
            this.groupName = groupName;
        }

        public void addParameter(string key, string value)
        {
            if (parameters.ContainsKey(key))
                throw new Exception($"IniGroup {groupName} already contains a parameter named {key}");

            parameters.Add(key, value);
        }

        public string this[string key]
        {
            get
            {
                if (!parameters.ContainsKey(key))
                    throw new Exception($"IniGroup {groupName} does not contain a key named {key}");

                return parameters[key];
            }

            set
            {
                if (!parameters.ContainsKey(key))
                    throw new Exception($"IniGroup {groupName} does not contain a key named {key}");

                parameters[key] = value;
            }
        }

        public object this[string key, Type type]
        {
            get
            {
                if (!parameters.ContainsKey(key))
                    throw new Exception($"IniGroup {groupName} does not contain a key named {key}");

                var dataConverter = TypeDescriptor.GetConverter(type);
                var result = dataConverter.ConvertFrom(parameters[key]);

                return result;
            }

            set
            {
                if (!parameters.ContainsKey(key))
                    throw new Exception($"IniGroup {groupName} does not contain a key named {key}");

                var dataConverter = TypeDescriptor.GetConverter(type);
                var result = dataConverter.ConvertToString(value);

                parameters[key] = result;
            }
        }

        internal Dictionary<string, string> parametersDictionary
        {
            get
            {
                return parameters;
            }
        }

        public int parameterCount
        {
            get
            {
                return parameters.Count;
            }
        }
    }

    public class IniFile
    {
        private List<IniGroup> groups;

        public string iniPath { get; set; }
        public string iniName { get; set; }

        public IniFile()
        {
            groups = new List<IniGroup>();
        }

        public IniFile(string filename) : this()
        {
            iniPath = filename;
            iniName = Path.GetFileNameWithoutExtension(filename);

            using (TextReader textReader = File.OpenText(filename))
                read(textReader);
        }

        public void addGroup(IniGroup iniGroup)
        {
            foreach (var group in groups)
            {
                if (group == iniGroup || group.groupName == iniGroup.groupName)
                    throw new Exception($"IniGroup {iniGroup.groupName} already exists in IniFile {iniName}");
            }

            groups.Add(iniGroup);
        }

        public IniGroup this[string groupName]
        {
            get
            {
                foreach (var group in groups)
                {
                    if (group.groupName == groupName) return group;
                }

                throw new Exception($"IniGroup {groupName} does not exist in IniFile {iniName}");
            }
        }

        public void read(TextReader textReader)
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

                    groups[groups.Count - 1].addParameter(parameterKey, parameterValue);
                }
            }
        }

        public void write(TextWriter textWriter)
        {
            foreach (var group in groups)
            {
                string line1 = String.Format("[{0}]", group.groupName);
                textWriter.WriteLine(line1);

                foreach (var parameter in group.parametersDictionary)
                {
                    string line2 = String.Format("{0}={1}", parameter.Key, parameter.Value);
                    textWriter.WriteLine(line2);
                }
            }
        }

        public void save(string filename)
        {
            using (TextWriter textWriter = File.CreateText(filename))
                write(textWriter);
        }

        public void save()
        {
            save(iniPath);
        }
    }
}
