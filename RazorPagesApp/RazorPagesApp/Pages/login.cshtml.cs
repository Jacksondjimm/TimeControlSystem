using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

using Microsoft.EntityFrameworkCore;
using RazorPagesApp.Models;

namespace RazorPagesApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class loginModel : PageModel
    {
        public string Message { get; private set; } = "Ќезарегистрированный пользователь";

        ApplicationContext context;
        public User Person { get; set; } = new();
        public List<User> Users { get; private set; } = new();
        public loginModel(ApplicationContext db)
        {
            context = db;
        }
        public User admin = new User    //¬ременный пользователь admin
        {
            Email = "admin",
            Password= "11111",
            Role= "adminRole",

        };
        

        public async Task OnGetAsync()
        {
            var user = HttpContext.User.Identity;
            if (user is not null && user.IsAuthenticated)
                Message = $"ѕользователь: {user.Name}";
        }

        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            Users = context.Users.AsNoTracking().ToList();
            // получаем из формы email и пароль
            var form = Request.Form;
            // если email и/или пароль не установлены, посылаем статусный код ошибки 400
            if (!form.ContainsKey("email") || !form.ContainsKey("password"))
                return Content("Email и/или пароль не установлены");

            //¬ременно добавл€ем пользовател€ admin:
            context.Users.Add(admin);
            await context.SaveChangesAsync();
            Users = context.Users.AsNoTracking().ToList();
            // находим пользовател€ 
            User? person = Users.FirstOrDefault(p => p.Email == email && p.Password == password);
            // если пользователь не найден, отправл€ем статусный код 401
            if (person is null) return Content("ѕользователь не установлен");

            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, person.Email),
                new Claim(ClaimTypes.Role, person.Role)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            //”дал€ем пользовател€ admin:
            context.Users.Remove(admin);
            await context.SaveChangesAsync();

            return Redirect("/");

        }



    }
}
