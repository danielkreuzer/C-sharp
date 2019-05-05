using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wetr.Cockpit.Gui {
    // RelayCommand objects delegate to the methods passed
    // as an argument.
    public class RelayCommand : ICommand {
        readonly Action<object> executeAction;
        readonly Predicate<object> canExecutePredicate;

        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            if (execute == null)
                throw new ArgumentNullException("execute");

            executeAction = execute;
            canExecutePredicate = canExecute;
        }

        // Invoked when command is executed
        public void Execute(object parameter) {
            executeAction(parameter);
        }

        // The returned value indicates whether command is active (can be executed) or not.
        public bool CanExecute(object parameter) {
            return canExecutePredicate == null ? true : canExecutePredicate(parameter);
        }

        // Occurs when changes occur that affect whether or not the command should execute.
        // The handling of this event ist delegated to the CommandManger.
        // WPF invokes CanExecute in response to this event. CanExecute returns the
        // command's state.
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
