namespace Booking.Application.Bookings;

public sealed record BookingFilters(
  DateTimeOffset? From = null,
  DateTimeOffset? To = null,
  bool? IsCompleted = null,
  bool? IsCanceled = null,
  Guid? ResourceId = null,
  Guid? UserId = null
);
