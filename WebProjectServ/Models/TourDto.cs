namespace WebProjectServ.Models
{
    public class TourDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; }
        public List<BookingDto>? Bookings { get; set; }
    }
}
