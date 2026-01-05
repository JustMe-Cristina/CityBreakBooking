using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Payment Payment { get; set; } = new();

    public List<SelectListItem> ReservationOptions { get; private set; } = new();
    public List<SelectListItem> StatusOptions { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var payment = await _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id.Value);
        if (payment is null) return NotFound();

        Payment = payment;

        ReservationOptions = await _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .OrderByDescending(r => r.ReservationDate)
            .Select(r => new SelectListItem(
                "#" + r.Id + " - " + r.Trip!.Title + " (" + r.Trip!.Destination!.Name + ")",
                r.Id.ToString()))
            .ToListAsync();

        StatusOptions = Enum.GetValues(typeof(PaymentStatus))
            .Cast<PaymentStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // ensure reservation exists
        var reservationExists = await _db.Reservations.AnyAsync(r => r.Id == Payment.ReservationId);
        if (!reservationExists)
            ModelState.AddModelError("Payment.ReservationId", "Selected reservation does not exist.");

        // unique payment per reservation (excluding current)
        var duplicate = await _db.Payments.AnyAsync(p => p.ReservationId == Payment.ReservationId && p.Id != Payment.Id);
        if (duplicate)
            ModelState.AddModelError("Payment.ReservationId", "This reservation already has a payment.");

        if (!ModelState.IsValid)
        {
            ReservationOptions = await _db.Reservations
                .Include(r => r.Trip)
                .ThenInclude(t => t!.Destination)
                .OrderByDescending(r => r.ReservationDate)
                .Select(r => new SelectListItem(
                    "#" + r.Id + " - " + r.Trip!.Title + " (" + r.Trip!.Destination!.Name + ")",
                    r.Id.ToString()))
                .ToListAsync();

            StatusOptions = Enum.GetValues(typeof(PaymentStatus))
                .Cast<PaymentStatus>()
                .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
                .ToList();

            return Page();
        }

        _db.Attach(Payment).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}