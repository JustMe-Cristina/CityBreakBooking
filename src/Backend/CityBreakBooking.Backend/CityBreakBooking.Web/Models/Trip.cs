using System.ComponentModel.DataAnnotations;

namespace CityBreakBooking.Web.Models;

public class Trip
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DestinationId { get; set; }
    public Destination? Destination { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Range(0, 10000)]
    public decimal PricePerPerson { get; set; }

    [Range(1, 100)]
    public int MaxSeats { get; set; }

    public bool IsActive { get; set; } = true;

    // Optional - pentru mobil (afișezi poza în MAUI)
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    // Relationship: 1 Trip -> many Reservations
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}