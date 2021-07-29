using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HedgeModManager.Annotations;

namespace HedgeModManager.UI.Models
{
    public class NotifyTask<T> : INotifyPropertyChanged
    {
        private readonly T mDefaultResult = default(T);

        public event PropertyChangedEventHandler PropertyChanged;
        public Task<T> Task { get; }

        public bool IsSuccessFullyCompleted => Status == TaskStatus.RanToCompletion;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !IsCompleted;
        public bool IsCancelled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public TaskStatus Status => Task.Status;
        public T Result => IsSuccessFullyCompleted ? Task.Result : mDefaultResult;

        public NotifyTask(Task<T> task)
        {
            Task = task;
            _ = RunTask();
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return Task.GetAwaiter();
        }

        private async Task RunTask()
        {
            try
            {
                await Task;
            }
            catch
            {
                // ignored
            }
            finally
            {
                NotifyProperties();
            }
        }

        private void NotifyProperties()
        {
            if (PropertyChanged == null)
                return;

            if (IsCancelled)
                OnPropertyChanged(nameof(IsCancelled));
            else if (IsFaulted)
                OnPropertyChanged(nameof(IsFaulted));
            else
                OnPropertyChanged(nameof(IsSuccessFullyCompleted));

            OnPropertyChanged(nameof(IsCompleted));
            OnPropertyChanged(nameof(IsNotCompleted));
            OnPropertyChanged(nameof(Result));
        }

        public static implicit operator T(NotifyTask<T> item) => item.Result;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // ReSharper disable once PossibleNullReferenceException
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NotifyTask : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Task Task { get; }

        public bool IsSuccessFullyCompleted => Status == TaskStatus.RanToCompletion;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !IsCompleted;
        public bool IsCancelled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public TaskStatus Status => Task.Status;

        public NotifyTask(Task task)
        {
            Task = task;
            _ = RunTask();
        }

        public TaskAwaiter GetAwaiter()
        {
            return Task.GetAwaiter();
        }

        private async Task RunTask()
        {
            try
            {
                await Task;
            }
            catch
            {
                // ignored
            }
            finally
            {
                NotifyProperties();
            }
        }

        private void NotifyProperties()
        {
            if (PropertyChanged == null)
                return;

            if (IsCancelled)
                OnPropertyChanged(nameof(IsCancelled));
            else if (IsFaulted)
                OnPropertyChanged(nameof(IsFaulted));
            else
                OnPropertyChanged(nameof(IsSuccessFullyCompleted));

            OnPropertyChanged(nameof(IsCompleted));
            OnPropertyChanged(nameof(IsNotCompleted));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // ReSharper disable once PossibleNullReferenceException
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
