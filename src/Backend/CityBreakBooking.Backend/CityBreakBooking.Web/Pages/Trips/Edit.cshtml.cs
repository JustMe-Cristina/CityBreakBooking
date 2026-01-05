using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Trips;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Trip Trip { get; set; } = new();

    public List<SelectListItem> DestinationOptions { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == id.Value);
        if (trip is null) return NotFound();

        Trip = trip;

        DestinationOptions = await _db.Destinations
            .OrderBy(d => d.Country).ThenBy(d => d.Name)
            .Select(d => new SelectListItem($"{d.Name} ({d.Country})", d.Id.ToString()))
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Trip.EndDate <= Trip.StartDate)
            ModelState.AddModelError("Trip.EndDate", "End date must be after start date.");

        if (!ModelState.IsValid)
        {
            DestinationOptions = await _db.Destinations
                .OrderBy(d => d.Country).ThenBy(d => d.Name)
                .Select(d => new SelectListItem($"{d.Name} ({d.Country})", d.Id.ToString()))
                .ToListAsync();
            return Page();
        }

        _db.Attach(Trip).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}