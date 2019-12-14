using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MeetingPoint.Models;
using MeetingPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingPoint.Controllers
{
    [Authorize]
    public class LobbyController : Controller
    {
        UserManager<User> _userManager;
        private ApplicationDbContext _context;

        public LobbyController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context;
        }

        // получить объект текущего пользователя
        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        public IActionResult Index()
        {
            /*
            var objUser = await GetCurrentUserAsync();
            //var _UserRooms = _context.UserRoom.Where(ur => ur.UserId == objUser.Id);

            LobbyRoomsModel lrm = new LobbyRoomsModel { UserRooms = _context.UserRoom.ToList(), Rooms = _context.Room.ToList(), currentUser = objUser, Users = _context.Users.ToList() }; 

            return View(lrm);
            */

            var objUser = GetCurrentUserAsync().Result; // без .Result не работает

            // Получаем все UserRooms данного пользователя
            var currentUserRoomsNoOrder = from ur in _context.UserRoom.ToList() where ur.UserId == objUser.Id select ur;
            // сортировка по дате присоединения / создания
            var currentUserRooms = currentUserRoomsNoOrder.OrderBy(cur => cur.ConnectionDate);
            objUser.UserRooms = currentUserRooms.ToList();

            // привязываем к каждой UserRoom своего User и свою Room
            foreach (var oneUserRoom in objUser.UserRooms)
            {
                oneUserRoom.Room = RoomController.GetFilledRoom(oneUserRoom.RoomId, _context);
                oneUserRoom.User = objUser; // необязательно
            }

            string[] months = { "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" };
            ViewData["months"] = months;

            // Передаем модель User
            return View(objUser);
        }

        [HttpPost]
        public IActionResult CreateRoom(CreateRoomModel model)
        {
            if (ModelState.IsValid)
            {
                string protocol = Url.ActionContext.HttpContext.Request.Scheme.ToString();
                string host = Url.ActionContext.HttpContext.Request.Host.ToString();

                model.Image = model.Image.Replace(protocol + "://", string.Empty);
                model.Image = model.Image.Replace(host, string.Empty);
                // TODO: Придумать другую проверку
                model.Image = Url.IsLocalUrl(model.Image) ? model.Image : null;

                User user = GetCurrentUserAsync().Result;

                Calendar calendar = new Calendar
                {
                    Id = Guid.NewGuid().ToString(),
                    DateFrom = model.DateFrom,
                    DateTo = model.DateTo
                };

                _context.Calendar.Add(calendar);

                Room room = new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Calendar = calendar,
                    CalendarId = calendar.Id,
                    CreationDate = DateTime.Now,
                    CreatorId = user.Id,
                    Image = model.Image ?? "/images/room_avatar/default.png" // TODO: Работать с файловой системой сервера
                };

                _context.Room.Add(room);

                UserRoom userRoom = new UserRoom
                {
                    UserId = user.Id,
                    User = user,
                    Room = room,
                    RoomId = room.Id,
                    ConnectionDate = DateTime.Now,
                    ExitDate = DateTime.Now, // TODO: Исправить ошибку с недопустимым значением NULL
                };

                _context.UserRoom.Add(userRoom);
                _context.SaveChanges();
                // Возврат пользователя на предыдущую страницу веб-приложения
                //UserRoom filledUserRoom = RoomController.GetFilledUserRoom(userRoom.User, userRoom.RoomId, _context);

                return RedirectToAction("Index", "Room",new { id = userRoom.RoomId });
                // При редиректе не успевает пройти анимация
                //return Content(userRoom.RoomId);
                //return Json(new { status = "success", id = room.Id });
                //return RedirectToAction("Index");

            }
            else
            {
                //ModelState.AddModelError(String.Empty, "Ошибка при создании комнаты");
                return BadRequest(ModelState);
            }


        }
    }
}