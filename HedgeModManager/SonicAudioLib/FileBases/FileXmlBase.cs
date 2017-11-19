using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace SonicAudioLib.FileBases
{
    // Because C# doesn't allow you to inherit
    // more than 1 abstract class.
    public abstract class FileXmlBase : FileBase
    {
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);

        public virtual void LoadXml(string sourceFileName)
        {
            using (XmlReader reader = XmlReader.Create(sourceFileName))
            {
                ReadXml(reader);
            }
        }

        public virtual void SaveXml(string destinationFileName)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(destinationFileName, settings))
            {
                WriteXml(writer);
            }
        }
    }
}
