using System.ComponentModel.DataAnnotations;
using CityBreakBooking.Web.Models.Enums;

namespace CityBreakBooking.Web.Models;

// cd /Users/cristina-pop/MP/CityBreakBooking/src/Backend/CityBreakBooking.Backend/CityBreakBooking.Web

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int TripId { get; set; }
    public Trip? Trip { get; set; }

    // ID tehnic venit din aplicația mobile (ex: customer7)
    [Required]
    public string UserId { get; set; } = string.Empty;

    // Email pentru afișare în Web (admin / agent)
    [Required, EmailAddress]
    public string UserEmail { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; } = DateTime.Now;

    [Range(1, 10)]
    public int NumberOfPersons { get; set; }

    [Required]
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    
    public Payment? Payment { get; set; }
}