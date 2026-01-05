using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    public DeleteModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Payment Payment { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var payment = await _db.Payments
            .Include(p => p.Reservation)
            .ThenInclude(r => r!.Trip)
            .ThenInclude(t => t!.Destination)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id.Value);

        if (payment is null) return NotFound();

        Payment = payment;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null) return NotFound();

        var payment = await _db.Payments.FindAsync(id.Value);
        if (payment is null) return NotFound();

        _db.Payments.Remove(payment);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}