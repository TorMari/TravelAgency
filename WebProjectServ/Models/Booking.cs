using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebProjectServ.Models;

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }
    [ValidateNever]
    public Client Client { get; set; }

    [Required]
    public int TourId { get; set; }
    [ValidateNever]
    public Tour Tour { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } 

    public int NumberOfPeople { get; set; }

    public decimal TotalPrice { get; set; }
}
