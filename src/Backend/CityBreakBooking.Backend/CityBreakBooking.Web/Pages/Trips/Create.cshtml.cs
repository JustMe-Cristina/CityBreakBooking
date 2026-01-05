using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Trips;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Trip Trip { get; set; } = new();

    public List<SelectListItem> DestinationOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        DestinationOptions = await _db.Destinations
            .OrderBy(d => d.Country).ThenBy(d => d.Name)
            .Select(d => new SelectListItem($"{d.Name} ({d.Country})", d.Id.ToString()))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Custom validation: StartDate < EndDate
        if (Trip.EndDate <= Trip.StartDate)
            ModelState.AddModelError("Trip.EndDate", "End date must be after start date.");

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        _db.Trips.Add(Trip);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}