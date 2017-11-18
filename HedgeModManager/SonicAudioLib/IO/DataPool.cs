using System.Collections;
using System.IO;

using SonicAudioLib.FileBases;

namespace SonicAudioLib.IO
{
    public class DataPool
    {
        private ArrayList items = new ArrayList();

        private long startPosition = 0;
        private uint align = 1;
        private long length = 0;
        private long baseLength = 0;

        public long Position
        {
            get
            {
                return startPosition;
            }
        }

        public long Length
        {
            get
            {
                return length;
            }
        }

        public long Align
        {
            get
            {
                return align;
            }
        }

        public event ProgressChanged ProgressChanged;

        public long Put(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                return 0;
            }

            length = Helpers.Align(length, align);

            long position = length;
            length += data.Length;
            items.Add(data);

            return position;
        }

        public long Put(Stream stream)
        {
            if (stream == null || stream.Length <= 0)
            {
                return 0;
            }

            length = Helpers.Align(length, align);

            long position = length;
            length += stream.Length;
            items.Add(stream);

            return position;
        }

        public long Put(FileInfo fileInfo)
        {
            if (fileInfo == null || fileInfo.Length <= 0)
            {
                return 0;
            }

            length = Helpers.Align(length, align);

            long position = length;
            length += fileInfo.Length;
            items.Add(fileInfo);

            return position;
        }

        public void Write(Stream destination)
        {
            startPosition = destination.Position;

            foreach (object item in items)
            {
                DataStream.Pad(destination, align);

                if (item is byte[] bytes)
                {
                    destination.Write(bytes, 0, bytes.Length);
                }

                else if (item is Stream stream)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(destination);
                }

                else if (item is FileInfo fileInfo)
                {
                    using (Stream source = fileInfo.OpenRead())
                    {
                        source.CopyTo(destination);
                    }
                }

                ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(((destination.Position - startPosition) / (double)(length - baseLength)) * 100.0));
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public DataPool(uint align, long baseLength)
        {
            this.align = align;

            this.baseLength = baseLength;
            length = this.baseLength;
        }
        
        public DataPool(uint align)
        {
            this.align = align;
        }

        public DataPool()
        {
        }
    }
}
