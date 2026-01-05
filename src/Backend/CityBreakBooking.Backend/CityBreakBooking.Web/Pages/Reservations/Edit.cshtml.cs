using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = new();

    public List<SelectListItem> TripOptions { get; private set; } = new();
    public List<SelectListItem> StatusOptions { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var res = await _db.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id.Value);
        if (res is null) return NotFound();

        Reservation = res;

        TripOptions = await _db.Trips
            .Include(t => t.Destination)
            .OrderBy(t => t.StartDate)
            .Select(t => new SelectListItem(
                t.Title + " - " + t.Destination!.Name + " (" + t.StartDate.ToString("yyyy-MM-dd") + " → " + t.EndDate.ToString("yyyy-MM-dd") + ")",
                t.Id.ToString()))
            .ToListAsync();

        StatusOptions = Enum.GetValues(typeof(ReservationStatus))
            .Cast<ReservationStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Trip exists?
        var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == Reservation.TripId);
        if (trip is null)
        {
            ModelState.AddModelError("Reservation.TripId", "Selected trip does not exist.");
        }
        else
        {
            // Capacity validation (exclude current reservation)
            var confirmedSeats = await _db.Reservations
                .Where(r => r.TripId == Reservation.TripId
                            && r.Status == ReservationStatus.Confirmed
                            && r.Id != Reservation.Id)
                .SumAsync(r => (int?)r.NumberOfPersons) ?? 0;

            var available = trip.MaxSeats - confirmedSeats;

            if (Reservation.NumberOfPersons > available)
                ModelState.AddModelError("Reservation.NumberOfPersons",
                    "Not enough available seats. Remaining: " + available + ".");
        }

        if (!ModelState.IsValid)
        {
            // rebuild dropdowns
            TripOptions = await _db.Trips
                .Include(t => t.Destination)
                .OrderBy(t => t.StartDate)
                .Select(t => new SelectListItem(
                    t.Title + " - " + t.Destination!.Name + " (" + t.StartDate.ToString("yyyy-MM-dd") + " → " + t.EndDate.ToString("yyyy-MM-dd") + ")",
                    t.Id.ToString()))
                .ToListAsync();

            StatusOptions = Enum.GetValues(typeof(ReservationStatus))
                .Cast<ReservationStatus>()
                .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
                .ToList();

            return Page();
        }

        _db.Attach(Reservation).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}