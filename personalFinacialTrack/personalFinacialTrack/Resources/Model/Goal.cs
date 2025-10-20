using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace personalFinacialTrack.Resources.Model
{
    public class Goal : INotifyPropertyChanged
    {
        public int GoalId { get; set; } // Primary key

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private decimal _amount = 0m;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); OnPropertyChanged(nameof(Progress)); OnPropertyChanged(nameof(Remaining)); }
        }

        private Currency _currency = Currency.USD;
        public Currency Currency
        {
            get => _currency;
            set { _currency = value; OnPropertyChanged(); }
        }

        private DateTime? _deadline;
        public DateTime? Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(); }
        }

        private string _note = string.Empty;
        public string Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(); }
        }

        private string _color = "#FFFFFF";
        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        private decimal _currentAmount = 0m;
        public decimal CurrentAmount
        {
            get => _currentAmount;
            set
            {
                if (_currentAmount == value) return;
                _currentAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(Remaining));
                OnPropertyChanged(nameof(DailySavings));
                OnPropertyChanged(nameof(WeeklySavings));
                OnPropertyChanged(nameof(MonthlySavings));
            }
        }

        // Progress as 0.0..1.0 (safe)
        public double Progress
        {
            get
            {
                if (Amount <= 0) return 0;
                var p = (double)(CurrentAmount / Amount);
                if (double.IsNaN(p) || double.IsInfinity(p)) return 0;
                return Math.Clamp(p, 0.0, 1.0);
            }
        }

        // Remaining
        public decimal Remaining => (Amount - CurrentAmount) < 0 ? 0 : (Amount - CurrentAmount);

        public decimal DailySavings
        {
            get
            {
                if (Deadline.HasValue && Deadline.Value > DateTime.Now)
                {
                    var days = (Deadline.Value - DateTime.Now).TotalDays;
                    if (days <= 0) return 0m;
                    return Math.Round(Remaining / (decimal)days, 2);
                }
                return Math.Round(Remaining / 30m, 2);
            }
        }

        public decimal WeeklySavings
        {
            get
            {
                if (Deadline.HasValue && Deadline.Value > DateTime.Now)
                {
                    var weeks = (Deadline.Value - DateTime.Now).TotalDays / 7.0;
                    if (weeks <= 0) return 0m;
                    return Math.Round(Remaining / (decimal)weeks, 2);
                }
                return Math.Round(Remaining / 4m, 2);
            }
        }

        public decimal MonthlySavings
        {
            get
            {
                if (Deadline.HasValue && Deadline.Value > DateTime.Now)
                {
                    var months = (Deadline.Value - DateTime.Now).TotalDays / 30.0;
                    if (months <= 0) return 0m;
                    return Math.Round(Remaining / (decimal)months, 2);
                }
                return Math.Round(Remaining / 1m, 2);
            }
        }

        // Placeholder for GraphicsView Drawable binding
        public IDrawable ProgressDrawable => null;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}






