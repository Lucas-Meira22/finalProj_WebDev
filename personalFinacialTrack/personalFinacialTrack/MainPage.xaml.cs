using System.Threading.Tasks;

namespace personalFinacialTrack
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void CreateNewGoal(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewGoalPage());
        }

    }
}
