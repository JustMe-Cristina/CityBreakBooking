using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Destinations;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Destination> Destinations { get; set; } = new List<Destination>();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    // null = All, true = Active, false = Inactive
    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<Destination> q = _db.Destinations;

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var s = Search.Trim();
            q = q.Where(d => d.Name.Contains(s) || d.Country.Contains(s));
        }

        if (IsActive.HasValue)
        {
            q = q.Where(d => d.IsActive == IsActive.Value);
        }

        Destinations = await q
            .OrderBy(d => d.Country)
            .ThenBy(d => d.Name)
            .ToListAsync();
    }
}