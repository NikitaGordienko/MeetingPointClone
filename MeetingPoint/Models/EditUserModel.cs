using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    [NotMapped]
    public class EditUserModel
    {
        public string Id { get; set; }

        public string Nickname { get; set; }
    }
}
