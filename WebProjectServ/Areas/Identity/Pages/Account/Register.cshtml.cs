using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebProjectServ.Models;

namespace WebProjectServ.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyDataContext _context;

        public RegisterModel(UserManager<ApplicationUser> userManager,
                             MyDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var client = new Client
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Email = Input.Email,
                    Phone = Input.Phone,
                    DateOfBirth = Input.DateOfBirth
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                user.ClientId = client.Id;
                await _userManager.UpdateAsync(user);

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

    }
}
