using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SonicAudioLib.Archives;

namespace SonicAudioLib.IO
{
    public class DataExtractor
    {
        public enum LoggingType
        {
            Progress,
            Message,
        }

        private class Item
        {
            public object Source { get; set; }
            public string DestinationFileName { get; set; }
            public long Position { get; set; }
            public long Length { get; set; }
        }

        private List<Item> items = new List<Item>();

        public int BufferSize { get; set; }
        public bool EnableThreading { get; set; }
        public int MaxThreads { get; set; }

        public event ProgressChanged ProgressChanged;

        public void Add(object source, string destinationFileName, long position, long length)
        {
            items.Add(new Item { Source = source, DestinationFileName = destinationFileName, Position = position, Length = length, });
        }

        public void Run()
        {
            double progress = 0.0;
            double factor = 100.0 / items.Count;

            object lockTarget = new object();

            Action<Item> action = item =>
            {
                if (ProgressChanged != null)
                {
                    lock (lockTarget)
                    {
                        progress += factor;
                        ProgressChanged(this, new ProgressChangedEventArgs(progress));
                    }
                }

                FileInfo destinationFileName = new FileInfo(item.DestinationFileName);

                if (!destinationFileName.Directory.Exists)
                {
                    destinationFileName.Directory.Create();
                }

                using (Stream source =
                    item.Source is string fileName ? new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize) :
                    item.Source is byte[] byteArray ? new MemoryStream(byteArray) :
                    item.Source is Stream stream ? stream :
                    throw new ArgumentException("Unknown source in item", nameof(item.Source))
                )
                using (Stream destination = destinationFileName.Create())
                {
                    DataStream.CopyPartTo(source, destination, item.Position, item.Length, BufferSize);
                }
            };

            if (EnableThreading)
            {
                Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = MaxThreads }, action);
            }

            else
            {
                items.ForEach(action);
            }
 
            items.Clear();
        }

        public DataExtractor()
        {
            BufferSize = 4096;
            EnableThreading = true;
            MaxThreads = 4;
        }
    }
}
