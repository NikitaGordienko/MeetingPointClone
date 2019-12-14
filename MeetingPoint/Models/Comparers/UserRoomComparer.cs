using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models.Comparers
{
    public class UserRoomComparer : IEqualityComparer<UserRoom>
    {
        public bool Equals(UserRoom x, UserRoom y)
        {
            if ((x.RoomId == y.RoomId) && (x.UserId == y.UserId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(UserRoom obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}
