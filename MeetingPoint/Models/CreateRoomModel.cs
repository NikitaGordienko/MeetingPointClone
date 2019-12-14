using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    [NotMapped]
    public class CreateRoomModel
    {
        [Required(ErrorMessage = "Укажите название")]
        [Display(Name = "Название комнаты")]
        public string Name { get; set; }

        //[Display(Name = "Аватар комнаты")]
        public string Image { get; set; }

        [Display(Name = "Цвет")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Не указана дата начала")]
        [Display(Name = "Дата начала")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "Не указана дата окончания")]
        //[DateGreaterThan("StartDate")] написать метод сравнения
        [Display(Name = "Дата окончания")]
        public DateTime DateTo { get; set; }
    }
}
