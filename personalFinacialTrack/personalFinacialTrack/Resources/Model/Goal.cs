using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace personalFinacialTrack.Resources.Model
{
    namespace personalFinacialTrack.Resources.Model
    {
        public class Goal
        {
            public int GoalId { get; set; } // Primary key for database
            public string Name { get; set; }
            public decimal Amount { get; set; } // target amount
            public Currency Currency { get; set; }
            public DateTime? Deadline { get; set; }
            public string Note { get; set; }
            public string Color { get; set; } // store as hex string like "#FF0000"

            // 🔹 New fields
            public decimal CurrentAmount { get; set; } // how much saved so far

            // 🔹 Computed property for progress (0.0 → 1.0 for ProgressBar)
            public double Progress
            {
                get
                {
                    if (Amount <= 0) return 0;
                    return (double)(CurrentAmount / Amount);
                }
            }

            // 🔹 Remaining amount to reach the goal
            public decimal Remaining => Amount - CurrentAmount;
        }
    }

}
