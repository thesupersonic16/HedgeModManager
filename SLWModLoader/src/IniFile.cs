using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLWModLoader
{
    public class IniGroup : Dictionary<string, string> { }

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
            // Pretty unsafe way reading a INI file, but if everything is set correctly (like no random spaces), then it will be OK
            // but it needs to be done in a safer way
            while (textReader.Peek() != -1)
            {
                string nextLine = textReader.ReadLine();

                // If blank line, skip
                if (string.IsNullOrWhiteSpace(nextLine))
                    continue;

                // If there's no any group in the dictionary and the first line doesn't start with a group, then throw an exception
                if (groups.Count < 1 && (nextLine[0] != '[' && nextLine[nextLine.Length] != ']'))
                    throw new Exception("Could not find any group to add parameter, invalid INI file.");

                // That means we found a comment and we're skipping it
                if (nextLine[0] == ';' || nextLine[0] == '#')
                    continue;

                // So that means we have found a new group
                if (nextLine[0] == '[' && nextLine[nextLine.Length - 1] == ']')
                {
                    string groupName = nextLine.Substring(1, nextLine.Length - 2);
                    IniGroup iniGroup = new IniGroup();

                    groups.Add(groupName, iniGroup);

                    lastGroup = groupName;

                    continue;
                }

                // Or it's a parameter
                string[] parameters = nextLine.Split('=');

                if (parameters.Length > 2)
                    throw new Exception("Invalid parameter in INI file.");

                groups[lastGroup].Add(parameters[0], parameters[1]);
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
