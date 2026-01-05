using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = new();

    public List<SelectListItem> TripOptions { get; private set; } = new();
    public List<SelectListItem> StatusOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        TripOptions = await _db.Trips
            .Include(t => t.Destination)
            .Where(t => t.IsActive)
            .OrderBy(t => t.StartDate)
            .Select(t => new SelectListItem(
                t.Title + " - " + t.Destination!.Name + " (" + t.StartDate.ToString("yyyy-MM-dd") + " → " + t.EndDate.ToString("yyyy-MM-dd") + ")",
                t.Id.ToString()))
            .ToListAsync();

        StatusOptions = Enum.GetValues(typeof(ReservationStatus))
            .Cast<ReservationStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        Reservation.Status = ReservationStatus.Pending;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Reservation.UserId))
            Reservation.UserId = "demo-user";

        // Trip exists?
        var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == Reservation.TripId);
        if (trip is null)
        {
            ModelState.AddModelError("Reservation.TripId", "Selected trip does not exist.");
        }
        else
        {
            // Capacity validation (only Confirmed count as occupied)
            var confirmedSeats = await _db.Reservations
                .Where(r => r.TripId == Reservation.TripId && r.Status == ReservationStatus.Confirmed)
                .SumAsync(r => (int?)r.NumberOfPersons) ?? 0;

            var available = trip.MaxSeats - confirmedSeats;

            if (Reservation.NumberOfPersons > available)
                ModelState.AddModelError("Reservation.NumberOfPersons",
                    "Not enough available seats. Remaining: " + available + ".");
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync(); // rebuild dropdowns
            return Page();
        }

        Reservation.ReservationDate = DateTime.Now;

        _db.Reservations.Add(Reservation);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}