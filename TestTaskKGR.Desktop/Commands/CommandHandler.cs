using System.Windows.Input;

namespace TestTaskKGR.Desktop.Commands;

public class CommandHandler : ICommand
{
    public Action<object> Method { get; set; }

    public Func<object, bool> CanExecuteMethod { get; set; }
    public CommandHandler(Action<object> method, Func<object, bool> canExecuteMethod = null)
    {
        this.Method = method;
        this.CanExecuteMethod = canExecuteMethod;
    }
    public CommandHandler() { }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter) => CanExecuteMethod(parameter);

    public void Execute(object parameter) => Method(parameter);
}
