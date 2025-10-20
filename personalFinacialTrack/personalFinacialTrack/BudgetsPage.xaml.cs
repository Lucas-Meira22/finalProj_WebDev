using Microsoft.Maui.Controls;
using personalFinacialTrack.Resources.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace personalFinacialTrack
{
    public partial class BudgetsPage : ContentPage
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        public ObservableCollection<dynamic> BudgetsDisplayed { get; set; } = new ObservableCollection<dynamic>();

        public BudgetsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            BudgetsDisplayed.Clear();
            var now = DateTime.Now;
            var budgets = await _db.GetBudgetsAsync(now.Year, now.Month);
            var categories = await _db.GetCategoriesAsync();
            foreach (var b in budgets)
            {
                var cat = categories.FirstOrDefault(c => c.CategoryId == b.CategoryId);
                BudgetsDisplayed.Add(new
                {
                    b.BudgetId,
                    CategoryName = cat?.Name ?? "(Unknown)",
                    b.LimitAmount
                });
            }
            BudgetsCollection.ItemsSource = BudgetsDisplayed;
        }

        private async void OnRefreshClicked(object sender, EventArgs e) => await LoadAsync();

        private async void OnNewBudgetClicked(object sender, EventArgs e)
        {
            var cats = await _db.GetCategoriesAsync();
            var options = cats.Select(c => c.Name).ToArray();
            string chosen = await DisplayActionSheet("Select category", "Cancel", null, options);
            if (chosen == "Cancel") return;
            var cat = cats.FirstOrDefault(c => c.Name == chosen);
            if (cat == null) return;

            string limitStr = await DisplayPromptAsync("Set monthly limit", "Amount:", "Save", "Cancel", "0", keyboard: Keyboard.Numeric);
            if (!decimal.TryParse(limitStr, out var limit) || limit <= 0) { await DisplayAlert("Error", "Invalid amount", "OK"); return; }

            var b = new Budget
            {
                CategoryId = cat.CategoryId,
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                LimitAmount = limit
            };

            await _db.CreateOrUpdateBudgetAsync(b);
            await LoadAsync();
            await DisplayAlert("Saved", "Budget saved/updated", "OK");
        }
    }
}
