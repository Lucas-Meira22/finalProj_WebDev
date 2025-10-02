using personalFinacialTrack.Resources.Model;
using personalFinacialTrack.Resources.Model.personalFinacialTrack.Resources.Model;

namespace personalFinacialTrack;
public partial class NewGoalPage : ContentPage
{
    private string _selectedColor;
    public NewGoalPage()
    {
        InitializeComponent();
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
        //TODO NAME OF FIELDS
        var goal = new Goal
        {
            //Name = GoalName.text,
            //Amount = decimal.TryParse(GoalAmountEntry.Text, out var amount) ? amount : 0,
            //Currency = CurrencyPicker.SelectedItem?.ToString(),
            //Deadline = DeadlinePicker.Date, // can be optional, handle null if needed
            //Note = GoalNoteEditor.Text,
            //Color = _selectedColor
        };
        await DisplayAlert("Goal Saved",
            $"Name: {goal.Name}\nAmount: {goal.Amount}\nCurrency: {goal.Currency}\nDeadline: {goal.Deadline}\nNote: {goal.Note}\nColor: {goal.Color}",
            "OK");

        // TODO: Save to database, API, or pass back to previous page
        await Navigation.PopAsync();
    }
}