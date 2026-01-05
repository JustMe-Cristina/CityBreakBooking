using System.ComponentModel.DataAnnotations;
using CityBreakBooking.Web.Models.Enums;

namespace CityBreakBooking.Web.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }

    [Range(0, 100000)]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
}