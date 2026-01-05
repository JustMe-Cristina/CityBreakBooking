using Microsoft.EntityFrameworkCore;
using CityBreakBooking.Web.Models;

namespace CityBreakBooking.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
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