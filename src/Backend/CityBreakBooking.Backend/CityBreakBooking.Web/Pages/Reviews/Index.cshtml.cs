using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reviews;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Review> Reviews { get; private set; } = new List<Review>();

    public async Task OnGetAsync()
    {
        Reviews = await _db.Reviews
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}