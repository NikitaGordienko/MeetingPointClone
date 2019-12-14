using MeetingPoint.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image {get;set;}

        [ForeignKey("Id")]
        public List<UserRoom> UserRooms { get; set;}

        public string CalendarId { get; set; }
        public Calendar Calendar { get; set; }


        public string CreatorId { get; set; }
        public DateTime CreationDate { get; set; }


        private void Connect(User user)
        {
            // TODO: Добавить событие для календаря OnUserConnect() и подписаться на метод Connect()
        }

        private void Disconnect(User user)
        {
            if (user.Id == CreatorId)
            {
                // TODO: Уничтожить комнату, если автор вышел
            }

        }
    }
}
