using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication4.Models;

namespace WebApplication4.Pages;

public class RegisterModel : PageModel
{
    private readonly ApiDbContext _context;

    public RegisterModel(ApiDbContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        
    }
    
    [BindProperty]
    public User user { get; set; }
    
    public IActionResult OnPost()
    {
        var newUser = new User()
        {
            Name = user.Name,
            Email = user.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
            CreatedAt = DateTime.UtcNow,
            Status = "Active",
            LastLogin = DateTime.UtcNow,
            IsBlocked = false,
            IsDeleted = false,
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        return RedirectToPage("/Users");
    }
}