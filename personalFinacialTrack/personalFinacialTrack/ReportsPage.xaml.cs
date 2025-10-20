using Microsoft.Maui.Controls;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personalFinacialTrack.Resources.Model;

namespace personalFinacialTrack
{
    public partial class ReportsPage : ContentPage
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        public ReportsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadChartsAsync();
        }

        private async Task LoadChartsAsync()
        {
            try
            {
                PieChartView.Chart = null; 
                BarChartView.Chart = null;

                var categories = await _db.GetCategoriesAsync();
                var now = DateTime.Now;
                int year = now.Year;
                int month = now.Month;

                // PIE: expense breakdown by category for current month 
                var pieEntries = new List<ChartEntry>();
                var expenseCats = categories.Where(c => c.Type == CategoryType.Expense).ToList();

                foreach (var cat in expenseCats)
                {
                    var total = await _db.GetTotalForCategoryInMonthAsync(cat.CategoryId, year, month);
                    if (total <= 0) continue;
                    var color = SKColor.Parse(RandomColorForString(cat.Name));
                    pieEntries.Add(new ChartEntry((float)total)
                    {
                        Label = cat.Name,
                        ValueLabel = total.ToString("C0"),
                        Color = color,
                        ValueLabelColor = SKColors.White,  
                        TextColor = SKColors.White         
                    });
                }

                PieChartView.Chart = new PieChart
                {
                    Entries = pieEntries,
                    LabelTextSize = 24f,
                    HoleRadius = 0.5f,
                    LabelMode = LabelMode.RightOnly, 
                    BackgroundColor = SKColor.Empty
                };

                // BAR: monthly net (income - expense)
                var monthsBack = 4;
                var barEntries = new List<ChartEntry>();
                var txs = await _db.GetTransactionsAsync(1000); 

                for (int i = monthsBack - 1; i >= 0; i--)
                {
                    var dt = now.AddMonths(-i);
                    var y = dt.Year;
                    var m = dt.Month;

                    var monthTxs = txs.Where(t => t.Date.Year == y && t.Date.Month == m);
                    decimal net = 0m;
                    foreach (var t in monthTxs)
                        net += (t.Type == TransactionType.Income) ? t.Amount : -t.Amount;

                    var label = dt.ToString("MMM");
                    var color = net >= 0 ? SKColor.Parse("#2ECC71") : SKColor.Parse("#E74C3C");

                    barEntries.Add(new ChartEntry((float)Math.Abs((double)net))
                    {
                        Label = label,
                        ValueLabel = net.ToString("C0"),
                        Color = color,
                        ValueLabelColor = SKColors.White,  
                        TextColor = SKColors.White         
                    });
                }

                BarChartView.Chart = new BarChart
                {
                    Entries = barEntries,
                    LabelTextSize = 20f,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelOrientation = Orientation.Horizontal,
                    BackgroundColor = SKColor.Empty
                };
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not load reports: {ex.Message}", "OK");
            }
        }


        // Helper: deterministic-ish color from string
        private string RandomColorForString(string s)
        {
            unchecked
            {
                int hash = 23;
                foreach (var c in s) hash = hash * 31 + c;
                var r = (hash & 0xFF0000) >> 16;
                var g = (hash & 0x00FF00) >> 8;
                var b = (hash & 0x0000FF);
                // create pleasant variations
                r = (r + 80) % 256;
                g = (g + 40) % 256;
                b = (b + 120) % 256;
                return $"#{r:X2}{g:X2}{b:X2}";
            }
        }
    }
}


