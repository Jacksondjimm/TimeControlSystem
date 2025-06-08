using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RazorPagesApp.Models
{
    public class CardBuffer
    {

        public int Id { get; set; }
        public string? Name { get; set; } // имя пользователя
        public string? Patronymic { get; set; } // отчество пользователя
        public string? Surname { get; set; } // имя пользователя
        public string? Num { get; set; } //card number
        public string? Room { get; set; } // room number
        public DateTime date { get; set; }
    }
}
