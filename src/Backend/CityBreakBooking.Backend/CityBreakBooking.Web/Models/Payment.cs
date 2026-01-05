using System.ComponentModel.DataAnnotations;

namespace CityBreakBooking.Web.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }

    [Range(0, 10000)]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
}