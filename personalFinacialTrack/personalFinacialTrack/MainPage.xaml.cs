using personalFinacialTrack.Resources.Model;
using personalFinacialTrack.Resources.Model.personalFinacialTrack.Resources.Model;
using System.Threading.Tasks;

namespace personalFinacialTrack
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private GoalsViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new GoalsViewModel();
            BindingContext = _viewModel;
        }

        private async void CreateNewGoal(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewGoalPage());
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadGoalsAsync();
        }
        private async void OnGoalTapped(Goal goal)
        {
            if (goal == null) return;

            // Navigate to details page
            await Application.Current.MainPage.Navigation.PushAsync(new GoalDetailsPage { BindingContext = goal});
            
        }
    }
}
