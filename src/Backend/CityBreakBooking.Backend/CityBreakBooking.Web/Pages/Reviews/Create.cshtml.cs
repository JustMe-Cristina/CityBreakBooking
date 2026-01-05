using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reviews;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Review Review { get; set; } = new();

    public List<SelectListItem> TripOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        TripOptions = await _db.Trips
            .Include(t => t.Destination)
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.EndDate)
            .Select(t => new SelectListItem(
                t.Title + " - " + t.Destination!.Name + " (" + t.StartDate.ToString("yyyy-MM-dd") + " → " + t.EndDate.ToString("yyyy-MM-dd") + ")",
                t.Id.ToString()))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Review.UserId))
            Review.UserId = "demo-user";

        var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == Review.TripId);
        if (trip is null)
        {
            ModelState.AddModelError("Review.TripId", "Selected trip does not exist.");
        }
        else
        {
            // Custom validation: only after trip ends
            if (trip.EndDate >= DateTime.Now)
                ModelState.AddModelError("Review.TripId", "You can add a review only after the trip has ended.");
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        Review.CreatedAt = DateTime.Now;

        _db.Reviews.Add(Review);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}