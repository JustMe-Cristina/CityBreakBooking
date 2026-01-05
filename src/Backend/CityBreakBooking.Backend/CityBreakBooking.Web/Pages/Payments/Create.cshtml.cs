using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Payment Payment { get; set; } = new();

    public List<SelectListItem> ReservationOptions { get; private set; } = new();
    public List<SelectListItem> StatusOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var alreadyPaid = await _db.Payments.Select(p => p.ReservationId).ToListAsync();

        ReservationOptions = await _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .Where(r => !alreadyPaid.Contains(r.Id))
            .OrderByDescending(r => r.ReservationDate)
            .Select(r => new SelectListItem(
                "#" + r.Id + " - " + r.Trip!.Title + " (" + r.Trip!.Destination!.Name + ")",
                r.Id.ToString()))
            .ToListAsync();

        StatusOptions = Enum.GetValues(typeof(PaymentStatus))
            .Cast<PaymentStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        Payment.Status = PaymentStatus.Paid;
        Payment.PaymentDate = DateTime.Now;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var reservationExists = await _db.Reservations.AnyAsync(r => r.Id == Payment.ReservationId);
        if (!reservationExists)
            ModelState.AddModelError("Payment.ReservationId", "Selected reservation does not exist.");

        var duplicate = await _db.Payments.AnyAsync(p => p.ReservationId == Payment.ReservationId);
        if (duplicate)
            ModelState.AddModelError("Payment.ReservationId", "This reservation already has a payment.");

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        Payment.PaymentDate = DateTime.Now;

        _db.Payments.Add(Payment);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}