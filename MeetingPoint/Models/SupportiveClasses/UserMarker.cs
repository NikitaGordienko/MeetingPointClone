using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models.SupportiveClasses
{
    public class UserMarker
    {
        public string UserName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public UserMarker(string id, string latitude, string longitude)
        {
            UserName = id ?? throw new ArgumentNullException(nameof(id));
            Latitude = latitude ?? throw new ArgumentNullException(nameof(latitude));
            Longitude = longitude ?? throw new ArgumentNullException(nameof(longitude));
        }
    }
}
