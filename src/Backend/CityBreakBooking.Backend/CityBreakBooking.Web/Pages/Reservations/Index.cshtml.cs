using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reservations;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public IList<Reservation> Reservations { get; private set; } = new List<Reservation>();

    [BindProperty(SupportsGet = true)]
    public ReservationStatus? Status { get; set; } // null = all

    [BindProperty(SupportsGet = true)]
    public int? TripId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? UserSearch { get; set; } // contains UserId

    public List<SelectListItem> StatusOptions { get; private set; } = new();
    public List<SelectListItem> TripOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        StatusOptions = new List<SelectListItem>
        {
            new() { Value = "", Text = "All" },
            new() { Value = ReservationStatus.Pending.ToString(), Text = "Pending" },
            new() { Value = ReservationStatus.Confirmed.ToString(), Text = "Confirmed" },
            new() { Value = ReservationStatus.Cancelled.ToString(), Text = "Cancelled" }
        };

        TripOptions = await _db.Trips
            .Include(t => t.Destination)
            .OrderByDescending(t => t.StartDate)
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{t.Title} - {t.Destination!.Name}"
            })
            .ToListAsync();

        IQueryable<Reservation> query = _db.Reservations
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .AsNoTracking()
            .OrderByDescending(r => r.ReservationDate);

        if (Status.HasValue)
            query = query.Where(r => r.Status == Status.Value);

        if (TripId.HasValue)
            query = query.Where(r => r.TripId == TripId.Value);

        if (!string.IsNullOrWhiteSpace(UserSearch))
            query = query.Where(r => r.UserEmail.Contains(UserSearch));

        Reservations = await query.ToListAsync();
    }
}