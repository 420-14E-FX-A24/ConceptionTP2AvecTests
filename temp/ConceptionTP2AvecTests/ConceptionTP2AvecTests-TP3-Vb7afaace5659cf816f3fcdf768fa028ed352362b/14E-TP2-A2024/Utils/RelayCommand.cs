using System;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action<object> _executeWithParam;
    private readonly Action _executeWithoutParam;
    private readonly Func<object, bool> _canExecuteWithParam;
    private readonly Func<bool> _canExecuteWithoutParam;

    public event EventHandler CanExecuteChanged;

    // Constructeur pour les méthodes avec paramètres
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _executeWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecuteWithParam = canExecute;
    }

    // Constructeur pour les méthodes sans paramètres
    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _executeWithoutParam = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecuteWithoutParam = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        // Vérifie si la méthode avec paramètres peut s'exécuter
        if (_canExecuteWithParam != null)
        {
            return _canExecuteWithParam(parameter);
        }

        // Sinon, vérifie si la méthode sans paramètres peut s'exécuter
        return _canExecuteWithoutParam == null || _canExecuteWithoutParam();
    }

    public void Execute(object parameter)
    {
        // Exécute la méthode avec paramètres si elle existe
        if (_executeWithParam != null)
        {
            _executeWithParam(parameter);
        }
        // Sinon, exécute la méthode sans paramètres
        else
        {
            _executeWithoutParam?.Invoke();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

}
