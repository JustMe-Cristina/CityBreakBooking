using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBreakBooking.Web.Pages.Destinations;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Destination Destination { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        _db.Destinations.Add(Destination);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Destination created successfully.";
        return RedirectToPage("./Index");
    }
}