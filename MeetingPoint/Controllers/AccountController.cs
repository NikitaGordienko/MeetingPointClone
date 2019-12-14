using MeetingPoint.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace MeetingPoint.Controllers
{
    public class AccountController : Controller
    {
        // Сервис управления пользователями
        private readonly UserManager<User> userManager;

        // Сервис аутентификации пользователя
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        #region Registration
        // Возвращает [Views/Account/Registration]
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        // Регистрирует пользователя [Views/Account/Registration]
        [HttpPost]
        public async Task<ActionResult> Registration(RegistrationModel model)
        {
            // Проверка корректности полей модели
            if (ModelState.IsValid)
            {
                string protocol = Url.ActionContext.HttpContext.Request.Scheme.ToString();
                string host = Url.ActionContext.HttpContext.Request.Host.ToString();

                model.Avatar = model.Avatar.Replace(protocol + "://", string.Empty);
                model.Avatar = model.Avatar.Replace(host, string.Empty);
                // TODO: Придумать другую проверку
                model.Avatar = Url.IsLocalUrl(model.Avatar) ? model.Avatar : null;

                // Создание пользователя
                User user = new User
                {
                    UserName = model.Login,
                    Nickname = model.Nickname,
                    Avatar = model.Avatar ?? "/images/user_avatar/default.png"
                };

                // Добавление пользователя в базу данных
                var identityResult = await userManager.CreateAsync(user, model.Password);

                if (identityResult.Succeeded)
                {
                    // Параметр isPersistent - отражает, должны ли сохранится cookie после закрытия браузера
                    // Авторизация после регистрации
                    // await signInManager.SignInAsync(user, false);

                    // Перенаправления пользователя после успешной авторизации
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(String.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region Authorization
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Аутентификация пользователя
                var identityResult = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);

                if (identityResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        // Возврат пользователя на предыдущую страницу веб-приложения
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Lobby");
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Неправильный логин и/или пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
