using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetingPoint.Models;
using MeetingPoint.Models.Comparers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MeetingPoint.Models.SupportiveClasses;
using Newtonsoft.Json;

namespace MeetingPoint.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        UserManager<User> userManager;
        private ApplicationDbContext _context;

        public RoomController(UserManager<User> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        [HttpGet]
        public IActionResult Index()
        {
            User currentUser = GetUser().Result;

            var roomId = RouteData.Values["id"].ToString();

            return View(GetFilledUserRoom(currentUser, roomId, _context));
        }

        [HttpGet]
        public IActionResult Invite(string roomId)
        {
            //var roomId = RouteData.Values["roomId"].ToString();
            // Получение пользователь комнаты по roomId
            var roomUsers = from UserRoom in _context.UserRoom.ToList() where UserRoom.RoomId == roomId select UserRoom;

            // Получение текущего пользователя перешедшего по ссылке
            User currentUser = GetUser().Result;
            UserRoom userRoom = new UserRoom
            {
                UserId = currentUser.Id,
                RoomId = roomId,
                ConnectionDate = DateTime.Today,
                ExitDate = DateTime.Today,
                Room = _context.Room.Find(roomId),
                User = currentUser
            };

            if (!roomUsers.Contains(userRoom, new UserRoomComparer()))
            {
                _context.UserRoom.Add(userRoom);
                _context.SaveChanges();
            }

            userRoom = GetFilledUserRoom(currentUser, roomId, _context);

            return View("Index", userRoom);

        }

        public async Task<User> GetUser()
        {
            return await userManager.GetUserAsync(User);
        }

        public static UserRoom GetFilledUserRoom(User user, string roomId, ApplicationDbContext _context)
        {

            UserRoom userRoom = _context.UserRoom.Find(user.Id, roomId);

            userRoom.Room = GetFilledRoom(roomId, _context);

            //Получаем все UserRooms пользователя, что бы вывести все его комнаты
            var selectedUserRooms2 = from ur in _context.UserRoom.ToList() where ur.UserId == user.Id select ur;
            var _selectedUserRooms2 = selectedUserRooms2.OrderBy(sur => sur.ConnectionDate);
            user.UserRooms = _selectedUserRooms2.ToList();

            foreach (var oneUserRoom in user.UserRooms)
            {
                oneUserRoom.Room = GetFilledRoom(oneUserRoom.RoomId, _context);
                oneUserRoom.User = user;
            }

            userRoom.User = user;

            return userRoom;
        }

        public static Room GetFilledRoom(string roomId, ApplicationDbContext _context)
        {
            Room room = _context.Room.Find(roomId);

            //Получаем все UserRoom этой комнаты, что бы вывести всех участников комнаты
            var selectedUserRooms1 = from ur in _context.UserRoom.ToList() where ur.RoomId == roomId select ur;
            room.UserRooms = selectedUserRooms1.ToList();

            //Заполняем объектами User
            foreach (var user in room.UserRooms)
            {
                User guest = _context.User.Find(user.UserId);
                user.User = guest;
            }

            Calendar calendar = _context.Calendar.Find(room.CalendarId);

            //Получаем все Intervals этого календаря
            var selectedIntervals = from i in _context.Interval.ToList() where i.CalendarId == calendar.Id select i;
            calendar.Intervals = selectedIntervals.ToList();

            room.Calendar = calendar; //Выбранный Room заполнен

            return room;
        }

        [HttpPost]
        public IActionResult Disconnect(String userId, String roomId)
        {
            var userRoom = _context.UserRoom.Find(userId, roomId);

            userRoom.Room = _context.Room.Find(roomId);

            userRoom.Room.UserRooms = (from ur in _context.UserRoom.ToList() where ur.RoomId == roomId select ur).ToList();

            if (userId == userRoom.Room.CreatorId)
            {
                foreach (var user in userRoom.Room.UserRooms)
                {
                    _context.UserRoom.Remove(user);
                }

                _context.Room.Remove(userRoom.Room);
            }
            else
            {
                _context.UserRoom.Remove(userRoom);
            }

            _context.SaveChanges();

            return RedirectToRoute(new { controller = "Lobby", action = "Index" });//RedirectToAction("Lobby");
        }

        [HttpPost]
        public IActionResult AddNewInterval([FromBody] AddIntervalModel[] model)
        {
            // Получение списка интервалов для удаления
            var intervalsToRemove =
                (from intr in _context.Interval.ToList()
                 where intr.UserId == GetUser().Result.Id
                 && intr.CalendarId == model[0].CalendarId
                 select intr);

            // Удаление интервалов
            _context.Interval.RemoveRange(intervalsToRemove);

            List<Interval> intervals = new List<Interval>();
            foreach (AddIntervalModel addInterval in model)
            {
                Interval interval = new Interval
                {
                    Id = Guid.NewGuid().ToString(),
                    CalendarId = addInterval.CalendarId,
                    UserId = GetUser().Result.Id,
                    User = GetUser().Result,
                    Calendar = _context.Calendar.Find(addInterval.CalendarId),
                    DateBegin = addInterval.DateBegin,
                    DateEnd = addInterval.DateEnd
                };
                intervals.Add(interval);
            }
            if (!(model[0].Status == "none-selected"))
            {
                foreach (Interval interv in intervals)
                {
                    _context.Interval.Add(interv);
                }
            }

            _context.SaveChanges();

            // Получаем все интервалы данного календаря и передаем в метод для расчета совпадений
            var currentCalendar = _context.Calendar.Find(model[0].CalendarId);
            List<Interval> currentCalendarIntervals = _context.Interval.Where(interv => interv.CalendarId == model[0].CalendarId).ToList();

            List<UnitedIntervals> unitedIntervals = CalculateUnitedIntervals.CalculateIntervals(currentCalendarIntervals, currentCalendar);
            
            // Формируем кортеж и разделяем результаты для последующей записи в БД
            (List<CalculatedInterval>, List<CalculatedIntervalUser>) tuple = CalculateUnitedIntervals.DivideUnitedIntervalsInTables(unitedIntervals);

            // Составляем списки для удаления
            List<CalculatedIntervalUser> calculatedIntervalUserToRemove = new List<CalculatedIntervalUser>();
            List<CalculatedInterval> calculatedIntervalsToRemove = _context.CalculatedInterval.Where(cir => cir.CalendarId == currentCalendar.Id).ToList();
            foreach (var item in calculatedIntervalsToRemove)
            {
                calculatedIntervalUserToRemove.AddRange(_context.CalculatedIntervalUser.Where(ciur => ciur.CalculatedIntervalId == item.Id));
            }

            // Удаляем записи по полученным спискам
            if (calculatedIntervalUserToRemove != null && calculatedIntervalsToRemove != null)
            {
                _context.RemoveRange(calculatedIntervalUserToRemove);
                _context.RemoveRange(calculatedIntervalsToRemove);
            }

            // Записываем полученные в кортеже записи
            _context.CalculatedInterval.AddRange(tuple.Item1);
            _context.CalculatedIntervalUser.AddRange(tuple.Item2);

            _context.SaveChanges();

            return Json(new
            {
                status = "Изменения сохранены",
            });
        }

        [HttpPost]
        public IActionResult AddMapMarker([FromBody] AddMapMarkerModel model)
        {
            var userRoom = _context.UserRoom.Find(GetUser().Result.Id, model.RoomId);

            userRoom.Latitude = model.Latitude;
            userRoom.Longitude = model.Longitude;

            _context.SaveChanges();

            // TODO: Заменить на JSON
            return Json(new
            {
                status = "Координаты маркера добавлены в базу",
            });
        }

        [HttpPost]
        public IActionResult GetMapMarker([FromBody] GetMapMarkerModel model)
        {

            var userRoomMarker = _context.UserRoom.ToList().FindAll(ur => ur.RoomId == model.RoomId);

            List<UserMarker> userMarkers = new List<UserMarker>();

            foreach (var item in userRoomMarker)
            {
                if (item.Latitude != null && item.Longitude != null)
                {
                    UserMarker userMarker = new UserMarker(userManager.FindByIdAsync(item.UserId).Result.UserName, item.Latitude, item.Longitude);
                    userMarkers.Add(userMarker);
                }

            }

            if (userMarkers.Count == 0)
            {
                return null;
            }
            else
            {
                return Json(userMarkers);
            }

        }
    }
}