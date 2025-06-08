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
    public class manageModel : PageModel
    {
        public string Message = "Незарегистрированный пользователь";
        public string Message2 = "Добавление нового пользователя";
        ApplicationContext context;
        [BindProperty]
        public User Person { get; set; } = new();
        public List<User> Users { get; private set; } = new();
        public List<CardBuffer> CardBuffers { get; private set; } = new();
        public manageModel(ApplicationContext db)
        {
            context = db;
        }

        public async Task OnGetAsync()
        {
            var user = HttpContext.User.Identity;
            if (user is not null && user.IsAuthenticated)
                Message = $"Пользователь: {user.Name}";
            Users = context.Users.AsNoTracking().ToList();
            CardBuffers = context.CardBuffers.AsNoTracking().ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = HttpContext.User.Identity;
            if (user is not null && user.IsAuthenticated)
                Message = $"Пользователь: {user.Name}";

            if (context.Users.FirstOrDefault(u => u.Email == Person.Email) != null)
            {
                Message2 = $"Такой почтовый адрес уже занят: {Person.Email}";
                return Page();
            }


            context.Users.Add(Person);
            await context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
            return RedirectToPage();
        }


    }
}
