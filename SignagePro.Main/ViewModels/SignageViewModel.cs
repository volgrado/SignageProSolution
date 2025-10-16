using SignagePro.Core;
using SignagePro.Core.Contracts;
using SignagePro.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SignagePro.Main.ViewModels
{
    public class SignageViewModel : INotifyPropertyChanged
    {
        private readonly SignageCalculationService _calculationService;
        private readonly IAutoCADAdapter _autocadAdapter;
        private SignalData? _selectedSignal;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<SignalData> AvailableSignals { get; }
        public ICommand CreateSignalCommand { get; }

        public SignalData? SelectedSignal
        {
            get => _selectedSignal;
            set
            {
                if (_selectedSignal != value)
                {
                    _selectedSignal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSignal)));
                    // Notificar que la condición de CanExecute puede haber cambiado
                    (CreateSignalCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public SignageViewModel(SignageCalculationService calculationService, IAutoCADAdapter autocadAdapter)
        {
            _calculationService = calculationService;
            _autocadAdapter = autocadAdapter;
            AvailableSignals = new ObservableCollection<SignalData>(_calculationService.GetAllSignals());
            CreateSignalCommand = new RelayCommand(CreateSignal, () => SelectedSignal != null);
        }

        private void CreateSignal()
        {
            if (SelectedSignal != null)
            {
                _autocadAdapter.DrawSignal(SelectedSignal);
            }
        }
    }

    // Clase de ayuda para implementar ICommand de forma sencilla
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
