using System.ComponentModel.DataAnnotations;

namespace CityBreakBooking.Web.Models;

public class Payment
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a reservation.")]
    public int ReservationId { get; set; }

    // IMPORTANT: navigation NU trebuie Required la form binding
    public Reservation? Reservation { get; set; }

    [Range(0.01, 100000, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    public Enums.PaymentStatus Status { get; set; } = Enums.PaymentStatus.Pending;
}