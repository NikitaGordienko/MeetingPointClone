using MeetingPoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MeetingPoint.ViewModels
{
    public class LobbyRoomsModel
    {
        public IEnumerable<UserRoom> UserRooms { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
        public User CurrentUser { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
