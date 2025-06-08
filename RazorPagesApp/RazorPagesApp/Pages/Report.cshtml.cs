using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesApp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace RazorPagesApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class ReportModel : PageModel
    {
        ApplicationContext context;
        public List<User> Users { get; private set; } = new();
        public List<TimeTrack> TimeTracks { get; private set; } =new();
        public TimeTrack? TimeTrack { get; set; } = new();
        public ReportModel(ApplicationContext db)
        {
            context = db;
        }

        static List<Month> months { get; } = new()
        {
            new Month(1, "Январь"),
            new Month(2, "Февраль"),
            new Month(3, "Март"),
            new Month(4, "Апрель"),
            new Month(5, "Май"),
            new Month(6, "Июнь"),
            new Month(7, "Июль"),
            new Month(8, "Август"),
            new Month(9, "Сентябрь"),
            new Month(10, "Октябрь"),
            new Month(11, "Ноябрь"),
            new Month(12, "Декабрь")
        };
        public SelectList Months { get; } = new SelectList(months, "Id", "Name");

        [BindProperty]
        public SelectedTime SelectedTime { get; set; } = new SelectedTime(DateTime.Now.Year, DateTime.Now.Month);
        public string Message { get; private set; } = "Выберите время";
        public List<ReportMounth> reportMounthDaily { get; set; } = new();

        public async Task <IActionResult> OnGetAsync() 
        {
            Users = context.Users.AsNoTracking().ToList();
            TimeTracks = context.TimeTracks.Include(u => u.User).AsNoTracking().ToList();
            return Page();
        }

        /*
        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Month? month = months.FirstOrDefault(m => m.Id == SelectedTime.MonthId);
            Message = $"Выбран {month?.Name} {SelectedTime.Year}";
            Users = context.Users.AsNoTracking().ToList();
            TimeTracks = context
                .TimeTracks
                .Include(u => u.User)
                .Where(t => t.dateStamp.Month == month.Id)
                .Where(t => t.dateStamp.Year == SelectedTime.Year)
                .AsNoTracking()
                .ToList();

            for (int i = 1; i <= DateTime.DaysInMonth(SelectedTime.Year, SelectedTime.MonthId); i = i + 1)
            {
                TimeTrack? test_in = context
                    .TimeTracks
                    .Include(u => u.User)
                    .Where(u => u.UserId == 1)
                    .Where(t => t.dateStamp.Month == month.Id)
                    .Where(t => t.dateStamp.Day == i)
                    .AsNoTracking()
                    .FirstOrDefault(t => t.status == true);

                TimeTrack? test_out = context
                    .TimeTracks
                    .Include(u => u.User)
                    .Where(u => u.UserId == 1)
                    .Where(t => t.dateStamp.Month == month.Id)
                    .Where(t => t.dateStamp.Day == i)
                    .AsNoTracking()
                    .FirstOrDefault(t => t.status == false);
                if (test_in != null && test_out != null)
                {
                    ReportMounth reportMounth = new ReportMounth(i, 1, test_out.User, test_in.dateStamp, test_out.dateStamp, (test_out.dateStamp.Hour - test_in.dateStamp.Hour));
                    reportMounthDaily.Add(reportMounth);
                }
            }

            return Page();
        }
        */

        public async Task<IActionResult> OnPostManAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            Users = context.Users.AsNoTracking().ToList();
            Month? month = months.FirstOrDefault(m => m.Id == SelectedTime.MonthId);
            Message = $"Выбран {month?.Name} {SelectedTime.Year}, {Users[id-1].Email}";
            await FixingLosers(id, month);
            TimeTracks = context
                .TimeTracks
                .Include(u => u.User)
                .Where(t => t.dateStamp.Year == SelectedTime.Year)
                .Where(t => t.dateStamp.Month == month.Id)
                .AsNoTracking()
                .ToList();

            for (int i = 1; i <= DateTime.DaysInMonth(SelectedTime.Year, SelectedTime.MonthId); i = i + 1)
            {
                TimeTrack? test_in = context
                    .TimeTracks
                    .Include(u => u.User)
                    .Where(u => u.UserId == id)
                    .Where(t => t.dateStamp.Year == SelectedTime.Year)
                    .Where(t => t.dateStamp.Month == month.Id)
                    .Where(t => t.dateStamp.Day == i)
                    .AsNoTracking()
                    .FirstOrDefault(t => t.status == true);

                TimeTrack? test_out = context
                    .TimeTracks
                    .Include(u => u.User)
                    .Where(u => u.UserId == id)
                    .Where(t => t.dateStamp.Year == SelectedTime.Year)
                    .Where(t => t.dateStamp.Month == month.Id)
                    .Where(t => t.dateStamp.Day == i)
                    .OrderBy(t => t.dateStamp)
                    .AsNoTracking()
                    .LastOrDefault(t => t.status == false);
                if (test_in != null && test_out != null)
                {
                    ReportMounth reportMounth = new ReportMounth(i, 1, test_out.User, test_in.dateStamp, test_out.dateStamp, DayTiming(test_in.dateStamp, test_out.dateStamp));
                    reportMounthDaily.Add(reportMounth);
                }
            }

            return Page();
        }

        public async Task FixingLosers(int id, Month? month)// исправление неудачников - добавление времени ухода из офиса, для тех кто забыл приложить карточку
        {

            for (int i = 1; i <= DateTime.DaysInMonth(SelectedTime.Year, SelectedTime.MonthId); i = i + 1)
            {
                // здесь добавить "continue;" с учетом года, месяца и дня.
                if (i == DateTime.Now.Day && SelectedTime.MonthId == DateTime.Now.Month && DateTime.Now.Year == SelectedTime.Year) 
                    continue;

                TimeTrack = new();
                  TimeTrack? LastDayTrackIn = context
                 .TimeTracks
                 .Include(u => u.User)
                 .Where(u => u.UserId == id)
                 .Where(t => t.dateStamp.Year == SelectedTime.Year)
                 .Where(t => t.dateStamp.Month == month.Id)
                 .Where(t => t.dateStamp.Day == i)
                 .OrderBy(t => t.dateStamp)
                 .LastOrDefault(t => t.status == true);

                TimeTrack? LastDayTrackOut = context
                 .TimeTracks
                 .Include(u => u.User)
                 .Where(u => u.UserId == id)
                 .Where(t => t.dateStamp.Year == SelectedTime.Year)
                 .Where(t => t.dateStamp.Month == month.Id)
                 .Where(t => t.dateStamp.Day == i)
                 .OrderBy(t => t.dateStamp)
                 .LastOrDefault(t => t.status == false);

                if (LastDayTrackIn != null && LastDayTrackOut != null && LastDayTrackIn.dateStamp > LastDayTrackOut.dateStamp)
                {
                    TimeTrack.UserId = id; 
                    TimeTrack.User = LastDayTrackIn.User;
                    TimeTrack.status = false;
                    TimeTrack.dateStamp = new DateTime(LastDayTrackOut.dateStamp.Year,
                                                        LastDayTrackOut.dateStamp.Month,
                                                        LastDayTrackOut.dateStamp.Day, 18, 0, 0);
                    context.TimeTracks.Add(TimeTrack);
                }

                if (LastDayTrackIn != null && LastDayTrackOut == null) 
                { 
                    if (LastDayTrackIn.dateStamp.Hour < 18)
                    {
                        TimeTrack.UserId = id; 
                        TimeTrack.User = LastDayTrackIn.User;
                        TimeTrack.status = false;
                        TimeTrack.dateStamp = new DateTime(LastDayTrackIn.dateStamp.Year,
                                                            LastDayTrackIn.dateStamp.Month,
                                                            LastDayTrackIn.dateStamp.Day, 18, 0, 0);
                        context.TimeTracks.Add(TimeTrack);
                    }
                    else
                    {
                        TimeTrack.UserId = id; 
                        TimeTrack.User = LastDayTrackIn.User;
                        TimeTrack.status = false;
                        TimeTrack.dateStamp = new DateTime(LastDayTrackIn.dateStamp.Year,
                                                            LastDayTrackIn.dateStamp.Month,
                                                            LastDayTrackIn.dateStamp.Day,
                                                            LastDayTrackIn.dateStamp.Hour+1, 0, 0);
                        context.TimeTracks.Add(TimeTrack);
                    }
                    
                }
                await context.SaveChangesAsync();
            }

        }

        public float DayTiming(DateTime dateStampIn, DateTime dateStampOut)
        {
            float TimeIn = dateStampIn.Hour*60 + dateStampIn.Minute;
            float TimeOut = dateStampOut.Hour * 60 + dateStampOut.Minute;
            return Single.Round(((TimeOut - TimeIn) / 60),2);
        }
    }


    public record class SelectedTime(int Year, int MonthId);
    public record class Month(int Id, string Name);
    public record class ReportMounth (int Id, int UserId, User? User, DateTime dateStampIn, DateTime dateStampOut, float WorkDayDura);

}
