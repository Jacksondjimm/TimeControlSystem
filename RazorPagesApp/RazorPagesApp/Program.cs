using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RazorPagesApp.Models;  // ������������ ���� ������ ApplicationContext
using Microsoft.Extensions.Options;
using RazorPagesApp.Data;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ ����������� �� ����� ������������
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ApplicationContext>(options =>options.UseNpgsql(connection));


// ��������� � ���������� ������� Razor Pages:
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/manage" , "OnlyForAdmin");
});

// �������������� � ������� ���� begin
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


// �������������� � ������� ���� end

var app = builder.Build();

//app.UseAuthentication();   // ���������� middleware �������������� 
//app.UseAuthorization();   // ���������� middleware ����������� 

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationContext>();
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // ������� ��� ���������� ���������� �� timestamp ��� �������� �� postgree
    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true); // ������� ��� ���������� ���������� �� timestamp ��� �������� �� postgree
    //context.Database.EnsureDeleted();
    //context.Database.EnsureCreated();
    DbInitializer.Initialize(context);

}

app.MapRazorPages();
await app.RunAsync();


