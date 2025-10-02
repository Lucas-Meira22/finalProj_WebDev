using personalFinacialTrack.Resources.Model.personalFinacialTrack.Resources.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace personalFinacialTrack.Resources.Model
{
    public class MainViewModel
    {
        public ObservableCollection<Goal> Goals { get; set; }

        public MainViewModel()
        {
            Goals = new ObservableCollection<Goal>
            {
                new Goal
                {
                    Name = "Buy a Laptop",
                    Amount = 1500,
                    CurrentAmount = 600,
                    Currency = Currency.USD,
                    Note = "For school and development",
                    Color = "#7A5CFF"
                },
                new Goal
                {
                    Name = "Vacation",
                    Amount = 3000,
                    CurrentAmount = 1200,
                    Currency = Currency.USD,
                    Note = "Trip to Europe",
                    Color = "#FF9800"
                },
                new Goal
                {
                    Name = "Emergency Fund",
                    Amount = 5000,
                    CurrentAmount = 2000,
                    Currency = Currency.USD,
                    Note = "3 months of living expenses",
                    Color = "#4CAF50"
                },
                new Goal
                {
                    Name = "New Phone",
                    Amount = 1200,
                    CurrentAmount = 400,
                    Currency = Currency.USD,
                    Note = "Upgrade to the latest model",
                    Color = "#2196F3"
                },
                new Goal
                {
                    Name = "Gym Membership",
                    Amount = 600,
                    CurrentAmount = 150,
                    Currency = Currency.USD,
                    Note = "Annual fitness plan",
                    Color = "#9C27B0"
                },
                new Goal
                {
                    Name = "Gaming Console",
                    Amount = 700,
                    CurrentAmount = 250,
                    Currency = Currency.USD,
                    Note = "Next-gen console release",
                    Color = "#FF5722"
                },
                new Goal
                {
                    Name = "Car Down Payment",
                    Amount = 8000,
                    CurrentAmount = 3000,
                    Currency = Currency.USD,
                    Note = "Save for a reliable car",
                    Color = "#00BCD4"
                }

            };

            Goals.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasGoals));
            };
        }

        // Computed properties
        public bool IsEmpty => Goals.Count == 0;
        public bool HasGoals => Goals.Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


