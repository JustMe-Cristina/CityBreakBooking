using CityBreakBooking.Web.Models;

namespace CityBreakBooking.Web.Data;

public static class SeedData
{
    public static void EnsureSeeded(AppDbContext db)
    {
        if (db.Destinations.Any()) return;

        var paris = new Destination { Name = "Paris", Country = "France", Description = "Classic city-break", IsActive = true };
        var rome  = new Destination { Name = "Rome", Country = "Italy", Description = "History & food", IsActive = true };
        var barca = new Destination { Name = "Barcelona", Country = "Spain", Description = "Sea & culture", IsActive = true };

        db.Destinations.AddRange(paris, rome, barca);
        db.SaveChanges();

        db.Trips.AddRange(
            new Trip { DestinationId = paris.Id, Title = "Paris Essentials", StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(13), PricePerPerson = 299, MaxSeats = 20, IsActive = true },
            new Trip { DestinationId = rome.Id,  Title = "Rome Weekend",     StartDate = DateTime.Today.AddDays(20), EndDate = DateTime.Today.AddDays(23), PricePerPerson = 259, MaxSeats = 18, IsActive = true },
            new Trip { DestinationId = barca.Id, Title = "Barcelona Escape", StartDate = DateTime.Today.AddDays(30), EndDate = DateTime.Today.AddDays(34), PricePerPerson = 349, MaxSeats = 25, IsActive = true }
        );

        db.SaveChanges();
    }
}