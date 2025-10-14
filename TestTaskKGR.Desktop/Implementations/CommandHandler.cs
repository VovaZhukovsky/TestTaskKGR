using System;
using System.Windows.Input;

namespace TestTaskKGR.Desktop.Implementations;

public class CommandHandler : ICommand
{
    public CommandHandler(Action<object> method, Func<object, bool> canExecuteMethod = null)
        {
            this.Method = method;
            this.CanExecuteMethod = canExecuteMethod;
        }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter) => CanExecuteMethod(parameter);

    public void Execute(object parameter) => Method(parameter);

    public Action<object> Method { get; set; }

    public Func<object, bool> CanExecuteMethod { get; set; }
}
