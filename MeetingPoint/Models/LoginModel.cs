using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models
{
    [NotMapped]
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }

        // Сохраняет Url страницы, к которой необходимо будет вернуться после авторизации
        public string ReturnUrl { get; set; }
    }
}
