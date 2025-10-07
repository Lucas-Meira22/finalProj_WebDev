using MySqlConnector;
using personalFinacialTrack.Resources.Model.personalFinacialTrack.Resources.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace personalFinacialTrack.Resources.Model
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=10.0.2.2;Port=3306;Database=personalFinancialTracker;Uid=root;Pwd=1234;";

        // Test connection
        public async Task TestConnectionAsync()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                Debug.WriteLine("Connection successful!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        // CREATE
        public async Task CreateGoalAsync(Goal goal)
        {
            string query = @"
            INSERT INTO Goal
            (Name, Amount, CurrentAmount, Currency, Deadline, Note, Color)
            VALUES (@Name, @Amount, @CurrentAmount, @Currency, @Deadline, @Note, @Color);";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(query,conn);
            cmd.Parameters.AddWithValue("@Name", goal.Name);
            cmd.Parameters.AddWithValue("@Amount", goal.Amount);
            cmd.Parameters.AddWithValue("@CurrentAmount", goal.CurrentAmount);
            cmd.Parameters.AddWithValue("@Currency", goal.Currency);
            cmd.Parameters.AddWithValue("@Deadline", goal.Deadline.HasValue ? (object)goal.Deadline.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Note", goal.Note);
            cmd.Parameters.AddWithValue("@Color", goal.Color);

            int rows = await cmd.ExecuteNonQueryAsync();
            Debug.WriteLine($"INSERT rows: {rows} | Name={goal.Name}, Amount={goal.Amount}, Currency={goal.Currency}, Deadline={goal.Deadline}, Note={goal.Note}, Color={goal.Color}");

        }

        // SELECT
        public async Task<List<Goal>> GetGoalsAsync()
        {
            var goals = new List<Goal>();
            string query = "SELECT * FROM Goal;";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                goals.Add(new Goal
                {
                    GoalId = reader.GetInt32("GoalID"), // 👈 match your DB column name
                    Name = reader.GetString("Name"),
                    Amount = reader.GetDecimal("Amount"),
                    CurrentAmount = reader.GetDecimal("CurrentAmount"),
                    Currency = Enum.TryParse<Currency>(reader.GetString("Currency"), out var currency) ? currency : Currency.USD,
                    Deadline = reader["Deadline"] == DBNull.Value ? null : (DateTime?)reader.GetDateTime("Deadline"),
                    Note = reader["Note"] == DBNull.Value ? "" : reader.GetString("Note"),
                    Color = reader["Color"] == DBNull.Value ? "" : reader.GetString("Color")
                });

            }

            return goals;
        }


        // DELETE by Name or you can add Id
        public async Task DeleteGoalAsync(int id)
        {
            string query = "DELETE FROM Goal WHERE GoalId = @Id;";

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

    }
}
