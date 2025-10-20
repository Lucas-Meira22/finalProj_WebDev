using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using personalFinacialTrack.Resources.Model;
using System.Linq;
using personalFinacialTrack;

public class GoalsViewModel : BindableObject
{
    private readonly DatabaseHelper _db = new DatabaseHelper();

    public ObservableCollection<Goal> Goals { get; set; } = new ObservableCollection<Goal>();

    private Goal _selectedGoal;
    public Goal SelectedGoal
    {
        get => _selectedGoal;
        set
        {
            _selectedGoal = value;
            OnPropertyChanged();
        }
    }

    private bool _hasGoals;
    public bool HasGoals
    {
        get => _hasGoals;
        set
        {
            _hasGoals = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoadGoalsCommand { get; }
    public ICommand AddGoalCommand { get; }
    public ICommand DeleteGoalCommand { get; }
    public ICommand GoalTappedCommand { get; }

    public GoalsViewModel()
    {
        LoadGoalsCommand = new Command(async () => await LoadGoalsAsync());
        AddGoalCommand = new Command<Goal>(async (goal) => await AddGoalAsync(goal));
        DeleteGoalCommand = new Command(async () => await DeleteGoalAsync());
        GoalTappedCommand = new Command<Goal>(async (goal) => await OnGoalTapped(goal));
    }

    public async Task LoadGoalsAsync()
    {
        Goals.Clear();
        var goalsFromDb = await _db.GetGoalsAsync();
        foreach (var goal in goalsFromDb)
            Goals.Add(goal);

        HasGoals = Goals.Any();
    }

    public async Task AddGoalAsync(Goal newGoal)
    {
        await _db.CreateGoalAsync(newGoal);
        await LoadGoalsAsync();
    }

    public async Task DeleteGoalAsync()
    {
        if (SelectedGoal != null)
        {
            await _db.DeleteGoalAsync(SelectedGoal.GoalId);
            Goals.Remove(SelectedGoal);
            SelectedGoal = null;
            HasGoals = Goals.Any();
        }
    }

    private async Task OnGoalTapped(Goal goal)
    {
        if (goal == null) return;

        await Application.Current.MainPage.Navigation.PushAsync(new GoalDetailsPage(goal));
    }
}


