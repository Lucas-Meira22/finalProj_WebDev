using Microsoft.Maui.Controls;
using personalFinacialTrack.Resources.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace personalFinacialTrack
{
    public partial class TransactionsPage : ContentPage
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

        public TransactionsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTransactionsAsync();
        }

        private async Task LoadTransactionsAsync()
        {
            Transactions.Clear();
            var list = await _db.GetTransactionsAsync(200);
            foreach (var t in list) Transactions.Add(t);

            EmptyLabel.IsVisible = !Transactions.Any();
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            await LoadTransactionsAsync();
        }

        private async void OnAddTransactionClicked(object sender, EventArgs e)
        {

            string amountStr = await DisplayPromptAsync("Add Transaction", "Amount:", "Next", "Cancel", "0", keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(amountStr)) return;
            if (!decimal.TryParse(amountStr, out var amount))
            {
                await DisplayAlert("Error", "Invalid amount", "OK");
                return;
            }

            string type = await DisplayActionSheet("Type", "Cancel", null, "Expense", "Income");
            if (type == "Cancel") return;

            // pick a category quickly: show the list of names
            var cats = await _db.GetCategoriesAsync();
            string[] catNames = cats.Select(c => c.Name).ToArray();
            string chosen = await DisplayActionSheet("Category", "None", null, catNames);
            int? catId = null;
            if (chosen != "None" && !string.IsNullOrEmpty(chosen))
            {
                var sel = cats.FirstOrDefault(c => c.Name == chosen);
                if (sel != null) catId = sel.CategoryId;
            }

            string note = await DisplayPromptAsync("Note", "Optional note", "Save", "Cancel", "", -1, keyboard: Keyboard.Default);
            var tx = new Transaction
            {
                Amount = amount,
                Type = type == "Income" ? TransactionType.Income : TransactionType.Expense,
                CategoryId = catId,
                Note = note,
                Date = DateTime.Now
            };

            try
            {
                await _db.CreateTransactionAsync(tx);
                await LoadTransactionsAsync();
                await DisplayAlert("Saved", "Transaction saved", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}