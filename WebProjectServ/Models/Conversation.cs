namespace WebProjectServ.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public int? TourId { get; set; } 
        public bool IsSupport { get; set; } 

        public ICollection<ConversationUser> Participants { get; set; } = new List<ConversationUser>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
