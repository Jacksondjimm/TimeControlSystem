using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RazorPagesApp.Models;  // ������������ ���� ������ ApplicationContext
using Iuliia; //�������������� - ����������


namespace RazorPagesApp.Pages
{
	public class IndexModel : PageModel
	{
		public string Message = "�������������������� ������������";   // ���������� ���������� text
		string? querystring = "";
		string searchNum = "num";
		public string num_ = "";
		public string name_ = "Ivanov.I.I";
		public string status_ = "at office";
		public string time_ = "15:15";

		public int dbCardBuffersCount = 100; //���-�� ������� � �� ��� ����� ���� ��������� � ��������� ���������� ����
		public int iSearch = 0; // ����� ������ ����� ������ � �� ��� ����� ���� ��������� � ��������� ���������� ����

        public int dbTimeTracksCount = 100;//���-�� ������� � �� TimeTracks

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
                    UserBuf.Surname = "��� � ��";
                    UserBuf.Name = "��� � ��";
                    UserBuf.Patronymic = "��� � ��";
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
                Message = $"������������: {user.Name}";
            return Page();

        }

		public async Task InsertBDCardBuffers() // ��� �� ��� ����� ���� ��������� � ��������� ���������� ����
        {
            DateTime dateSearch = DateTime.Now;
            string? RequestQueryNum = Request.Query["num"];
            string? RequestQueryRoom = Request.Query["room"];

            User? UserBuf = Users.FirstOrDefault(p=>p.Num == RequestQueryNum); // ���� ����� � �� �����������
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
                    CardBuffer.Name = "��� � ��";
                    CardBuffer.Surname = "��� � ��";
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
                    CardBuffer.Name = "��� � ��";
                    CardBuffer.Surname = "��� � ��";
                    CardBuffer.Room = RequestQueryRoom;
                }
                context.Update(CardBuffer);

            }
            await context.SaveChangesAsync();
        }

        public async Task InsertBDTimeTracks() // ��� �� ��� ����� ������� ����������� ����������� � �����
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
                    //TimeTrack.UserId = UserBuf.Id; //������� �������
                    TimeTrack.User = UserBuf; // ������� �������������� �������
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
                    //TimeTrack_.UserId = UserBuf.Id; //������� �������
                    TimeTrack.User = UserBuf;// ������� �������������� �������
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
 ����:
�� 1. ������� �������� ��� ��������� ������ �� ������ �����, ����������� �����
1.2 ������� �������� ��� ��������������� �� ��������� ������ �� ������ �����, ����������� ����� (��. Edit.cshtml.cs)
OK 2. �������� ��������� ������ ������ �� �������������� ����, ��� �������������� �����.
OK 3. �������� � �������������� �������? - c ������������� ����� �������.
OK 3.1 � ����� �������� � "������ ���������" �������, �����, ��.�����..
OK 3.2  ������� ������ �����������/���������� � �����.
OK 4. � "������ ���������" �������� ���������� �� ����������, ������� �������.
OK 5. ������� �������� ��.
OK 6. ��������� � ������ ������� ����
OK 7. ������� �� PostgreSQL

 */
