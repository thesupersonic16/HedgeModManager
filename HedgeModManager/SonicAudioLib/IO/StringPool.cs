using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace SonicAudioLib.IO
{
    public class StringPool
    {
        private List<StringItem> items = new List<StringItem>();

        private long startPosition = 0;
        private long length = 0;
        private Encoding encoding = Encoding.Default;

        public const string AdxBlankString = "<NULL>";

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

        public long Put(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            long position = length;
            items.Add(new StringItem() { Value = value, Position = position });

            length += encoding.GetByteCount(value) + 1;
            return position;
        }

        public void Write(Stream destination)
        {
            startPosition = (uint)destination.Position;

            foreach (StringItem item in items)
            {
                DataStream.WriteCString(destination, item.Value, encoding);
            }
        }

        public bool ContainsString(string value)
        {
            return items.Any(item => item.Value == value);
        }

        public long GetStringPosition(string value)
        {
            return items.First(item => item.Value == value).Position;
        }

        public void Clear()
        {
            items.Clear();
        }

        public StringPool(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public StringPool()
        {
        }

        private class StringItem
        {
            public string Value { get; set; }
            public long Position { get; set; }
        }
    }
}
