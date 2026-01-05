using System.ComponentModel.DataAnnotations;

namespace CityBreakBooking.Web.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int TripId { get; set; }
    public Trip? Trip { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; } = DateTime.Now;

    [Range(1, 10)]
    public int NumberOfPersons { get; set; }

    [Required]
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
}