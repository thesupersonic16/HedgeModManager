using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class ModProfile : INotifyPropertyChanged
    {
        [JsonIgnore]
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string ModDBPath { get; set; }

        public ModProfile(string name, string modDBPath)
        {
            Name = name;
            ModDBPath = modDBPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
