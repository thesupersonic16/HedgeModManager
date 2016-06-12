using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.ComponentModel;
using System.IO;

namespace SLWModLoader
{
    public class IniGroup
    {
        private Dictionary<string, string> parameters;
        public string GroupName { get; set; }
        public int ParameterCount { get { return parameters.Count; } }

        public IniGroup()
        {
            parameters = new Dictionary<string, string>();
        }

        public IniGroup(string groupName) : this()
        {
            GroupName = groupName;
        }

        public void AddParameter(string key, string value)
        {
            parameters.Add(key, value);
        }

        public string this[string key]
        {
            get { return parameters[key]; }
            set { parameters[key] = value; }
        }

        public object this[string key, Type type]
        {
            get { return TypeDescriptor.GetConverter(type).ConvertFrom(parameters[key]); }
            set { parameters[key] = TypeDescriptor.GetConverter(type).ConvertToString(value); }
        }

        public KeyValuePair<string, string> this[int index] { get { return parameters.ElementAt(index); } }
    }

    public class IniFile
    {
        private List<IniGroup> groups;

        public string IniPath { get; set; }
        public string IniName { get; set; }

        public IniFile()
        {
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
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].GroupName == iniGroup.GroupName)
                {
                    string message = String.Format("{0} already contains a group named {1}", IniName, iniGroup.GroupName);
                    throw new Exception(message);
                }
            }

            groups.Add(iniGroup);
        }

        public IniGroup this[string groupName]
        {
            get
            {
                foreach (var group in groups)
                    if (group.GroupName == groupName)
                        return group;

                string message = String.Format("{0} does not have a group named {1}", IniName, groupName);
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
                string line1 = String.Format("[{0}]", groups[i].GroupName);
                textWriter.WriteLine(line1);

                for (int j = 0; j < groups[i].ParameterCount; j++)
                {
                    string line2 = String.Format("{0}={1}", groups[i][j].Key, groups[i][j].Value);
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
