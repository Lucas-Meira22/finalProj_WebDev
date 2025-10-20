using System;
using Microsoft.Maui.Controls;
using personalFinacialTrack.Resources.Model;
using System.Threading.Tasks;

namespace personalFinacialTrack
{
    public partial class GoalDetailsPage : ContentPage
    {
        private Goal currentGoal;
        private readonly DatabaseHelper _db = new DatabaseHelper();

        public GoalDetailsPage(Goal goal)
        {
            InitializeComponent();
            currentGoal = goal;
            BindingContext = currentGoal;
        }

        //  Add Savings (deposit)
        private async void OnAddSavingsClicked(object sender, EventArgs e)
        {
            string input = await DisplayPromptAsync("Add Savings", "Enter amount to add:", "OK", "Cancel", "0", keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(input)) return;

            if (!decimal.TryParse(input, out decimal amountToAdd) || amountToAdd <= 0)
            {
                await DisplayAlert("Error", "Please enter a valid positive number.", "OK");
                return;
            }

            // Update in-memory immediately
            currentGoal.CurrentAmount += amountToAdd;

            try
            {
                // Update goal in DB
                await _db.UpdateGoalAsync(currentGoal);

                // Register the record (Type must be exactly 'Add')
                await _db.AddGoalRecordAsync(currentGoal.GoalId, amountToAdd, "Add", "Manual addition");

                // Refresh from DB to keep authoritative values
                var fresh = await GetFreshGoalFromDb(currentGoal.GoalId);
                if (fresh != null)
                    currentGoal.CurrentAmount = fresh.CurrentAmount;

                await DisplayAlert("Success", $"You added ${amountToAdd}. Total saved: ${currentGoal.CurrentAmount}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Database update failed:\n{ex.Message}", "OK");
            }
        }

        // Withdraw
        private async void OnWithdrawClicked(object sender, EventArgs e)
        {
            string input = await DisplayPromptAsync("Withdraw", "Enter amount to withdraw:", "OK", "Cancel", "0", keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(input)) return;

            if (!decimal.TryParse(input, out decimal amountToWithdraw) || amountToWithdraw <= 0)
            {
                await DisplayAlert("Error", "Please enter a valid positive number.", "OK");
                return;
            }

            if (amountToWithdraw > currentGoal.CurrentAmount)
            {
                await DisplayAlert("Error", "You cannot withdraw more than the current saved amount.", "OK");
                return;
            }

            // Update in-memory immediately 
            currentGoal.CurrentAmount -= amountToWithdraw;

            try
            {
                // Update goal in DB
                await _db.UpdateGoalAsync(currentGoal);

                // Register the withdrawal record 
                await _db.AddGoalRecordAsync(currentGoal.GoalId, amountToWithdraw, "Withdraw", "Manual withdrawal");

                // Refresh data
                var fresh = await GetFreshGoalFromDb(currentGoal.GoalId);
                if (fresh != null)
                    currentGoal.CurrentAmount = fresh.CurrentAmount;

                await DisplayAlert("Success", $"You withdrew ${amountToWithdraw}. Total saved: ${currentGoal.CurrentAmount}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Database update failed:\n{ex.Message}", "OK");
            }
        }

        // Delete Goal
        private async void OnDeleteGoalClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete '{currentGoal.Name}'?", "Yes", "No");
            if (!confirm) return;

            try
            {
                await _db.DeleteGoalAsync(currentGoal.GoalId);
                await DisplayAlert("Deleted", $"Goal '{currentGoal.Name}' was deleted successfully.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Database deletion failed:\n{ex.Message}", "OK");
            }
        }

        // View goal transaction records
        private async void OnViewRecordsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GoalRecordsPage(currentGoal.GoalId));
        }

        // Helper to refresh goal info
        private async Task<Goal> GetFreshGoalFromDb(int goalId)
        {
            var all = await _db.GetGoalsAsync();
            return all.Find(g => g.GoalId == goalId);
        }
    }
}










