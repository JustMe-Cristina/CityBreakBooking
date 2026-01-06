using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public DetailsModel(AppDbContext db) => _db = db;

    public Payment Payment { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Payment = await _db.Payments
            .Include(p => p.Reservation)
            .ThenInclude(r => r.Trip)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Payment is null) return NotFound();
        return Page();
    }
}