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
        //TODO NAME OF FIELDS
        var goal = new Goal
        {
            Name = EntryName.Text,
            Amount = decimal.TryParse(EntryAmount.Text, out var amt) ? amt : 0,
            Currency = (Currency)EntryCurrency.SelectedItem,
            Deadline = EntryDeadline.Date,
            Note = EntryNote.Text,
            Color = _selectedColor
        };



        //Pop up a confirmation dialog
        await DisplayAlert("Goal Saved",
            $"Name: {goal.Name}\nAmount: {goal.Amount}\nCurrency: {goal.Currency}\nDeadline: {goal.Deadline}\nNote: {goal.Note}\nColor: {goal.Color}",
            "OK");

        // TODO: Save to database, API, or pass back to previous page
        
        //Sends back to previous page
        await Navigation.PopAsync();
    }
}