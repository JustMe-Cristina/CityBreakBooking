using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

[Authorize(Roles = "Admin,TravelAgent")]
public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Payment Payment { get; set; } = new();

    public SelectList ReservationOptions { get; set; } = default!;

    public async Task OnGetAsync()
    {
        await LoadReservationsAsync();
        Payment.PaymentDate = DateTime.Now;
        Payment.Status = PaymentStatus.Pending;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // dacă nu e selectat
        if (Payment.ReservationId <= 0)
            ModelState.AddModelError("Payment.ReservationId", "Please select a reservation.");

        // dacă deja are payment (din cauza unique)
        var alreadyExists = Payment.ReservationId > 0 &&
                            await _db.Payments.AnyAsync(p => p.ReservationId == Payment.ReservationId);

        if (alreadyExists)
            ModelState.AddModelError("Payment.ReservationId", "This reservation already has a payment.");

        if (!ModelState.IsValid)
        {
            await LoadReservationsAsync();
            return Page();
        }

        _db.Payments.Add(Payment);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Could not save payment. This reservation might already have a payment.");
            await LoadReservationsAsync();
            return Page();
        }

        TempData["Success"] = "Payment created successfully.";
        return RedirectToPage("./Index");
    }

    private async Task LoadReservationsAsync()
    {
        // Recomandat: arată doar rezervările care NU au încă payment
        var reservations = await _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .Where(r => !_db.Payments.Any(p => p.ReservationId == r.Id))
            .OrderByDescending(r => r.ReservationDate)
            .Select(r => new
            {
                r.Id,
                Text = $"#{r.Id} | {r.Trip!.Title} | {r.UserEmail} | {r.Status}"
            })
            .ToListAsync();

        ReservationOptions = new SelectList(reservations, "Id", "Text");
    }
}