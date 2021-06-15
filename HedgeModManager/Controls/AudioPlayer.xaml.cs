using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio;
using NAudio.Wave;
using System.Net;
using System.IO;

namespace HedgeModManager.Controls
{
    /// <summary>
    /// Interaction logic for AudioPlayer.xaml
    /// </summary>
    public partial class AudioPlayer : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(AudioPlayer));
        public event PropertyChangedEventHandler PropertyChanged;

        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public bool IsPlaying
        {
            get => SoundOutput?.PlaybackState == PlaybackState.Playing;
            set
            {
                if (value)
                    Play();
                else
                    Pause();

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsPlaying)));
            }
        }

        protected bool WasPlaying;
        protected DispatcherTimer MediaTimer;
        protected WaveStream AudioReader;
        protected IWavePlayer SoundOutput;

        public AudioPlayer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Play()
        {
            if (AudioReader == null && !InitializeSource())
                return;

            MediaTimer.Start();
            SoundOutput.Play();
        }

        public void Pause()
        {
            SoundOutput.Pause();
        }

        public void Stop()
        {
            if (AudioReader == null)
                return;

            IsPlaying = false;
            AudioReader.Position = 0;
            Slider.Value = 0;
            MediaTimer.Stop();

            if (SoundOutput.PlaybackState != PlaybackState.Stopped)
                SoundOutput.Stop();
        }

        public bool InitializeSource()
        {
            if (Source == null)
                return false;

            AudioReader = new MediaFoundationReader(Source.ToString());
            SoundOutput = new DirectSoundOut();
            SoundOutput.Init(AudioReader);
            SoundOutput.PlaybackStopped += (s, en) =>
            {
                Stop();
            };

            return true;
        }

        private void Player_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
            MediaTimer?.Stop();
            SoundOutput?.Stop();
            AudioReader?.Close();
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            MediaTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(1)
            };

            MediaTimer.Tick += MediaTimer_Tick;
        }

        private void MediaTimer_Tick(object sender, EventArgs e)
        {
            if (SoundOutput.PlaybackState == PlaybackState.Playing)
            {
                Slider.Maximum = AudioReader.Length;
                Slider.Value = AudioReader.Position;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (AudioReader != null)
            {
                AudioReader.Position = (long)Slider.Value;

                if (WasPlaying)
                {
                    Play();
                    WasPlaying = false;
                }
            }
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (AudioReader != null)
            {
                WasPlaying = IsPlaying;
                Pause();
            }
        }
    }
}
