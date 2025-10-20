using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace personalFinacialTrack.Resources.Model
{
    public class NewGoalViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Currency> Currencies { get; }

        private Currency _selectedCurrency;
        public Currency SelectedCurrency
        {
            get => _selectedCurrency;
            set { _selectedCurrency = value; OnPropertyChanged(); }
        }

        private string _goalName;
        public string GoalName
        {
            get => _goalName;
            set { _goalName = value; OnPropertyChanged(); }
        }

        private decimal _goalAmount;
        public decimal GoalAmount
        {
            get => _goalAmount;
            set { _goalAmount = value; OnPropertyChanged(); }
        }

        private DateTime _deadline = DateTime.Now;
        public DateTime Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(); }
        }

        private string _note;
        public string Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(); }
        }

        private string _color;
        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        public NewGoalViewModel()
        {
            Currencies = new ObservableCollection<Currency>(
                Enum.GetValues(typeof(Currency)) as Currency[] ?? Array.Empty<Currency>()
            );
            SelectedCurrency = Currency.USD; // default
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


