namespace Booking.API.DTO.Booking;

public sealed record BookingRescheduleRequest(Guid Id, DateTimeOffset From, DateTimeOffset To);
