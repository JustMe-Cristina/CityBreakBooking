using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db) => _db = db;

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
                $"{t.Title} - {t.Destination!.Name} ({t.StartDate:yyyy-MM-dd} → {t.EndDate:yyyy-MM-dd})",
                t.Id.ToString()))
            .ToListAsync();

        StatusOptions = Enum.GetValues<ReservationStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        Reservation.Status = ReservationStatus.Pending;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Demo until auth: set a placeholder user id
        if (string.IsNullOrWhiteSpace(Reservation.UserId))
            Reservation.UserId = "demo-user";

        // Custom validation: seats available
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
                .Where(r => r.Status != ReservationStatus.Cancelled)
                .Sum(r => r.NumberOfPersons);

            var remaining = trip.MaxSeats - alreadyReserved;
            if (Reservation.NumberOfPersons > remaining)
                ModelState.AddModelError("Reservation.NumberOfPersons", $"Not enough seats. Remaining: {remaining}.");
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        Reservation.ReservationDate = DateTime.Now;

        _db.Reservations.Add(Reservation);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}