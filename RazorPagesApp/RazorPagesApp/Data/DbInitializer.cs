using RazorPagesApp.Models;

namespace RazorPagesApp.Data
{
    public static class DbInitializer 
    {
        public static void Initialize(ApplicationContext context) 
        { 
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            var users = new User[]
            {
                new User{ Name = "Ded", Surname = "Moroz", Email = "D@M.ru", Password = "11111", Role = "adminRole", Num = "1001"},
                new User{ Name = "Snezhanna", Surname = "Morozovna", Email = "S@M.ru", Password = "11111", Role = "userRole", Num = "1002"},
                new User{ Name = "Olen", Surname = "Morozovich", Email = "O@M.ru", Password = "11111", Role = "userRole", Num = "1003"}
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            var timetracks = new TimeTrack[]
            {
                new TimeTrack{ dateStamp = DateTime.Parse("01/04/2025 09:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("01/04/2025 09:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("01/04/2025 09:05:01"), status = true, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("01/04/2025 18:40:01"), status = false, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("01/04/2025 18:05:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("01/04/2025 18:10:01"), status = false, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("02/04/2025 09:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("02/04/2025 09:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("02/04/2025 09:05:01"), status = true, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("02/04/2025 18:40:01"), status = false, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("02/04/2025 18:05:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("02/04/2025 18:10:01"), status = false, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("03/04/2025 09:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("03/04/2025 09:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("03/04/2025 09:05:01"), status = true, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("03/04/2025 18:40:01"), status = false, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("03/04/2025 18:05:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("03/04/2025 18:10:01"), status = false, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("04/04/2025 09:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("04/04/2025 09:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("04/04/2025 09:05:01"), status = true, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("04/04/2025 18:40:01"), status = false, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("04/04/2025 18:05:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("04/04/2025 18:10:01"), status = false, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("05/04/2025 09:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("05/04/2025 09:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("05/04/2025 09:05:01"), status = true, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("05/04/2025 18:40:01"), status = false, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("05/04/2025 18:05:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("05/04/2025 18:10:01"), status = false, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("01/05/2025 09:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("01/05/2025 09:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("01/05/2025 09:05:01"), status = true, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 10:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 10:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 10:05:01"), status = true, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 16:00:01"), status = false, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 16:05:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 16:10:01"), status = false, UserId = 3},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 17:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 17:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("02/05/2025 17:05:01"), status = true, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("03/05/2025 19:00:01"), status = true, UserId = 1},
                new TimeTrack{ dateStamp = DateTime.Parse("03/05/2025 19:01:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("03/05/2025 19:05:01"), status = true, UserId = 3},

                new TimeTrack{ dateStamp = DateTime.Parse("12/05/2025 09:00:01"), status = true, UserId = 1},

                new TimeTrack{ dateStamp = DateTime.Parse("12/05/2025 08:10:01"), status = true, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("12/05/2025 9:20:01"), status = false, UserId = 2},
                new TimeTrack{ dateStamp = DateTime.Parse("12/05/2025 10:15:01"), status = true, UserId = 2}

            };
            context.TimeTracks.AddRange(timetracks);
            context.SaveChanges();

            var cardbuffers = new CardBuffer[]
            {
                new CardBuffer{Name = "Ded", Surname = "Moroz", Num = "1001", date = DateTime.Parse("01/04/2025 09:00:01") },
                new CardBuffer{Name = "Snezhanna", Surname = "Morozovna", Num = "1002", date = DateTime.Parse("01/04/2025 09:01:01") },
                new CardBuffer{Name = "Olen", Surname = "Morozovich", Num = "1003", date = DateTime.Parse("01/04/2025 09:05:01") },
                new CardBuffer{Name = "Ded", Surname = "Moroz", Num = "1001", date = DateTime.Parse("01/04/2025 18:00:01") },
                new CardBuffer{Name = "Snezhanna", Surname = "Morozovna", Num = "1002", date = DateTime.Parse("01/04/2025 18:05:01")},
                new CardBuffer{Name = "Olen", Surname = "Morozovich", Num = "1003", date = DateTime.Parse("01/04/2025 18:10:01") }

            };
            context.CardBuffers.AddRange(cardbuffers);
            context.SaveChanges();

        }

    }
}
