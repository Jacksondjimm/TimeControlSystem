using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesApp.Models;

namespace RazorPagesApp.Pages
{
    public class VisitorLogModel : PageModel
    {
        ApplicationContext context;
        public List<User> Users { get; private set; } = new();
        public List<TimeTrack> TimeTracks { get; private set; } = new();
        public VisitorLogModel(ApplicationContext db)
        {
            context = db;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Users = context.Users.AsNoTracking().ToList();
            TimeTracks = context.TimeTracks.Include(u => u.User).AsNoTracking().ToList();
            return Page();
        }
    }
}


