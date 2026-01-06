using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations.Manage;

[Authorize(Roles = "Admin,TravelAgent")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Reservation> PendingReservations { get; private set; } = new List<Reservation>();

    public async Task OnGetAsync()
    {
        PendingReservations = await _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .Where(r => r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostConfirmAsync(int id)
    {
        var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (reservation is null) return NotFound();

        var trip = await _db.Trips.FirstOrDefaultAsync(t => t.Id == reservation.TripId);
        if (trip is null) return NotFound();

        var confirmedSeats = await _db.Reservations
            .Where(r => r.TripId == reservation.TripId && r.Status == ReservationStatus.Confirmed)
            .SumAsync(r => (int?)r.NumberOfPersons) ?? 0;

        var available = trip.MaxSeats - confirmedSeats;

        if (reservation.NumberOfPersons > available)
        {
            TempData["Error"] = $"Cannot confirm. Not enough seats. Remaining: {available}.";
            return RedirectToPage();
        }

        reservation.Status = ReservationStatus.Confirmed;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Reservation confirmed.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (reservation is null) return NotFound();

        reservation.Status = ReservationStatus.Cancelled;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Reservation cancelled.";
        return RedirectToPage();
    }
}