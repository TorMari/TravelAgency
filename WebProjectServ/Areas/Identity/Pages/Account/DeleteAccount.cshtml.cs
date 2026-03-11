using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebProjectServ.Models;

namespace WebProjectServ.Areas.Identity.Pages.Account
{
    public class DeleteAccountModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyDataContext _context;

        public DeleteAccountModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            MyDataContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Index", new { area = "" });

            if (user.ClientId != null)
            {
                var client = await _context.Clients.FindAsync(user.ClientId);
                if (client != null)
                    _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return Page();
            }

            await _signInManager.SignOutAsync();

            return RedirectToPage("/Index", new { area = "" });
        }
    }
}
