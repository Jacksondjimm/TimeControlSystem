using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesApp.Models
{
    public class TimeTrack // зависимая сущность
    {

        public int Id { get; set; }
        public DateTime dateStamp { get; set; } // time of checkin card
        public bool status { get; set; } // thrue: in office, false: out of office.
        public string? Room { get; set; } // Room number
        public int UserId { get; set; } // foreign key // внешний ключ
        public User? User { get; set; } // navigational property // навигационное свойство
    }
}
