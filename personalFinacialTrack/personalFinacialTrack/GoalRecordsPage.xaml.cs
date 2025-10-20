using personalFinacialTrack.Resources.Model;

namespace personalFinacialTrack;

public partial class GoalRecordsPage : ContentPage
{
    private readonly DatabaseHelper db = new();
    private readonly int goalId;

    public GoalRecordsPage(int goalId)
    {
        InitializeComponent();
        this.goalId = goalId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var records = await db.GetGoalRecordsAsync(goalId);
        recordsList.ItemsSource = records;
    }
}
