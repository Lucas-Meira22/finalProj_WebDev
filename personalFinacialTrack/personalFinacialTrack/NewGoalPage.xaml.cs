
using personalFinacialTrack.Resources.Model;
using personalFinacialTrack.Resources.Model.personalFinacialTrack.Resources.Model;

namespace personalFinacialTrack;
public partial class NewGoalPage : ContentPage
{
    private string _selectedColor;
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
        }
    }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var goal = new Goal
        {
            Name = EntryName.Text,
            Amount = decimal.TryParse(EntryAmount.Text, out var amt) ? amt : 0,
            Currency = Enum.TryParse<Currency>(EntryCurrency.SelectedItem?.ToString(), out var c)
            ? c
            : Currency.USD,
            Deadline = EntryDeadline.Date,
            Note = EntryNote.Text,
            Color = _selectedColor,
            CurrentAmount = 0
        };

        try
        {
            if (BindingContext is GoalsViewModel vm)
            {
                await vm.AddGoalAsync(goal); // ✅ Call ViewModel method
            }

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