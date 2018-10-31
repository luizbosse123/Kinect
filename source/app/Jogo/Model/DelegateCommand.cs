using System;
using System.Windows.Input;

namespace Jogo.Model
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> action)
        {
            this._action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
