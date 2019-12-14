using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class Interval
    {

        public string Id {get;set;}

        public string CalendarId {get;set;}

        public Calendar Calendar {get;set;}

        public string UserId {get;set;}

        public User User {get;set;}

        // Дата начала интервала
        public DateTime DateBegin { get; set; }

        // Дата окончания интервала
        public DateTime DateEnd { get; set; }
    }
}
