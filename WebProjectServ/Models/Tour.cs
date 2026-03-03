using System.ComponentModel.DataAnnotations;

namespace WebProjectServ.Models
{
    public class Tour : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name of tour is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(20)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 100000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration of tour is required")]
        [Range(1, 30, ErrorMessage = "Number of days must be between 1 and 30")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Start data is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.Date <= DateTime.Today)
            {
                yield return new ValidationResult(
                    "Tour start date must be in the future.",
                    new[] { nameof(StartDate) });
            }
        }

        [StringLength(1000)]
        public string Description { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
