using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class UserRoom
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string RoomId { get; set; }
        public Room Room { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public DateTime ConnectionDate { get; set; }
        public DateTime ExitDate { get; set; }
    }
}
