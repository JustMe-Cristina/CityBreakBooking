using CityBreakBooking.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CityBreakBooking.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext(options)
{
    public DbSet<Destination> Destinations { get; set; } = default!;
    public DbSet<Trip> Trips { get; set; } = default!;
    public DbSet<Reservation> Reservations { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    public DbSet<Payment> Payments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.ReservationId)
            .IsUnique();
    }
}