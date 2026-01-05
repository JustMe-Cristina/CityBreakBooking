using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Trips;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    public DeleteModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Trip Trip { get; set; } = new();

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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null) return NotFound();

        var trip = await _db.Trips.FindAsync(id.Value);
        if (trip is null) return NotFound();

        _db.Trips.Remove(trip);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}