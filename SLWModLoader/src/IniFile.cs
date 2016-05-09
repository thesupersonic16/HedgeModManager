using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLWModLoader
{
    public class IniGroup : Dictionary<string, string>
    {
        public string groupName { get; set; }

        public IniGroup(string groupName)
        {
            this.groupName = groupName;
        }
    }

    public class IniFile
    {
        public Dictionary<string, IniGroup> groups { get; }

        public string iniPath { get; set; }
        public string iniName { get; set; }

        private string lastGroup;

        public IniFile()
        {
            groups = new Dictionary<string, IniGroup>();
        }

        public IniFile(string filename) : this()
        {
            iniPath = filename;
            iniName = Path.GetFileNameWithoutExtension(filename);

            using (TextReader textReader = File.OpenText(filename))
                read(textReader);
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
                    lastGroup = iniGroup.groupName;

                    groups.Add(iniGroup.groupName, iniGroup);
                }

                else if (equalsPosition != -1)
                {
                    string parameterName = stringResult.Substring(0, equalsPosition);
                    string parameterValue = stringResult.Substring(equalsPosition + 1);

                    groups[lastGroup].Add(parameterName, parameterValue);
                }
            }
        }

        public void write(TextWriter textWriter)
        {
            foreach (var group in groups)
            {
                textWriter.WriteLine($"[{group.Key}]");
                foreach (var groupParameter in group.Value)
                    textWriter.WriteLine($"{groupParameter.Key}={groupParameter.Value}");
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
