using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using personalFinacialTrack.Resources.Model;
using personalFinacialTrack.Resources.Model.personalFinacialTrack.Resources.Model;

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

    public ICommand LoadGoalsCommand { get; }
    public ICommand AddGoalCommand { get; }
    public ICommand DeleteGoalCommand { get; }

    public GoalsViewModel()
    {
        LoadGoalsCommand = new Command(async () => await LoadGoalsAsync());
        AddGoalCommand = new Command<Goal>(async (goal) => await AddGoalAsync(goal));
        DeleteGoalCommand = new Command(async () => await DeleteGoalAsync());
    }

    public async Task LoadGoalsAsync()
    {
        Goals.Clear();
        var goalsFromDb = await _db.GetGoalsAsync();
        foreach (var goal in goalsFromDb)
            Goals.Add(goal);
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
        }
    }

}
