using MeetingPoint.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MeetingPoint.Models
{
    public class User : IdentityUser
    {
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public DateTime RegistrationDate { get; set; }

        [ForeignKey("Id")]
        public List<UserRoom> UserRooms { get; set; }

        [ForeignKey("Id")]
        public List<Interval> Intervals { get; set; }

        [ForeignKey("Id")]
        public List<CalculatedIntervalUser> CalculatedIntervalUsers { get; set; }

        // TODO: Создать класс RoomList (Возможно нужно использовать Entity Framework Core)
        // TODO: Создать событие OnRoomCreate и добавлять в список новую команту (Или передать это DbContext (см. Entity Framework Core))
        public void CreateRoom(DateTime dateBegin, DateTime dateEnd)
        {

        }

        public void ConnectToRoom()
        {
            // TODO: Как подключаться к комнате, которую не знаешь? Нужен список всех комнат активных?
        }

        public void CreateInterval(Room room, DateTime dateBegin, DateTime dateEnd)
        {

        }

    }
}
