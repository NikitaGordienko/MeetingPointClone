using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    [NotMapped]
    public class ChangePasswordModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NewPassword { get; set; }
    }
}
