using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class Steam
    {
        public static string SteamLocation;

        public static void Init()
        {
            // Gets Steam's Registry Key
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
            // If null then try get it from the 64-bit Registry
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE\\Valve\\Steam");
            // Checks if the Key and Value exists.
            if (key != null && key.GetValue("SteamPath") is string steamPath)
                SteamLocation = steamPath;
        }

        public static Image GetCachedProfileImage(string SID)
        {
            string filePath = Path.Combine(SteamLocation, "config/avatarcache/", SID + ".png");
            if (File.Exists(filePath))
                return Image.FromFile(filePath);
            return null;
        }

        public class VDFFile
        {
            public class VDFElement
            {
                public string Name;
                public string Value;
                public VDFElement Parent;
            }

            public class VDFArray : VDFElement
            {
                public Dictionary<string, VDFElement> Elements = new Dictionary<string, VDFElement>();

                public VDFArray(string name)
                {
                    Name = name;
                }
            }

            public VDFArray Array = null;

            protected VDFFile()
            {

            }

            // I know this is not how you read .vdf files. But it works for files that I need to read.
            public static VDFFile ReadVDF(string filePath)
            {
                var file = new VDFFile();
                string buffer = "";
                VDFArray mainArray = null;
                VDFArray lastArray = null;
                VDFArray currentArray = null;

                VDFElement element = null;
                using (var textReader = File.OpenText(filePath))
                {
                    while (textReader.Peek() != -1)
                    {
                        string line = textReader.ReadLine();
                        bool startReadingString = false;
                        for (int i = 0; i < line.Length; ++i)
                        {
                            // Read String
                            if (startReadingString)
                            {
                                if (line[i] == '\"')
                                {
                                    startReadingString = false;
                                    if (element != null)
                                    {
                                        element.Value = buffer;
                                        buffer = "";
                                        currentArray.Elements.Add(element.Name, element);
                                        element = null;
                                    }
                                    continue;
                                }
                                buffer += line[i];
                                continue;
                            }

                            switch (line[i])
                            {
                                case '\"':
                                    if (buffer.Length != 0)
                                    {
                                        if (element == null)
                                        {
                                            element = new VDFElement();
                                            element.Name = buffer;
                                            buffer = "";
                                        }
                                    }
                                    startReadingString = true;
                                    break;
                                case '{':
                                    if (mainArray == null)
                                    {
                                        mainArray = new VDFArray(buffer);
                                        currentArray = mainArray;
                                        buffer = "";
                                        break;
                                    }
                                    var array = new VDFArray(buffer);
                                    lastArray = currentArray;
                                    currentArray.Elements.Add(array.Name, array);
                                    currentArray = array;
                                    buffer = "";
                                    break;
                                case '}':
                                    currentArray = lastArray;
                                    lastArray = mainArray;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                file.Array = mainArray;
                return file;
            }


        }
    }
}
