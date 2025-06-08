using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RazorPagesApp.Models;  // пространство имен класса ApplicationContext
using Iuliia; //Транслитерация - библиотека


namespace RazorPagesApp.Pages
{
	public class IndexModel : PageModel
	{
		public string Message = "Незарегистрированный пользователь";   // определяем переменную text
		string? querystring = "";
		string searchNum = "num";
		public string num_ = "";
		public string name_ = "Ivanov.I.I";
		public string status_ = "at office";
		public string time_ = "15:15";

		public int dbCardBuffersCount = 100; //кол-во записей в БД для учёта всех обращений к терминалу считывания карт
		public int iSearch = 0; // самый старый номер записи в БД для учёта всех обращений к терминалу считывания карт

        public int dbTimeTracksCount = 100;//кол-во записей в БД TimeTracks

        ApplicationContext context;
		public List<User> Users { get; private set; } = new();
        public User? UserBuf { get; set; } = new();
        public CardBuffer? CardBuffer  { get; set; } = new();
        public List<CardBuffer> CardBuffers { get; private set; } = new();
        public TimeTrack? TimeTrack { get; set; } = new();
        public TimeTrack? TimeTrackBuf { get; set; } = new();
        public List<TimeTrack> TimeTracks { get; private set; } = new();
        public IndexModel(ApplicationContext db)
		{
			context = db;
		}
        

        public async Task <IActionResult> OnGetAsync() 
        {
            Users = context.Users.ToList();
            CardBuffers = context.CardBuffers.ToList();
            TimeTracks = context.TimeTracks.ToList();
            querystring = Request.QueryString.Value;
            

            if (querystring.IndexOf(searchNum) > 0 && querystring != null)
			{
                await InsertBDCardBuffers();
                await InsertBDTimeTracks();

                num_ = Request.Query["num"];
                User? UserBufTest = Users.FirstOrDefault(u => u.Num == num_);
                if (UserBufTest == null)
                {
                    UserBuf.Surname = "Нет в БД";
                    UserBuf.Name = "Нет в БД";
                    UserBuf.Patronymic = "Нет в БД";
                    TimeTrackBuf.status = false;
                }
                else 
                {
                    UserBuf = UserBufTest;
                    TimeTrackBuf = context.TimeTracks.Include(u => u.User).OrderBy(t => t.dateStamp).LastOrDefault(t => t.UserId == UserBuf.Id);
                }

                string FDStr = IuliiaTranslator.Translate($"{UserBuf.Surname} {UserBuf.Name}", Schemas.Mosmetro); // FDStr - First Display String
                string SDStr = "";// SDStr - Second Display String
                if (TimeTrackBuf.status)
                {
                    SDStr = $" in: {DateTime.Now.ToShortTimeString()}";
                }
                else
                {
                    SDStr = $"out: {DateTime.Now.ToShortTimeString()}";
                }
                
                return Content($"FDStr = {FDStr},EFDStr, SDStr = {SDStr},ESDStr, endssp");
            }

            var user = HttpContext.User.Identity;
            if (user is not null && user.IsAuthenticated)
                Message = $"Пользователь: {user.Name}";
            return Page();

        }

