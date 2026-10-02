using Booking.Domain.BookingEntity;

namespace Booking.Application.Bookings;

public sealed record BookingDto(
  Guid Id,
  Guid ResourceId,
  Guid UserId,
  DateTimeOffset From,
  DateTimeOffset To,
  BookingStateType StateType,
  bool IsCanceled
)
{
  public static BookingDto FromEntity(BookingEntity b, DateTimeOffset now)
  {
    var state = b.GetState(now);
    var timeInterval = b.TimeInterval;

    return new(b.Id, b.ResourceId, b.UserId, timeInterval.From, timeInterval.To, state, state == BookingStateType.Canceled);
  }
}
