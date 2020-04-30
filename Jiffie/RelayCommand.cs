using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Jiffie
{
    internal class RelayCommand : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> command;

        internal RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        internal RelayCommand(Action<object> command, Predicate<object> canExecute)
        {
            this.command = command;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.command(parameter);
        }
    }
}