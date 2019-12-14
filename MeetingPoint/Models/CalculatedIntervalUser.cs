using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class CalculatedIntervalUser
    {
        public string CalculatedIntervalId { get; set; }

        public CalculatedInterval CalculatedInterval { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}
