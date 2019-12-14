using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MeetingPoint.Models
{
    [NotMapped]
    public class ChangeRoleModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<IdentityRole> Roles { get;set; }
        public IList<string> UserRoles { get; set; }
    }
}
