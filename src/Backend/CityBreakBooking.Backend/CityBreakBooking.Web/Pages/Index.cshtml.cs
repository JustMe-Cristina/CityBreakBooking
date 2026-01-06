using CityBreakBooking.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    public int DestinationsCount { get; private set; }
    public int TripsCount { get; private set; }
    public int ReservationsCount { get; private set; }
    public int PendingApprovalsCount { get; private set; }

    public List<UpcomingTripVm> UpcomingTrips { get; private set; } = new();

    public async Task OnGetAsync()
    {
        DestinationsCount = await _db.Destinations.CountAsync();
        TripsCount = await _db.Trips.CountAsync();
        ReservationsCount = await _db.Reservations.CountAsync();

        PendingApprovalsCount = await _db.Reservations
            .CountAsync(r => r.Status == Models.Enums.ReservationStatus.Pending);

        // Upcoming trips (top 6)
        UpcomingTrips = await _db.Trips
            .Include(t => t.Destination)
            .Where(t => t.IsActive && t.StartDate >= DateTime.Today)
            .OrderBy(t => t.StartDate)
            .Take(6)
            .Select(t => new UpcomingTripVm
            {
                Id = t.Id,
                Title = t.Title,
                City = t.Destination!.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                PricePerPerson = t.PricePerPerson,
                MaxSeats = t.MaxSeats,
                ImageUrl = t.ImageUrl
            })
            .ToListAsync();
    }

    public class UpcomingTripVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string City { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerPerson { get; set; }
        public int MaxSeats { get; set; }
        public string? ImageUrl { get; set; }
    }
}