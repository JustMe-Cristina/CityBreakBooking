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
    public Payment Payment { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Payment = await _db.Payments
            .Include(p => p.Reservation)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Payment is null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null) return NotFound();

        _db.Payments.Remove(payment);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Payment deleted.";
        return RedirectToPage("./Index");
    }
}