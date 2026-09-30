using Booking.Domain.BookingEntity;

namespace Booking.Application.Bookings;

public sealed record BookingDto(
  Guid Id,
  Guid ResourceId,
  Guid UserId,
  DateTimeOffset From,
  DateTimeOffset To,
  bool IsCompleted,
  bool IsCanceled
)
{
  public static BookingDto FromEntity(BookingEntity b) =>
    new(b.Id, b.ResourceId, b.UserId, b.TimeInterval.From, b.TimeInterval.To, b.IsCompleted, b.IsCanceled);
}
