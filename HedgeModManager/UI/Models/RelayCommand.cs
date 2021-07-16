using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HedgeModManager.UI.Models
{
    public class RelayCommand : ICommand
    {
        public readonly Action ExecuteFunc;
        public readonly Func<bool> CanExecuteFunc;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action executeFunc, Func<bool> canExecuteFunc = null)
        {
            ExecuteFunc = executeFunc;
            CanExecuteFunc = canExecuteFunc;
        }

        public bool CanExecute(object parameter)
        {
            return ExecuteFunc != null && (CanExecuteFunc == null || CanExecuteFunc());
        }

        public void Execute(object parameter)
        {
            ExecuteFunc();
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
