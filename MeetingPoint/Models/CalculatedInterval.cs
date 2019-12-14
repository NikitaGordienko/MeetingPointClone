using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class CalculatedInterval
    {
        public string Id { get; set; }

        [ForeignKey("Id")]
        public List<CalculatedIntervalUser> CalculatedIntervalUsers { get; set; }

        public string CalendarId { get; set; }

        [ForeignKey("CalendarId")]
        public Calendar Calendar { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
        
    }
}
