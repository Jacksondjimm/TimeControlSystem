using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesApp.Models
{
    public class User // главная сущность
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]// не помогло 
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]// не помогло 
        public int Id { get; set; }
        public string? Name { get; set; } // имя пользователя
        public string? Patronymic { get; set; } // отчество пользователя
        public string? Surname { get; set; } // фамилия пользователя
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Num { get; set; } //card number
        public List<TimeTrack> TimeTracks { get; set; } = new();// навигационное свойство
    }
}
