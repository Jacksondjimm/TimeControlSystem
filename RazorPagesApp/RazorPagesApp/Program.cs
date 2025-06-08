using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RazorPagesApp.Models;  // пространство имен класса ApplicationContext
using Microsoft.Extensions.Options;
using RazorPagesApp.Data;

var builder = WebApplication.CreateBuilder(args);

// получаем строку подключения из файла конфигурации
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options =>options.UseNpgsql(connection));


// добавляем в приложение сервисы Razor Pages:
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/manage" , "OnlyForAdmin");
});

// аутентификация с помощью куки begin
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => options.LoginPath = "/login");

builder.Services.AddAuthorization(
        options =>
        {
            options.AddPolicy("OnlyForAdmin", policy =>
            {
                policy.RequireRole("adminRole");
            });
        });


// аутентификация с помощью куки end

var app = builder.Build();

//app.UseAuthentication();   // добавление middleware аутентификации 
//app.UseAuthorization();   // добавление middleware авторизации 

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationContext>();
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // строчка для устранения исключения по timestamp при переходе на postgree
    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true); // строчка для устранения исключения по timestamp при переходе на postgree
    //context.Database.EnsureDeleted();
    //context.Database.EnsureCreated();
    DbInitializer.Initialize(context);

}

app.MapRazorPages();
await app.RunAsync();


