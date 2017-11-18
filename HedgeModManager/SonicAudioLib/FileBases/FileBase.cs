using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SonicAudioLib.FileBases
{
    public abstract class FileBase
    {
        protected int bufferSize = 4096;

        public abstract void Read(Stream source);
        public abstract void Write(Stream destination);

        public virtual void Load(string sourceFileName, int bufferSize)
        {
            using (Stream source = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize))
            {
                Read(source);
            }

            this.bufferSize = bufferSize;
        }

        public virtual void Load(string sourceFileName)
        {
            Load(sourceFileName, 4096);
        }

        public virtual void Load(byte[] sourceByteArray)
        {
            using (Stream source = new MemoryStream(sourceByteArray))
            {
                Read(source);
            }
        }

        public virtual void Save(string destinationFileName)
        {
            Save(destinationFileName, 4096);
        }

        public virtual void Save(string destinationFileName, int bufferSize)
        {
            using (Stream destination = File.Create(destinationFileName, bufferSize))
            {
                Write(destination);
            }

            this.bufferSize = bufferSize;
        }

        public virtual byte[] Save()
        {
            using (MemoryStream destination = new MemoryStream())
            {
                Write(destination);
                return destination.ToArray();
            }
        }
    }
}
