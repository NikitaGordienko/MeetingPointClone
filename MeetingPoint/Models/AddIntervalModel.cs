using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class AddIntervalModel
    {
        public string CalendarId { get; set; }

        // Дата начала интервала
        public DateTime DateBegin { get; set; }

        // Дата окончания интервала
        public DateTime DateEnd { get; set; }

        public string Status { get; set; }
    }
}
