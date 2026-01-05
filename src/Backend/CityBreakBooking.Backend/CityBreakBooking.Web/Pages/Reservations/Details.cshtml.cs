using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public DetailsModel(AppDbContext db) => _db = db;

    public Reservation Reservation { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var res = await _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id.Value);

        if (res is null) return NotFound();

        Reservation = res;
        return Page();
    }
}