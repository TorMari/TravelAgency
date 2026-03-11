using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebProjectServ.Models;

namespace WebProjectServ.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
                          UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, false);

                if (result.Succeeded)
                {
                    var claims = new List<Claim>();

                    if (user.ClientId != null)
                    {
                        claims.Add(new Claim("ClientId", user.ClientId.ToString()));
                    }

                    await _signInManager.SignInWithClaimsAsync(user, false, claims);

                    return RedirectToPage("/Index");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return Page();
        }
    }
}
