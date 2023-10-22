using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication4.Pages;

public class Login : PageModel
{
    [BindProperty]
    public Credentials credentials { get; set; }
    private readonly ApiDbContext _context;
    private readonly ILogger<UsersModel> _logger;

    public Login(ILogger<UsersModel> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;

    }

    public void OnGet()
    {
        
    }

    public class Credentials
    {   [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == credentials.email);

            if (user.IsBlocked==true || user.IsDeleted==true) return RedirectToPage("/Login");

            if (user != null && BCrypt.Net.BCrypt.Verify(credentials.password, user.PasswordHash))
            {   
                user.LastLogin = DateTime.UtcNow;
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("isBlocked", user.IsBlocked.ToString().ToLower())
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                await _context.SaveChangesAsync();
                
                return RedirectToPage("/Users"); 
            }
    
        }
        
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        
        return Page();
    }
}