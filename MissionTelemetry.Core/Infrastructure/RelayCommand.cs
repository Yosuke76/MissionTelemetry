using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MissionTelemetry.Core.Infrastructure
{
    public sealed class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

       
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(execute is null ? throw new ArgumentNullException(nameof(execute)) : new Action<object?>(_ => execute()),
                   canExecute is null ? null : new Predicate<object?>(_ => canExecute()))
        { }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)                  // steuert ob ein Button aktiv ist 
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
