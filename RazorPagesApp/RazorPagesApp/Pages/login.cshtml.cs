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
        public string Message { get; private set; } = "�������������������� ������������";

        ApplicationContext context;
        public User Person { get; set; } = new();
        public List<User> Users { get; private set; } = new();
        public loginModel(ApplicationContext db)
        {
            context = db;
        }
        public User admin = new User    //��������� ������������ admin
        {
            Email = "admin",
            Password= "11111",
            Role= "adminRole",

        };
        

        public async Task OnGetAsync()
        {
            var user = HttpContext.User.Identity;
            if (user is not null && user.IsAuthenticated)
                Message = $"������������: {user.Name}";
        }

        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            Users = context.Users.AsNoTracking().ToList();
            // �������� �� ����� email � ������
            var form = Request.Form;
            // ���� email �/��� ������ �� �����������, �������� ��������� ��� ������ 400
            if (!form.ContainsKey("email") || !form.ContainsKey("password"))
                return Content("Email �/��� ������ �� �����������");

            //�������� ��������� ������������ admin:
            context.Users.Add(admin);
            await context.SaveChangesAsync();
            Users = context.Users.AsNoTracking().ToList();
            // ������� ������������ 
            User? person = Users.FirstOrDefault(p => p.Email == email && p.Password == password);
            // ���� ������������ �� ������, ���������� ��������� ��� 401
            if (person is null) return Content("������������ �� ����������");

            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, person.Email),
                new Claim(ClaimTypes.Role, person.Role)
            };
            // ������� ������ ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // ��������� ������������������ ����
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            //������� ������������ admin:
            context.Users.Remove(admin);
            await context.SaveChangesAsync();

            return Redirect("/");

        }



    }
}
