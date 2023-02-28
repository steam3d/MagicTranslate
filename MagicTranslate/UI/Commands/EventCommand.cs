using System;
using System.Windows.Input;

namespace MagicTranslate.UI.Commands
{
    public class EventCommand : ICommand
    {
        private readonly Func<bool> _canExecute = null;
        public event EventHandler CanExecuteChanged;
        public event EventHandler CommandExecuted;

        public EventCommand(){}

        public EventCommand(Func<bool> canExecute)
        {
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            CommandExecuted?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
