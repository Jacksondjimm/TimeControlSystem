using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesApp.Models;


namespace RazorPagesApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class EditModel : PageModel
    {
        ApplicationContext context;
        public string Message = "�������������� ������������";   // ���������� ���������� text
        public int UserId; //�� ������������ �����!!!!!!!
        public List<User> Users { get; private set; } = new();
        [BindProperty]
        public User? Person { get; set; }
        
        public EditModel (ApplicationContext db)
        {
            context= db;    
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Person = await context.Users.FindAsync(id);
            if (Person == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Users = context.Users.ToList();
            // ����� ����� �������� ������ ������������ ����� ��� ����� ����� ������ ����������. ������� ������ �� ������ � ����������.
            // ��������� � �� �������� ���������� ������ ���� (���� ���� ��������� ����������� �������� ���������� �� �� Index.cshtml):

            /*if ( context.Users.FirstOrDefault(u=>u.Email == Person.Email).Id != Person.Id )
            {
                Message = $"����� �������� ����� ��� �����: {Person.Email}";
                return Page();
            }
            // end*/
            context.Users.Update(Person!);
            await context.SaveChangesAsync();
            return RedirectToPage("manage");
        }
    }
}
