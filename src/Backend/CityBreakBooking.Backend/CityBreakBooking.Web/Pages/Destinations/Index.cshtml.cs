using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Destinations;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    public IList<Destination> Destinations { get; private set; } = new List<Destination>();

    public async Task OnGetAsync()
    {
        Destinations = await _db.Destinations
            .OrderBy(d => d.Country).ThenBy(d => d.Name)
            .ToListAsync();
    }
}