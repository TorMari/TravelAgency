namespace WebProjectServ.Models
{
    public class Client
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
