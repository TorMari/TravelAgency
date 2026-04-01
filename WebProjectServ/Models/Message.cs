using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.VisualBasic;

namespace WebProjectServ.Models
{
  
    public class Message
    {
        public int Id { get; set; }

        public string SenderId { get; set; }     
        public string SenderName { get; set; }      

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public string? Text { get; set; }

        public int? TourId { get; set; }           

        public string? ReceiverId { get; set; }     

        public string? FileName { get; set; }
        public string? FileUrl { get; set; }       
        public string? FileType { get; set; }       
    }
}
