using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models.SupportiveClasses
{
    public class UnitedIntervals
    {
        public string Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string UserId { get; set; }
        public string CalendarId { get; set; }

        public UnitedIntervals(string id, DateTime dateFrom, DateTime dateTo, string userId, string calendarId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(Id));
            DateFrom = dateFrom;
            DateTo = dateTo;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            CalendarId = calendarId ?? throw new ArgumentNullException(nameof(calendarId));
        }
    }
}
