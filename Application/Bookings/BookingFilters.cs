using Booking.Domain.BookingEntity;

namespace Booking.Application.Bookings;

public sealed record BookingFilters(
  DateTimeOffset? From = null,
  DateTimeOffset? To = null,
  BookingStateType[]? InStateType = null,
  Guid? ResourceId = null,
  Guid? UserId = null
);