		public async Task InsertBDCardBuffers() // для БД для учёта всех обращений к терминалу считывания карт
        {
            DateTime dateSearch = DateTime.Now;
            string? RequestQueryNum = Request.Query["num"];
            string? RequestQueryRoom = Request.Query["room"];

            User? UserBuf = Users.FirstOrDefault(p=>p.Num == RequestQueryNum); // ищем номер в БД сотрудников
            if (CardBuffers.Count < dbCardBuffersCount) 
            {
                if (UserBuf != null)
                {
                    CardBuffer.Num = RequestQueryNum;
                    CardBuffer.date = DateTime.Now;
                    CardBuffer.Name = UserBuf.Name;
                    CardBuffer.Surname = UserBuf.Surname;
                    CardBuffer.Room = RequestQueryRoom;
                }
                else
                {
                    CardBuffer.Num = RequestQueryNum;
                    CardBuffer.date = DateTime.Now;
                    CardBuffer.Name = "Нет в БД";
                    CardBuffer.Surname = "Нет в БД";
                    CardBuffer.Room = RequestQueryRoom;
                }
                context.CardBuffers.Add(CardBuffer);
            }
            else
            {
                for (int i = 0; i < (CardBuffers.Count); i++)
                {
                    if (CardBuffers[i].date < dateSearch)
                    {
                        dateSearch = CardBuffers[i].date;
                        iSearch = i;
                    }
                }
                CardBuffer = CardBuffers[iSearch];
                if (UserBuf != null)
                {
                    CardBuffer.Num = RequestQueryNum;
                    CardBuffer.date = DateTime.Now;
                    CardBuffer.Name = UserBuf.Name;
                    CardBuffer.Surname = UserBuf.Surname;
                    CardBuffer.Room = RequestQueryRoom;
                }
                else
                {
                    CardBuffer.Num = RequestQueryNum;
                    CardBuffer.date = DateTime.Now;
                    CardBuffer.Name = "Нет в БД";
                    CardBuffer.Surname = "Нет в БД";
                    CardBuffer.Room = RequestQueryRoom;
                }
                context.Update(CardBuffer);

            }
            await context.SaveChangesAsync();
        }

        public async Task InsertBDTimeTracks() // для БД для учёта времени присутствия сотрудников в офисе
        {
            iSearch = 0;
            bool iStatus = true;
            DateTime dateSearch = DateTime.Now;
            string? RequestQueryNum = Request.Query["num"];
            string? RequestQueryRoom = Request.Query["room"];
            //TimeTrack.dateStamp = DateTime.Now;
            User? UserBuf = Users.FirstOrDefault(p => p.Num == RequestQueryNum);

            if (TimeTracks.Count < dbTimeTracksCount)
            {
                if (UserBuf != null)
                {
                    var buf = TimeTracks.Where(t => t.UserId == UserBuf.Id).OrderBy(t => t.dateStamp).LastOrDefault();
                    if (buf == null) iStatus = false;
                    else iStatus = buf.status;
                    if (iStatus == false) TimeTrack.status = true;
                    else TimeTrack.status = false;
                    
                    TimeTrack.dateStamp = DateTime.Now;
                    TimeTrack.Room = RequestQueryRoom;
                    //TimeTrack.UserId = UserBuf.Id; //рабочий вариант
                    TimeTrack.User = UserBuf; // рабочий альтернативный вариант
                    context.TimeTracks.Add(TimeTrack);
                }
                else
                {
                    return;
                }

            }
            else
            {
                if (UserBuf != null)
                {
                    var buf = TimeTracks.OrderBy(t => t.dateStamp).FirstOrDefault();
                    if (buf == null) return;
                    else TimeTrack = buf;

                    buf = TimeTracks.Where(t => t.UserId == UserBuf.Id).OrderBy(t => t.dateStamp).LastOrDefault();
                    if (buf == null) iStatus = false;
                    else iStatus = buf.status;
                    if (iStatus == false) { TimeTrack.status = true; }
                    else { TimeTrack.status = false; }

                    TimeTrack.dateStamp = DateTime.Now;
                    TimeTrack.Room = RequestQueryRoom;
                    //TimeTrack_.UserId = UserBuf.Id; //рабочий вариант
                    TimeTrack.User = UserBuf;// рабочий альтернативный вариант
                    context.Update(TimeTrack);
                }
                else
                {
                    return;
                }

            }
            await context.SaveChangesAsync();
        }
    }

}

/*
 План:
ОК 1. Сделать проверку при занесении такого же номера карты, электронной почты
1.2 Сделать проверку при корректировании на занесение такого же номера карты, электронной почты (см. Edit.cshtml.cs)
OK 2. Отдельно проверить работу только по навигационному полю, без навигационного ключа.
OK 3. Остаться с навигационными ключами? - c навигационным полем остаёмся.
OK 3.1 И далее выводить в "Список посещения" фамилии, имена, эл.почту..
OK 3.2  Сделать логику присутствия/отсутствия в офисе.
OK 4. В "Список посещения" добавить сортировку по сотруднику, отрезку времени.
OK 5. Сделать миграцию БД.
OK 6. Запретить в отчёте текущий день
OK 7. Перейти на PostgreSQL

 */
