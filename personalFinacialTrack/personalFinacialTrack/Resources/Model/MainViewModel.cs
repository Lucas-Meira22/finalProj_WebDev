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
                    Currency = "USD",
                    Note = "For school and development",
                    Color = "#7A5CFF"
                },
                new Goal
                {
                    Name = "Vacation",
                    Amount = 3000,
                    CurrentAmount = 1200,
                    Currency = "USD",
                    Note = "Trip to Europe",
                    Color = "#FF9800"
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


