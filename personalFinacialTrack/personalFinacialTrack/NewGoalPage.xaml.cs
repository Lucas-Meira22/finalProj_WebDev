using System;
using personalFinacialTrack.Resources.Model;
using Microsoft.Maui.Controls;

namespace personalFinacialTrack
{
    public partial class NewGoalPage : ContentPage
    {
        private string _selectedColor;
        private readonly DatabaseHelper _db = new DatabaseHelper();

        public NewGoalPage()
        {
            InitializeComponent();
            BindingContext = new NewGoalViewModel();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnColorSelected(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter != null)
            {
                _selectedColor = btn.CommandParameter.ToString();

                // Reset all buttons’ borders
                foreach (var child in ((HorizontalStackLayout)btn.Parent).Children)
                {
                    if (child is Button b)
                        b.BorderColor = Colors.Transparent;
                }

                // Highlight selected
                btn.BorderColor = Colors.White;
                btn.BorderWidth = 3;
            }
        }


        private async void OnSaveClicked(object sender, EventArgs e)
        {

            var goal = new Goal
            {
                Name = EntryName?.Text ?? string.Empty,
                Amount = decimal.TryParse(EntryAmount?.Text, out var amt) ? amt : 0,
                Currency = Enum.TryParse<Currency>(EntryCurrency?.SelectedItem?.ToString(), out var c) ? c : Currency.USD,
                Deadline = EntryDeadline?.Date,
                Note = EntryNote?.Text ?? string.Empty,
                Color = string.IsNullOrEmpty(_selectedColor) ? "#5C3EE8" : _selectedColor,
                CurrentAmount = 0
            };

            try
            {
                await _db.CreateGoalAsync(goal);

                await DisplayAlert("Goal Saved",
                    $"Name: {goal.Name}\nAmount: {goal.Amount}\nCurrency: {goal.Currency}\nDeadline: {goal.Deadline}\nNote: {goal.Note}\nColor: {goal.Color}",
                    "OK");

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not save goal: {ex.Message}", "OK");
            }
        }
    }
}