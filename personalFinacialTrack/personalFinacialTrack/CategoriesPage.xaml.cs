using Microsoft.Maui.Controls;
using personalFinacialTrack.Resources.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace personalFinacialTrack
{
    public partial class CategoriesPage : ContentPage
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        public CategoriesPage()
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
            Categories.Clear();
            var list = await _db.GetCategoriesAsync();
            foreach (var c in list)
                Categories.Add(c);
        }

        private async void OnRefreshClicked(object sender, EventArgs e) => await LoadAsync();

        private async void OnNewCategoryClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("New Category", "Enter name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string type = await DisplayActionSheet("Select type", "Cancel", null, "Expense", "Income");
            if (type == "Cancel" || string.IsNullOrEmpty(type)) return;

            var cat = new Category { Name = name, Type = type == "Income" ? CategoryType.Income : CategoryType.Expense };
            await _db.CreateCategoryAsync(cat);
            await LoadAsync();
        }

        private async void OnEditCategoryClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Category category)
            {
                string newName = await DisplayPromptAsync("Edit Category", "Update name:", initialValue: category.Name);
                if (string.IsNullOrWhiteSpace(newName)) return;

                string newType = await DisplayActionSheet("Select type", "Cancel", null, "Expense", "Income");
                if (newType == "Cancel" || string.IsNullOrEmpty(newType)) return;

                category.Name = newName;
                category.Type = newType == "Income" ? CategoryType.Income : CategoryType.Expense;

                await _db.UpdateCategoryAsync(category);
                await LoadAsync();
            }
        }

        private async void OnDeleteCategoryClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Category category)
            {
                bool confirm = await DisplayAlert("Delete", $"Delete '{category.Name}'?", "Yes", "No");
                if (!confirm) return;

                await _db.DeleteCategoryAsync(category.CategoryId);
                await LoadAsync();
            }
        }
    }
}
