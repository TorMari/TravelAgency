using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebProjectServ.Models
{
    public class Client : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(20)]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\+\d+$", ErrorMessage = "Phone must contain only digits and start with the '+' symbol")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone must be between 10 and 15 digits")]
        public string Phone { get; set; }


        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth.ToDateTime(TimeOnly.MinValue) > DateTime.Today.AddYears(-18))
            {
                yield return new ValidationResult(
                    "Client must be at least 18 years old.",
                    new[] { nameof(DateOfBirth) });
            }
        }

        public ICollection<Booking>? Bookings { get; set; } = new List<Booking>();
        public string FullName => $"{FirstName} {LastName}";

        [ValidateNever]
        public ApplicationUser? User { get; set; }

    }
}
