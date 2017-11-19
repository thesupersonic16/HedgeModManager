using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicAudioLib
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public double Progress { get; private set; }

        public ProgressChangedEventArgs(double progress)
        {
            Progress = Math.Round(progress, 2, MidpointRounding.AwayFromZero);
        }
    }

    public delegate void ProgressChanged(object sender, ProgressChangedEventArgs e);
}
