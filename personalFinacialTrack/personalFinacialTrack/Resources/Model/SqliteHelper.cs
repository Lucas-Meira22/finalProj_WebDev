using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace personalFinacialTrack.Resources.Model
{
    public class SqliteHelper
    {
        private readonly SQLiteAsyncConnection _db;

        public SqliteHelper(string dbPath = null)
        {
            var path = dbPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "personal_finance.db3");
            _db = new SQLiteAsyncConnection(path);
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            await _db.CreateTableAsync<Goal>();
            await _db.CreateTableAsync<Category>();
            await _db.CreateTableAsync<Transaction>();
            await _db.CreateTableAsync<Budget>();
        }

        // Simple wrappers (examples)
        public Task<List<Goal>> GetGoalsAsync() => _db.Table<Goal>().OrderByDescending(g => g.GoalId).ToListAsync();
        public Task<int> SaveGoalAsync(Goal g) => _db.InsertOrReplaceAsync(g);
        public Task<int> DeleteGoalAsync(Goal g) => _db.DeleteAsync(g);

        public Task<List<Category>> GetCategoriesAsync() => _db.Table<Category>().OrderBy(c => c.Name).ToListAsync();
        public Task<int> SaveCategoryAsync(Category c) => _db.InsertOrReplaceAsync(c);

        public Task<List<Transaction>> GetTransactionsAsync() => _db.Table<Transaction>().OrderByDescending(t => t.Date).ToListAsync();
        public Task<int> SaveTransactionAsync(Transaction t) => _db.InsertOrReplaceAsync(t);

        public Task<List<Budget>> GetBudgetsAsync() => _db.Table<Budget>().ToListAsync();
        public Task<int> SaveBudgetAsync(Budget b) => _db.InsertOrReplaceAsync(b);
    }
}
