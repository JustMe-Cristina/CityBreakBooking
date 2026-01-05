using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Destinations;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Destination Destination { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var dest = await _db.Destinations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id.Value);
        if (dest is null) return NotFound();

        Destination = dest;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null) return NotFound();

        var dest = await _db.Destinations.FindAsync(id.Value);
        if (dest is null) return NotFound();

        _db.Destinations.Remove(dest);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}