using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Reservation> Reservations { get; private set; } = new List<Reservation>();

    public async Task OnGetAsync()
    {
        Reservations = await _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }
}