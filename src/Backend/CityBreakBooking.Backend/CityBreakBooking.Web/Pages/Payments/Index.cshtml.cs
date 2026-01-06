using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Payment> Payments { get; private set; } = new List<Payment>();

    public PaymentStatus? StatusFilter { get; set; }

    public async Task OnGetAsync(PaymentStatus? status)
    {
        StatusFilter = status;

        var query = _db.Payments
            .Include(p => p.Reservation)
            .ThenInclude(r => r.Trip)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        Payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}