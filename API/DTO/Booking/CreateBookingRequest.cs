namespace Booking.API.DTO.Booking;

public sealed record CreateBookingRequest(Guid ResourceId, Guid UserId, DateTimeOffset From, DateTimeOffset To);
