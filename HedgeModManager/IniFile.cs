using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class IniFile
    {
        public Dictionary<string, IniGroup> Groups = new Dictionary<string, IniGroup>();

        public IniGroup this[string key]
        {
            get
            {
                if (!Groups.ContainsKey(key))
                    Groups.Add(key, new IniGroup());
                Groups.TryGetValue(key, out IniGroup grp);
                return grp;
            }
            set
            {
                if (Groups.ContainsKey(key))
                    Groups[key] = value;
                else
                    Groups.Add(key, value);
            }
        }

        public IniFile()
        {
 
        }

        public IniFile(string path) : this()
        {
            using (var stream = File.OpenRead(path))
                Read(stream);
        }

        public virtual void Read(Stream stream)
        {
            var reader = new StreamReader(stream);
            var currentGroup = string.Empty;
            while (!reader.EndOfStream)
            {
                char c;
                switch ((c = (char)reader.Read()))
                {
                    case ';':
                        {
                            reader.ReadLine();
                            break;
                        }

                    case '[':
                        {
                            currentGroup = reader.ReadLine().Replace("]", string.Empty);
                            Groups.Add(currentGroup, new IniGroup());
                            break;
                        }
                    case '=':
                        {
                            reader.ReadLine();
                            break;
                        }
                    default:
                        {
                            var line = reader.ReadLine();

                            if (string.IsNullOrEmpty(line) || !line.Contains('='))
                                break;

                            var splits = line.Split('=');
                            var temp = splits[1].Split('\"');
                            this[currentGroup].AddParameter($"{c}{splits[0]}", temp.Length == 1 ? temp[0] : temp[1]);
                            break;
                        }
                }
            }
        }

        public virtual void Write(Stream stream)
        {
            var writer = new StreamWriter(stream);

            foreach (var group in Groups)
            {
                writer.WriteLine($"[{group.Key}]");
                foreach (var val in group.Value.Params)
                {
                    if (int.TryParse(val.Value, out int num))
                        writer.WriteLine($"{val.Key}={val.Value}");
                    else
                        writer.WriteLine($"{val.Key}=\"{val.Value}\"");
                }
            }
            writer.Flush();
            writer.Dispose();
        }

        public class IniGroup
        {

            public Dictionary<string, string> Params = new Dictionary<string, string>();

            public string this[string key]
            {
                get
                {
                    Params.TryGetValue(key, out string val);
                    return val ?? string.Empty;
                }
                set
                {
                    if (Params.ContainsKey(key))
                        Params[key] = value;
                    else
                        Params.Add(key, value);
                }
            }

            public object this[string key, Type type]
            {
                get
                {
                    if (Params.ContainsKey(key))
                    {
                        string val = "";
                        if (Params.ContainsKey(key))
                            Params.TryGetValue(key, out val);
                        return Convert.ChangeType(val, type);
                    }
                    else return null;
                }
                set
                {
                    if (Params.ContainsKey(key))
                        Params[key] = value.ToString();
                    else
                        Params.Add(key, value.ToString());
                }
            }


            public void AddParameter(string key, object param)
            {
                Params.Add(key, param.ToString());
            }

            public void RemoveParameter(string key)
            {
                if (Params.ContainsKey(key))
                    Params.Remove(key);
            }

        }
    }
}
