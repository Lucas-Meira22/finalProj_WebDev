using System;

namespace personalFinacialTrack.Resources.Model
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; } = TransactionType.Expense;
        public int? CategoryId { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public int? GoalId { get; set; } // optional link to a goal
    }

    public enum TransactionType
    {
        Expense,
        Income
    }
}