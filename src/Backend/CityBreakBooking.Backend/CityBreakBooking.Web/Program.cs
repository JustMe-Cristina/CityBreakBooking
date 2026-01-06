using CityBreakBooking.Web.Data;
using CityBreakBooking.Web.Models;
using CityBreakBooking.Web.Models.Dto;
using CityBreakBooking.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// DbContext (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (Roles)
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Seed roles + admin user
await IdentitySeed.SeedAsync(app.Services);

// -------- API endpoints for MAUI --------

// POST /api/reservations
// Body: { "tripId": 3, "userEmail": "x@y.com", "numberOfPersons": 2 }

app.MapPost("/api/reservations", async (CreateReservationRequest req, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.UserEmail))
        return Results.BadRequest("UserEmail is required.");

    if (req.NumberOfPersons < 1 || req.NumberOfPersons > 10)
        return Results.BadRequest("NumberOfPersons must be between 1 and 10.");

    var trip = await db.Trips.FirstOrDefaultAsync(t => t.Id == req.TripId && t.IsActive);
    if (trip is null) return Results.NotFound("Trip not found.");

    // capacity check pe confirmed (NU pe pending)
    var confirmed = await db.Reservations
        .Where(r => r.TripId == req.TripId && r.Status == ReservationStatus.Confirmed)
        .SumAsync(r => (int?)r.NumberOfPersons) ?? 0;

    var available = trip.MaxSeats - confirmed;
    if (req.NumberOfPersons > available)
        return Results.BadRequest($"Not enough seats. Remaining: {available}");

    var reservation = new Reservation
    {
        TripId = req.TripId,
        UserEmail = req.UserEmail.Trim(),
        NumberOfPersons = req.NumberOfPersons,
        Status = ReservationStatus.Pending,
        ReservationDate = DateTime.Now
    };

    db.Reservations.Add(reservation);
    await db.SaveChangesAsync();

    return Results.Created($"/Reservations/Details?id={reservation.Id}", new
    {
        reservation.Id,
        reservation.Status,
        reservation.TripId,
        reservation.UserEmail
    });
});

// (Optional) GET /api/trips (pentru mobile)
app.MapGet("/api/trips", async (AppDbContext db) =>
{
    var trips = await db.Trips
        .Include(t => t.Destination)
        .Where(t => t.IsActive)
        .OrderBy(t => t.StartDate)
        .Select(t => new
        {
            t.Id,
            t.Title,
            Destination = t.Destination!.Name,
            t.StartDate,
            t.EndDate,
            t.PricePerPerson,
            t.MaxSeats
        })
        .ToListAsync();

    return Results.Ok(trips);
});

// Razor Pages routes
app.MapRazorPages();


app.Run();