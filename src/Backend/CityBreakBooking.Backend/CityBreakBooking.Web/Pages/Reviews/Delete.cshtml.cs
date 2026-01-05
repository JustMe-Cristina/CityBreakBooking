using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Reviews;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    public DeleteModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Review Review { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();

        var review = await _db.Reviews
            .Include(r => r.Trip)
            .ThenInclude(t => t!.Destination)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id.Value);

        if (review is null) return NotFound();

        Review = review;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null) return NotFound();

        var review = await _db.Reviews.FindAsync(id.Value);
        if (review is null) return NotFound();

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}