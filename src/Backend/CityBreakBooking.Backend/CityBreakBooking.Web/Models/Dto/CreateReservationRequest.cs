namespace CityBreakBooking.Web.Models.Dto;

public record CreateReservationRequest(
    int TripId,
    string UserEmail,
    int NumberOfPersons
);