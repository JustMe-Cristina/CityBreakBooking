using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Trips;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Trip> Trips { get; private set; } = new List<Trip>();

    // Filters (bind from query string)
    [BindProperty(SupportsGet = true)]
    public int? DestinationId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; } // null = all

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; } // title contains

    public List<SelectListItem> DestinationOptions { get; private set; } = new();
    public List<SelectListItem> ActiveOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        DestinationOptions = await _db.Destinations
            .OrderBy(d => d.Name)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Name} ({d.Country})"
            })
            .ToListAsync();

        ActiveOptions = new List<SelectListItem>
        {
            new() { Value = "", Text = "All" },
            new() { Value = "true", Text = "Active" },
            new() { Value = "false", Text = "Inactive" }
        };

        IQueryable<Trip> query = _db.Trips
            .Include(t => t.Destination)
            .AsNoTracking()
            .OrderByDescending(t => t.StartDate);

        if (DestinationId.HasValue)
            query = query.Where(t => t.DestinationId == DestinationId.Value);

        if (IsActive.HasValue)
            query = query.Where(t => t.IsActive == IsActive.Value);

        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(t => t.Title.Contains(Search));

        Trips = await query.ToListAsync();
    }
}