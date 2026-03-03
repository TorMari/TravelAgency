using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebProjectServ.Models;

public class Booking
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Client is required")]
    public int ClientId { get; set; }

    [ValidateNever]
    public Client Client { get; set; }

    [Required(ErrorMessage = "Tour is required")]
    public int TourId { get; set; }

    [ValidateNever]
    public Tour Tour { get; set; }

    public DateTime BookingDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [Range(1, 20, ErrorMessage = "Number of people must be between 1 and 20")]
    public int NumberOfPeople { get; set; }

    public decimal TotalPrice { get; set; }
}
