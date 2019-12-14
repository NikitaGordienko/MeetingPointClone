using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class Calendar
    {
        public string Id { get; set; }

        [ForeignKey("Id")]
        public List<Interval> Intervals { get; set; }

        [ForeignKey("Id")]
        public List<CalculatedInterval> CalculatedInterval { get; set; }

        public DateTime DateFrom {get; set;}
        public DateTime DateTo {get;set;}

    }
}
