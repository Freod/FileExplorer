using System.Windows.Input;

namespace FileExplorer.ViewModels;

public class RelayCommand : ICommand
{
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;
    private EventHandler _requerySuggestedLocal;

    public RelayCommand(Action<object> execute)
        : this(execute, null)
    {
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null) throw new ArgumentNullException("execute");

        _execute = execute;
        if (canExecute != null) _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            if (_canExecute != null)
            {
                var eventHandler = _requerySuggestedLocal;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref _requerySuggestedLocal, value2, eventHandler2);
                } while (eventHandler != eventHandler2);

                CommandManager.RequerySuggested += value;
            }
        }
        remove
        {
            if (_canExecute != null)
            {
                var eventHandler = _requerySuggestedLocal;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    var value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange(ref _requerySuggestedLocal, value2, eventHandler2);
                } while (eventHandler != eventHandler2);

                CommandManager.RequerySuggested -= value;
            }
        }
    }

    public bool CanExecute(object parameter)
    {
        if (_canExecute != null) return _canExecute(parameter);

        return true;
    }

    public virtual void Execute(object parameter)
    {
        if (CanExecute(parameter) && _execute != null) _execute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}