using System;
using System.IO;
using System.Linq;

namespace SonicAudioLib.IO
{
    public sealed class SubStream : Stream
    {
        private Stream baseStream;
        private long basePosition;
        private long baseLength;

        public override bool CanRead
        {
            get
            {
                return baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return baseStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return baseStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return baseLength;
            }
        }

        public override long Position
        {
            get
            {
                return baseStream.Position - basePosition;
            }

            set
            {
                baseStream.Position = basePosition + value;
            }
        }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (baseStream.Position >= basePosition + baseLength)
            {
                count = 0;
            }

            else if (baseStream.Position + count > basePosition + baseLength)
            {
                count = (int)(basePosition + baseLength - baseStream.Position);
            }

            return baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                offset += basePosition;
            }

            else if (origin == SeekOrigin.End)
            {
                offset = basePosition + baseLength - offset;
                origin = SeekOrigin.Begin;
            }

            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseLength = value;

            if (basePosition + baseLength > baseStream.Length)
            {
                baseStream.SetLength(basePosition + baseLength);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (baseStream.Position >= basePosition + baseLength)
            {
                count = 0;
            }

            else if (baseStream.Position + count > basePosition + baseLength)
            {
                count = (int)(basePosition + baseLength - baseStream.Position);
            }

            baseStream.Write(buffer, 0, count);
        }

        public byte[] ToArray()
        {
            long previousPosition = baseStream.Position;

            baseStream.Seek(basePosition, SeekOrigin.Begin);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                CopyTo(memoryStream);

                baseStream.Seek(previousPosition, SeekOrigin.Begin);
                return memoryStream.ToArray();
            }
        }

        public SubStream(Stream baseStream, long basePosition) : this(baseStream, basePosition, baseStream.Length - basePosition)
        {
        }

        public SubStream(Stream baseStream, long basePosition, long baseLength)
        {
            this.baseStream = baseStream;
            this.basePosition = basePosition;
            this.baseLength = baseLength;

            baseStream.Seek(this.basePosition, SeekOrigin.Begin);
        }
    }
}
