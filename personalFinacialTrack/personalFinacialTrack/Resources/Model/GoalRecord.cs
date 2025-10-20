using System;

namespace personalFinacialTrack.Resources.Model
{
    public class GoalRecord
    {
        public int RecordId { get; set; }
        public int GoalId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Add" or "Withdraw"
        public string Note { get; set; }
        public DateTime Date { get; set; }
    }
}
