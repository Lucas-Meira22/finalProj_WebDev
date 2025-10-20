using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace personalFinacialTrack.Resources.Model
{
    public class DatabaseHelper
    {
        private readonly string connectionString =
            "Server=10.0.2.2;Port=3307;Database=personalFinancialTracker;Uid=root;Pwd=23422342;";

        // ----- CONNECTION TEST -----
        public async Task TestConnectionAsync()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                Debug.WriteLine("✅ Connection successful!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Connection failed: {ex.Message}");
            }
        }

        // ---------------- GOALS CRUD ----------------
        public async Task CreateGoalAsync(Goal goal)
        {
            string query = @"
                INSERT INTO goals (Name, Amount, CurrentAmount, Currency, Deadline, Note, Color)
                VALUES (@Name, @Amount, @CurrentAmount, @Currency, @Deadline, @Note, @Color);";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(goal.Name) ? "Untitled" : goal.Name);
            cmd.Parameters.AddWithValue("@Amount", goal.Amount);
            cmd.Parameters.AddWithValue("@CurrentAmount", goal.CurrentAmount);
            cmd.Parameters.AddWithValue("@Currency", goal.Currency.ToString());
            cmd.Parameters.AddWithValue("@Deadline", goal.Deadline.HasValue ? (object)goal.Deadline.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(goal.Note) ? DBNull.Value : (object)goal.Note);
            cmd.Parameters.AddWithValue("@Color", string.IsNullOrEmpty(goal.Color) ? "#FFFFFF" : (object)goal.Color);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Goal>> GetGoalsAsync()
        {
            var goals = new List<Goal>();
            string query = "SELECT GoalId, Name, Amount, CurrentAmount, Currency, Deadline, Note, Color FROM goals ORDER BY GoalId DESC;";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var g = new Goal
                {
                    GoalId = reader.GetInt32("GoalId"),
                    Name = reader["Name"] == DBNull.Value ? string.Empty : reader.GetString("Name"),
                    Amount = reader["Amount"] == DBNull.Value ? 0m : reader.GetDecimal("Amount"),
                    CurrentAmount = reader["CurrentAmount"] == DBNull.Value ? 0m : reader.GetDecimal("CurrentAmount"),
                    Currency = Enum.TryParse(reader["Currency"]?.ToString(), out Currency parsedCurrency) ? parsedCurrency : Currency.USD,
                    Deadline = reader["Deadline"] == DBNull.Value ? null : (DateTime?)reader.GetDateTime("Deadline"),
                    Note = reader["Note"] == DBNull.Value ? string.Empty : reader.GetString("Note"),
                    Color = reader["Color"] == DBNull.Value ? "#FFFFFF" : reader.GetString("Color")
                };
                goals.Add(g);
            }
            return goals;
        }

        public async Task UpdateGoalAsync(Goal goal)
        {
            string query = @"
                UPDATE goals
                SET Name=@Name, Amount=@Amount, CurrentAmount=@CurrentAmount, Currency=@Currency, Deadline=@Deadline, Note=@Note, Color=@Color
                WHERE GoalId=@GoalId;";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@GoalId", goal.GoalId);
            cmd.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(goal.Name) ? "Untitled" : goal.Name);
            cmd.Parameters.AddWithValue("@Amount", goal.Amount);
            cmd.Parameters.AddWithValue("@CurrentAmount", goal.CurrentAmount);
            cmd.Parameters.AddWithValue("@Currency", goal.Currency.ToString());
            cmd.Parameters.AddWithValue("@Deadline", goal.Deadline.HasValue ? (object)goal.Deadline.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(goal.Note) ? DBNull.Value : (object)goal.Note);
            cmd.Parameters.AddWithValue("@Color", string.IsNullOrEmpty(goal.Color) ? "#FFFFFF" : (object)goal.Color);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteGoalAsync(int id)
        {
            string query = "DELETE FROM goals WHERE GoalId = @Id;";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // ---------------- CATEGORIES CRUD ----------------
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var list = new List<Category>();
            string q = "SELECT CategoryId, Name, Type, CreatedAt FROM categories ORDER BY Name;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new Category
                {
                    CategoryId = r.GetInt32("CategoryId"),
                    Name = r["Name"]?.ToString() ?? string.Empty,
                    Type = Enum.TryParse(r["Type"]?.ToString(), out CategoryType ct) ? ct : CategoryType.Expense,
                });
            }
            return list;
        }

      
        public async Task<int> CreateCategoryAsync(Category cat)
        {
            string q = "INSERT INTO categories (Name, Type) VALUES (@Name, @Type); SELECT LAST_INSERT_ID();";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Name", cat.Name);
            cmd.Parameters.AddWithValue("@Type", cat.Type.ToString());
            var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return id;
        }

        public async Task<int> UpdateCategoryAsync(Category cat)
        {
            string q = "UPDATE categories SET Name=@Name, Type=@Type WHERE CategoryId=@Id;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Id", cat.CategoryId);
            cmd.Parameters.AddWithValue("@Name", cat.Name);
            cmd.Parameters.AddWithValue("@Type", cat.Type.ToString());
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            string q = "DELETE FROM categories WHERE CategoryId=@Id;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync();
        }

        // ---------------- TRANSACTIONS CRUD ----------------
        public async Task<List<Transaction>> GetTransactionsAsync(int limit = 100)
        {
            var list = new List<Transaction>();
            string q = $"SELECT TransactionId, Amount, Type, CategoryId, Note, Date, GoalId FROM transactions ORDER BY Date DESC LIMIT @Limit;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Limit", limit);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new Transaction
                {
                    TransactionId = r.GetInt32("TransactionId"),
                    Amount = r["Amount"] == DBNull.Value ? 0m : r.GetDecimal("Amount"),
                    Type = Enum.TryParse(r["Type"]?.ToString(), out TransactionType tt) ? tt : TransactionType.Expense,
                    CategoryId = r["CategoryId"] == DBNull.Value ? null : (int?)r.GetInt32("CategoryId"),
                    Note = r["Note"] == DBNull.Value ? string.Empty : r.GetString("Note"),
                    Date = r["Date"] == DBNull.Value ? DateTime.Now : r.GetDateTime("Date"),
                    GoalId = r["GoalId"] == DBNull.Value ? null : (int?)r.GetInt32("GoalId")
                });
            }
            return list;
        }

        public async Task<int> CreateTransactionAsync(Transaction tx)
        {
            string q = @"
                INSERT INTO transactions (Amount, Type, CategoryId, Note, Date, GoalId)
                VALUES (@Amount, @Type, @CategoryId, @Note, @Date, @GoalId);
                SELECT LAST_INSERT_ID();";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Amount", tx.Amount);
            cmd.Parameters.AddWithValue("@Type", tx.Type.ToString());
            cmd.Parameters.AddWithValue("@CategoryId", tx.CategoryId.HasValue ? (object)tx.CategoryId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(tx.Note) ? DBNull.Value : (object)tx.Note);
            cmd.Parameters.AddWithValue("@Date", tx.Date);
            cmd.Parameters.AddWithValue("@GoalId", tx.GoalId.HasValue ? (object)tx.GoalId.Value : DBNull.Value);
            var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            await UpdateBudgetAfterTransactionAsync(tx);

            // If tx is linked to a goal and it's Income, add to Goal.CurrentAmount; if Expense and linked, subtract
            if (tx.GoalId.HasValue)
            {
                var gList = await GetGoalsAsync();
                var g = gList.Find(x => x.GoalId == tx.GoalId.Value);
                if (g != null)
                {
                    string recordType;
                    if (tx.Type == TransactionType.Income)
                    {
                        g.CurrentAmount += tx.Amount;
                        recordType = "Add";
                    }
                    else
                    {
                        g.CurrentAmount -= tx.Amount;
                        recordType = "Withdraw";
                    }

                    if (g.CurrentAmount < 0) g.CurrentAmount = 0;
                    await UpdateGoalAsync(g);

                    // Register record
                    await AddGoalRecordAsync(g.GoalId, tx.Amount, recordType, tx.Note);
                }
            }

            return id;
        }

        // ---------------- BUDGETS CRUD ----------------
        public async Task<List<Budget>> GetBudgetsAsync(int year, int month)
        {
            var list = new List<Budget>();
            string q = "SELECT BudgetId, CategoryId, Year, Month, LimitAmount FROM budgets WHERE Year=@Year AND Month=@Month;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new Budget
                {
                    BudgetId = r.GetInt32("BudgetId"),
                    CategoryId = r.GetInt32("CategoryId"),
                    Year = r.GetInt32("Year"),
                    Month = r.GetInt32("Month"),
                    LimitAmount = r.GetDecimal("LimitAmount")
                });
            }
            return list;
        }

        public async Task<int> CreateOrUpdateBudgetAsync(Budget b)
        {
            // If a budget exists for same category/year/month, update; otherwise insert
            string check = "SELECT BudgetId FROM budgets WHERE CategoryId=@CategoryId AND Year=@Year AND Month=@Month LIMIT 1;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using (var cmd = new MySqlCommand(check, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryId", b.CategoryId);
                cmd.Parameters.AddWithValue("@Year", b.Year);
                cmd.Parameters.AddWithValue("@Month", b.Month);
                var res = await cmd.ExecuteScalarAsync();
                if (res != null && res != DBNull.Value)
                {
                    int id = Convert.ToInt32(res);
                    string upd = "UPDATE budgets SET LimitAmount=@LimitAmount WHERE BudgetId=@BudgetId;";
                    using var cmdUpd = new MySqlCommand(upd, conn);
                    cmdUpd.Parameters.AddWithValue("@LimitAmount", b.LimitAmount);
                    cmdUpd.Parameters.AddWithValue("@BudgetId", id);
                    await cmdUpd.ExecuteNonQueryAsync();
                    return id;
                }
                else
                {
                    string ins = "INSERT INTO budgets (CategoryId, Year, Month, LimitAmount) VALUES (@CategoryId, @Year, @Month, @LimitAmount); SELECT LAST_INSERT_ID();";
                    using var cmdIns = new MySqlCommand(ins, conn);
                    cmdIns.Parameters.AddWithValue("@CategoryId", b.CategoryId);
                    cmdIns.Parameters.AddWithValue("@Year", b.Year);
                    cmdIns.Parameters.AddWithValue("@Month", b.Month);
                    cmdIns.Parameters.AddWithValue("@LimitAmount", b.LimitAmount);
                    return Convert.ToInt32(await cmdIns.ExecuteScalarAsync());
                }
            }
        }

        public async Task DeleteBudgetAsync(int id)
        {
            string q = "DELETE FROM budgets WHERE BudgetId=@Id;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // ---------------- Helper: get total spent for a category in a month (used by budgets/report) ----------------
        public async Task<decimal> GetTotalForCategoryInMonthAsync(int categoryId, int year, int month)
        {
            string q = @"
                SELECT IFNULL(SUM(CASE WHEN Type='Expense' THEN Amount WHEN Type='Income' THEN -Amount ELSE 0 END),0) AS Total
                FROM transactions
                WHERE CategoryId=@CategoryId
                  AND YEAR(Date)=@Year
                  AND MONTH(Date)=@Month;";
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Parameters.AddWithValue("@Month", month);
            var val = await cmd.ExecuteScalarAsync();
            return val == DBNull.Value ? 0m : Convert.ToDecimal(val);
        }

        // ---------------- AUTOMATIC BUDGET UPDATE WHEN NEW TRANSACTION ----------------
        public async Task UpdateBudgetAfterTransactionAsync(Transaction tx)
        {
            if (tx.CategoryId == null)
                return; // Only apply to transactions linked to a category

            int year = tx.Date.Year;
            int month = tx.Date.Month;
            int categoryId = tx.CategoryId.Value;

            // Check if a budget exists for this category/month
            var budgets = await GetBudgetsAsync(year, month);
            var budget = budgets.FirstOrDefault(b => b.CategoryId == categoryId);

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            if (budget == null)
            {
                // No budget yet → create one with limit 0
                string ins = "INSERT INTO budgets (CategoryId, Year, Month, LimitAmount) VALUES (@Cat, @Year, @Month, @Limit);";
                using var cmd = new MySqlCommand(ins, conn);
                cmd.Parameters.AddWithValue("@Cat", categoryId);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Limit", 0m);
                await cmd.ExecuteNonQueryAsync();

                // Reload to get the new budget reference
                budgets = await GetBudgetsAsync(year, month);
                budget = budgets.FirstOrDefault(b => b.CategoryId == categoryId);
            }

            // Adjust the limit (positive for Income, negative for Expense)
            decimal adjustment = tx.Type == TransactionType.Income ? tx.Amount : -tx.Amount;
            budget.LimitAmount += adjustment;

            // Save the updated budget
            string upd = "UPDATE budgets SET LimitAmount=@LimitAmount WHERE BudgetId=@Id;";
            using (var cmd = new MySqlCommand(upd, conn))
            {
                cmd.Parameters.AddWithValue("@LimitAmount", budget.LimitAmount);
                cmd.Parameters.AddWithValue("@Id", budget.BudgetId);
                await cmd.ExecuteNonQueryAsync();
            }

            Debug.WriteLine($"📊 Budget updated for Category={categoryId}, {month}/{year}: new total = {budget.LimitAmount}");
        }

        // ---------------- GOAL RECORDS ----------------
        public async Task AddGoalRecordAsync(int goalId, decimal amount, string type, string note = "")
        {
            string q = @"INSERT INTO goal_records (GoalId, Amount, Type, Note, Date)
                 VALUES (@GoalId, @Amount, @Type, @Note, @Date);";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@GoalId", goalId);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@Type", type);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? DBNull.Value : note);
            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<GoalRecord>> GetGoalRecordsAsync(int goalId)
        {
            var list = new List<GoalRecord>();
            string q = "SELECT RecordId, GoalId, Amount, Type, Note, Date FROM goal_records WHERE GoalId=@GoalId ORDER BY Date DESC;";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@GoalId", goalId);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new GoalRecord
                {
                    RecordId = r.GetInt32("RecordId"),
                    GoalId = r.GetInt32("GoalId"),
                    Amount = r.GetDecimal("Amount"),
                    Type = r.GetString("Type"),
                    Note = r["Note"] == DBNull.Value ? string.Empty : r.GetString("Note"),
                    Date = r.GetDateTime("Date")
                });
            }
            return list;
        }

    }
}