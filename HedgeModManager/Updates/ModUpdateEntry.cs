using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager.Updates
{
    [JsonObject]
    public partial class ModUpdateEntry : IList<ModUpdateEntry>
    {
        public const string DirectorySeparatorCharAsString = "/";

        [JsonIgnore]
        public ModUpdateEntry Parent { get; internal set; }
        
        [JsonIgnore]
        public string Name { get; set; }

        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        [JsonProperty("items", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ModUpdateEntry> Children { get; set; } = new List<ModUpdateEntry>();

        [JsonIgnore]
        public bool IsFile => Size != null;

        [JsonIgnore]
        public int Count => Children.Count;

        [JsonIgnore]
        public bool IsReadOnly => false;

        [JsonIgnore]
        public string FullPath => BuildPath();

        public void ImportDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
                return;

            Clear();

            var tasks = new List<Task>(16);
            foreach (string dir in Directory.EnumerateDirectories(path))
            {
                var entry = new ModUpdateEntry
                {
                    Name = Path.GetFileName(dir),
                };

                Add(entry);
                
                tasks.Add(Task.Run(() =>
                {
                    entry.ImportDirectory(dir);
                }));
            }

            foreach (string file in Directory.EnumerateFiles(path))
            {
                var entry = new ModUpdateEntry();

                entry.ImportFile(file);
                Add(entry);
            }

            Task.WaitAll(tasks.ToArray());

            //Parallel.ForEach(Directory.EnumerateFiles(path), file =>
            //{
            //    var entry = new ModUpdateEntry
            //    {
            //        Name = Path.GetFileName(file),
            //        Hash = HedgeApp.ComputeMD5Hash(file)
            //    };

            //    Add(entry);
            //});
        }

        public string BuildPath()
        {
            var names = new Stack<string>(4);
            var current = this;
            
            while (current.Parent != null)
            {
                names.Push(current.Name);
                current = current.Parent;
            }

            string path = string.Join(DirectorySeparatorCharAsString, names);
            return IsFile ? path : $"{path}{DirectorySeparatorCharAsString}";
        }

        public ModUpdateEntry Find(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            bool isDir = path.EndsWith(DirectorySeparatorCharAsString) || path.EndsWith(Path.DirectorySeparatorChar.ToString());
            string[] names = path.Split(new[] {DirectorySeparatorCharAsString[0], Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 0)
                return this;

            var currentItem = this;
            for (int i = 0; i < names.Length; i++)
            {
                if (currentItem == null)
                    return null;

                string name = names[i];
                bool isLast = i == names.Length - 1;

                switch (name)
                {
                    case ".":
                        break;

                    case "..":
                        currentItem = currentItem.Parent;
                        break;

                    default:
                        currentItem = currentItem.FindItem(name, !isLast || isDir);
                        break;
                }

                if (isLast)
                    return currentItem;
            }

            return null;
        }

        internal ModUpdateEntry FindItem(string name, bool folder = false)
        {
            return this.FirstOrDefault(e => e.Name == name && e.IsFile == !folder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ImportFile(string path)
        {
            Name = Path.GetFileName(path);

            try
            {
                using var md5 = MD5.Create();
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x4000);

                byte[] hash = md5.ComputeHash(stream);
                var builder = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    builder.Append($"{b:X2}");

                Hash = builder.ToString();
                Size = stream.Length;
            }
            catch
            {
                Hash = string.Empty;
                Size = 0;
            }
        }

        internal void FixChildren()
        {
            var items = new Queue<KeyValuePair<ModUpdateEntry, ModUpdateEntry>>(512);

            foreach (var child in this)
            {
                child.Parent = this;
                foreach (var deepChild in child)
                {
                    items.Enqueue(new KeyValuePair<ModUpdateEntry, ModUpdateEntry>(child, deepChild));
                }
            }

            while (items.Count != 0)
            {
                var item = items.Dequeue();
                item.Value.Parent = item.Key;

                foreach (var child in item.Value)
                {
                    items.Enqueue(new KeyValuePair<ModUpdateEntry, ModUpdateEntry>(item.Value, child));
                }
            }
        }

        public IEnumerator<ModUpdateEntry> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ModUpdateEntry item)
        {
            if (item == null) return;
            
            item.Parent = this;
            Children.Add(item);
        }

        public void Clear()
        {
            foreach (var child in Children)
                child.Parent = null;

            Children.Clear();
        }

        public bool Contains(ModUpdateEntry item)
        {
            return Contains(item, out _);
        }

        public bool Contains(ModUpdateEntry item, out ModUpdateEntry foundEntry)
        {
            if (item == null)
            {
                foundEntry = null;
                return false;
            }

            foundEntry =
                this.FirstOrDefault(x => x.IsFile == item.IsFile && x.Hash == item.Hash && x.Name == item.Name);

            return foundEntry != null;
        }

        public void CopyTo(ModUpdateEntry[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public bool Remove(ModUpdateEntry item)
        {
            if (item == null) return false;

            bool removed = Children.Remove(item);
            if (removed)
                item.Parent = null;

            return removed;
        }

        public int IndexOf(ModUpdateEntry item)
        {
            return Children.IndexOf(item);
        }

        public void Insert(int index, ModUpdateEntry item)
        {
            if (item != null)
            {
                item.Parent = this;
                Children.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            var item = Children[index];
            item.Parent = null;
            Children.RemoveAt(index);
        }

        public ModUpdateEntry this[int index]
        {
            get => Children[index];
            set
            {
                var child = Children[index];
                child.Parent = null;

                value.Parent = this;
                Children[index] = value;
            }
        }

        public override string ToString()
        {
            return IsFile ? Name : $"{Name}{DirectorySeparatorCharAsString}";
        }

        public ModUpdateEntry Clone()
        {
            var cloned = CloneOne(this);

            var items = new Queue<KeyValuePair<ModUpdateEntry, ModUpdateEntry>>(128);
            items.Enqueue(new KeyValuePair<ModUpdateEntry, ModUpdateEntry>(this, cloned));

            while (items.Count != 0)
            {
                var item = items.Dequeue();
                foreach (var child in item.Key)
                {
                    var childClone = CloneOne(child);
                    item.Value.Add(childClone);

                    if (item.Key.Count > 0)
                        items.Enqueue(new KeyValuePair<ModUpdateEntry, ModUpdateEntry>(child, childClone));
                }
            }

            return cloned;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ModUpdateEntry CloneOne(ModUpdateEntry entry)
            {
                return new ModUpdateEntry
                {
                    Name = entry.Name,
                    Hash = entry.Hash,
                    Size = entry.Size,
                    Parent = entry.Parent
                };
            }
        }
    }
}
