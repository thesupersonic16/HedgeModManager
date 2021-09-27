using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public partial class ModFileEntry : IList<ModFileEntry>, INotifyCollectionChanged
    {
        public const string DirectorySeparatorCharAsString = "/";

        [JsonIgnore]
        public ModFileEntry Parent { get; internal set; }
        
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        [JsonProperty("items", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ObservableCollection<ModFileEntry> Children { get; set; } = new ObservableCollection<ModFileEntry>();

        [JsonIgnore]
        public bool IsFile => Size != null;

        [JsonIgnore]
        public int Count => Children.Count;

        [JsonIgnore]
        public bool IsReadOnly => false;

        [JsonIgnore]
        public string FullPath => BuildPath();

        public ModFileEntry()
        {
            Children.CollectionChanged += OnChildrenChanged;
        }

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
                var entry = new ModFileEntry
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
                var entry = new ModFileEntry();

                entry.ImportFile(file);
                Add(entry);
            }

            Task.WaitAll(tasks.ToArray());
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

        public ModFileEntry Find(string path)
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

        internal ModFileEntry FindItem(string name, bool folder = false)
        {
            return this.FirstOrDefault(e =>
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && e.IsFile == !folder);
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
            var items = new Queue<KeyValuePair<ModFileEntry, ModFileEntry>>(512);

            foreach (var child in this)
            {
                child.Parent = this;
                foreach (var deepChild in child)
                {
                    items.Enqueue(new KeyValuePair<ModFileEntry, ModFileEntry>(child, deepChild));
                }
            }

            while (items.Count != 0)
            {
                var item = items.Dequeue();
                item.Value.Parent = item.Key;

                foreach (var child in item.Value)
                {
                    items.Enqueue(new KeyValuePair<ModFileEntry, ModFileEntry>(item.Value, child));
                }
            }
        }

        public IEnumerator<ModFileEntry> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ModFileEntry item)
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

        public bool Contains(ModFileEntry item)
        {
            return Contains(item, out _);
        }

        public bool Contains(ModFileEntry item, out ModFileEntry foundEntry)
        {
            if (item == null)
            {
                foundEntry = null;
                return false;
            }

            foundEntry =
                this.FirstOrDefault(x =>
                    x.IsFile == item.IsFile && x.Hash == item.Hash &&
                    x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));

            return foundEntry != null;
        }

        public void CopyTo(ModFileEntry[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public bool Remove(ModFileEntry item)
        {
            if (item == null) return false;

            bool removed = Children.Remove(item);
            if (removed)
                item.Parent = null;

            return removed;
        }

        public int IndexOf(ModFileEntry item)
        {
            return Children.IndexOf(item);
        }

        public void Insert(int index, ModFileEntry item)
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

        public ModFileEntry this[int index]
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

        public ModFileEntry Clone()
        {
            var cloned = CloneOne(this);

            var items = new Queue<KeyValuePair<ModFileEntry, ModFileEntry>>(128);
            items.Enqueue(new KeyValuePair<ModFileEntry, ModFileEntry>(this, cloned));

            while (items.Count != 0)
            {
                var item = items.Dequeue();
                foreach (var child in item.Key)
                {
                    var childClone = CloneOne(child);
                    item.Value.Add(childClone);

                    if (item.Key.Count > 0)
                        items.Enqueue(new KeyValuePair<ModFileEntry, ModFileEntry>(child, childClone));
                }
            }

            return cloned;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ModFileEntry CloneOne(ModFileEntry entry)
            {
                return new ModFileEntry
                {
                    Name = entry.Name,
                    Hash = entry.Hash,
                    Size = entry.Size,
                    Parent = entry.Parent
                };
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }
    }
}
