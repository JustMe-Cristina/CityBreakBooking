using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reviews;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Review Review { get; set; } = new();

    public List<SelectListItem> TripOptions { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var review = await _db.Reviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id.Value);
        if (review is null) return NotFound();

        Review = review;

        TripOptions = await _db.Trips
            .Include(t => t.Destination)
            .OrderByDescending(t => t.EndDate)
            .Select(t => new SelectListItem(
                t.Title + " - " + t.Destination!.Name + " (" + t.StartDate.ToString("yyyy-MM-dd") + " → " + t.EndDate.ToString("yyyy-MM-dd") + ")",
                t.Id.ToString()))
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == Review.TripId);
        if (trip is null)
        {
            ModelState.AddModelError("Review.TripId", "Selected trip does not exist.");
        }
        else
        {
            //Custom validation: only after trip ends
            if (trip.EndDate >= DateTime.Now)
                ModelState.AddModelError("Review.TripId", "You can edit a review only after the trip has ended.");
        }

        if (!ModelState.IsValid)
        {
            TripOptions = await _db.Trips
                .Include(t => t.Destination)
                .OrderByDescending(t => t.EndDate)
                .Select(t => new SelectListItem(
                    t.Title + " - " + t.Destination!.Name + " (" + t.StartDate.ToString("yyyy-MM-dd") + " → " + t.EndDate.ToString("yyyy-MM-dd") + ")",
                    t.Id.ToString()))
                .ToListAsync();

            return Page();
        }

        // keep CreatedAt if user didn't send it
        if (Review.CreatedAt == default)
            Review.CreatedAt = DateTime.Now;

        _db.Attach(Review).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}