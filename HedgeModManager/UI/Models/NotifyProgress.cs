using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Annotations;

namespace HedgeModManager.UI.Models
{
    public class NotifyProgress<T> : IProgress<T>, INotifyPropertyChanged
    {
        public T Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Report(T value)
        {
            Value = value;
        }
    }
}
