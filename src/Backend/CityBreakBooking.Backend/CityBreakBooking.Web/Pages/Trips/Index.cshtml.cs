using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Trips;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Trip> Trips { get; private set; } = new List<Trip>();

    public async Task OnGetAsync()
    {
        Trips = await _db.Trips
            .Include(t => t.Destination)
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }
}