using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public partial class ModUpdateEntry
    {
        public CompareResult Compare(ModUpdateEntry item)
        {
            var result = new CompareResult();
            var entries = new Queue<KeyValuePair<ModUpdateEntry, ModUpdateEntry>>(128);
            var addedEntries = new Queue<ModUpdateEntry>(128);

            entries.Enqueue(new KeyValuePair<ModUpdateEntry, ModUpdateEntry>(this, item));

            while (entries.Count != 0)
            {
                var entry = entries.Dequeue();
                var self = entry.Key;
                var other = entry.Value;

                foreach (var selfChild in self)
                {
                    if (!other.Contains(selfChild, out var otherChild))
                    {
                        if (selfChild.Size.HasValue)
                            result.RemovedSize += selfChild.Size.Value;

                        result.RemovedEntries.Add(selfChild);
                        continue;
                    }

                    if (!otherChild.IsFile)
                    {
                        entries.Enqueue(new KeyValuePair<ModUpdateEntry, ModUpdateEntry>(selfChild, otherChild));
                    }
                }

                foreach (var otherChild in other)
                {
                    if (self.Contains(otherChild))
                        continue;

                    if (otherChild.Size.HasValue)
                        result.AddedSize += otherChild.Size.Value;

                    if (otherChild.IsFile)
                        result.AddedEntries.Add(otherChild);

                    foreach (var deepChild in otherChild)
                        if (!deepChild.IsFile)
                            addedEntries.Enqueue(deepChild);
                }
            }

            while (addedEntries.Count != 0)
            {
                var entry = addedEntries.Dequeue();
                foreach (var child in entry)
                {
                    if (child.IsFile)
                    {
                        if (child.Size.HasValue)
                            result.AddedSize += child.Size.Value;

                        result.AddedEntries.Add(child);
                    }

                    foreach (var deepChild in child)
                        if (!deepChild.IsFile)
                            addedEntries.Enqueue(deepChild);
                        else
                        {
                            if (deepChild.Size.HasValue)
                                result.AddedSize += deepChild.Size.Value;

                            result.AddedEntries.Add(deepChild);
                        }
                }
            }

            return result;
        }
    }

    public class CompareResult
    {
        public long AddedSize { get; set; }
        public long RemovedSize { get; set; }

        public List<ModUpdateEntry> AddedEntries { get; set; } = new List<ModUpdateEntry>(64);
        public List<ModUpdateEntry> RemovedEntries { get; set; } = new List<ModUpdateEntry>(64);
    }
}
