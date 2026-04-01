using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebProjectServ.Models
{
    public class ConversationUser
    {
        public int ConversationId { get; set; }
        [ValidateNever]
        public Conversation Conversation { get; set; }

        public string UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
