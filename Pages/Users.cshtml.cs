using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication4.Models;

namespace WebApplication4.Pages
{
    [Authorize(Policy = "MustBelongToUser")]
    public class UsersModel : PageModel
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<UsersModel> _logger;

        public UsersModel(ILogger<UsersModel> logger, ApiDbContext context)
        {
            _logger = logger;
            _context = context;

        }

        public List<User> users { get; set; }
        [BindProperty] public List<int> selectedUserIds { get; set; }

        public void OnGet()
        {
            users = _context.Users.Where(u => u.IsDeleted == false).ToList();
        }

        public IActionResult OnPost()
        {
            if (selectedUserIds.Any())
            {
                var usersToUpdate = _context.Users.Where(u => selectedUserIds.Contains(u.Id)).ToList();

                if (Request.Form.ContainsKey("deleteUser"))
                {

                    foreach (var user in usersToUpdate)
                    {
                        user.IsDeleted = true;
                    }
                }

                if (Request.Form.ContainsKey("blockUser"))
                {
                    foreach (var user in usersToUpdate)
                    {
                        user.IsBlocked = true;
                    }
                }

                if (Request.Form.ContainsKey("unblockUser"))
                {
                    foreach (var user in usersToUpdate)
                    {
                        user.IsBlocked = false;
                    }
                }
                
                _context.SaveChanges();
                
                var userEmailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;

                var currentUser = _context.Users.FirstOrDefault(u => u.Email == userEmailClaim);

                if (currentUser != null && (currentUser.IsBlocked == true || currentUser.IsDeleted == true))
                {
                    return RedirectToPage("/Logout");
                }

            }
            return RedirectToPage("/Users");

        }
    }
}