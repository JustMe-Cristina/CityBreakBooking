using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Reservation Reservation { get; set; } = new();

    public List<SelectListItem> TripOptions { get; private set; } = new();
    public List<SelectListItem> StatusOptions { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var res = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == id.Value);
        if (res is null) return NotFound();

        Reservation = res;

        TripOptions = await _db.Trips
            .Include(t => t.Destination)
            .OrderBy(t => t.StartDate)
            .Select(t => new SelectListItem(
                $"{t.Title} - {t.Destination!.Name} ({t.StartDate:yyyy-MM-dd} → {t.EndDate:yyyy-MM-dd})",
                t.Id.ToString()))
            .ToListAsync();

        StatusOptions = Enum.GetValues<ReservationStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validate seats again (if trip or number changed)
        var trip = await _db.Trips
            .Include(t => t.Reservations)
            .FirstOrDefaultAsync(t => t.Id == Reservation.TripId);

        if (trip is null)
        {
            ModelState.AddModelError("Reservation.TripId", "Selected trip does not exist.");
        }
        else
        {
            var alreadyReserved = trip.Reservations
                .Where(r => r.Id != Reservation.Id && r.Status != ReservationStatus.Cancelled)
                .Sum(r => r.NumberOfPersons);

            var remaining = trip.MaxSeats - alreadyReserved;
            if (Reservation.NumberOfPersons > remaining)
                ModelState.AddModelError("Reservation.NumberOfPersons", $"Not enough seats. Remaining: {remaining}.");
        }

        if (!ModelState.IsValid)
        {
            TripOptions = await _db.Trips
                .Include(t => t.Destination)
                .OrderBy(t => t.StartDate)
                .Select(t => new SelectListItem(
                    $"{t.Title} - {t.Destination!.Name} ({t.StartDate:yyyy-MM-dd} → {t.EndDate:yyyy-MM-dd})",
                    t.Id.ToString()))
                .ToListAsync();

            StatusOptions = Enum.GetValues<ReservationStatus>()
                .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
                .ToList();

            return Page();
        }

        _db.Attach(Reservation).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}