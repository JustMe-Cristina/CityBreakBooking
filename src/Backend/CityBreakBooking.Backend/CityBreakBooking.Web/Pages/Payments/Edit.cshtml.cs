using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Pages.Payments;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db) => _db = db;

    [BindProperty]
    public Payment Payment { get; set; } = null!;

    public List<SelectListItem> StatusOptions { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (Payment is null) return NotFound();

        LoadStatusOptions();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadStatusOptions();

        if (!ModelState.IsValid)
            return Page();

        var existing = await _db.Payments.FirstOrDefaultAsync(p => p.Id == Payment.Id);
        if (existing is null) return NotFound();

        existing.Amount = Payment.Amount;
        existing.PaymentDate = Payment.PaymentDate;
        existing.Status = Payment.Status;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Payment updated.";
        return RedirectToPage("./Index");
    }

    private void LoadStatusOptions()
    {
        StatusOptions = Enum.GetValues<PaymentStatus>()
            .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() })
            .ToList();
    }
}