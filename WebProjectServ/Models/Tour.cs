namespace WebProjectServ.Models
{
    public class Tour
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public DateTime StartDate { get; set; }

        public string Description { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
