using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Trips;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public DetailsModel(AppDbContext db) => _db = db;

    public Trip Trip { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var trip = await _db.Trips
            .Include(t => t.Destination)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id.Value);

        if (trip is null) return NotFound();

        Trip = trip;
        return Page();
    }
}