using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.Annotations;

namespace HedgeModManager
{
    public class StringLogger : ILogger, INotifyPropertyChanged
    {
        private StringBuilder TextBuilder { get; set; } = new StringBuilder(2048);
        public string Text => TextBuilder.ToString();

        public void Write(string str)
        {
            TextBuilder.Append(str);
            OnPropertyChanged(nameof(Text));
        }

        public void WriteLine(string str)
        {
            TextBuilder.AppendLine(str);
            OnPropertyChanged(nameof(Text));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
