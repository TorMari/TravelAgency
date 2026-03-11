using Microsoft.AspNetCore.Identity;

namespace WebProjectServ.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? ClientId { get; set; }

        public Client Client { get; set; }
    }
}
