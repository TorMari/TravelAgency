using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebProjectServ.Models
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int TourId { get; set; }
        public Tour? Tour { get; set; }
        public string Status { get; set; }
        public int NumberOfPeople { get; set; }

        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
