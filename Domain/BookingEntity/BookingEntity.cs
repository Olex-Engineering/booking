using Booking.Domain.Common;

namespace Booking.Domain.BookingEntity;

public sealed class BookingEntity(
  Guid resourceId, Guid userId, TimeInterval timeInterval
  )
{
  public const int MaxBookingDays = 30;
  
  public Guid Id { get; } = Guid.CreateVersion7();
  public Guid ResourceId { get; } = resourceId;
  public Guid UserId { get; } = userId;
  public TimeInterval TimeInterval { get; } = timeInterval;
  public bool IsCanceled { get; private set; } = false;

  public bool IsCompleted => DateTimeOffset.UtcNow > TimeInterval.To && !IsCanceled;

  public Result<Guid> CancelBooking()
  {
    if (IsCompleted) return Result<Guid>.Conflict();

    IsCanceled = true;
    return Result<Guid>.Ok(Id);
  }
}

