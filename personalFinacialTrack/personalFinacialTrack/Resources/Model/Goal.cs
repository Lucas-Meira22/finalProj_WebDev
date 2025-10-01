using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace personalFinacialTrack.Resources.Model
{
    public class Goal
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? Deadline { get; set; }
        public string Note { get; set; }
        public string Color { get; set; } // store as hex string like "#FF0000"
    }
}
