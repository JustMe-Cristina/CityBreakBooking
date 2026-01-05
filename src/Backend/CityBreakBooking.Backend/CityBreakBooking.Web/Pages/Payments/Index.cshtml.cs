using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Payment> Payments { get; private set; } = new List<Payment>();

    public async Task OnGetAsync()
    {
        Payments = await _db.Payments
            .Include(p => p.Reservation)
            .ThenInclude(r => r!.Trip)
            .ThenInclude(t => t!.Destination)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}