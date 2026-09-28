using Booking.Domain.Common;

namespace Booking.Domain.BookingEntity;

public sealed class BookingEntity
{
  public const int MaxBookingDays = 30;
  
  public Guid Id { get; } = Guid.CreateVersion7();
  public Guid ResourceId { get; }
  public Guid UserId { get; }
  public DateTimeOffset From { get; }
  public DateTimeOffset To { get; }
  public bool IsCanceled { get; private set; } = false;

  public bool IsCompleted => DateTimeOffset.UtcNow > To && !IsCanceled;

  public BookingEntity(
    Guid resourceId, Guid userId, DateTimeOffset from, DateTimeOffset to
  )
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(DateTimeOffset.UtcNow, from);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);

    ResourceId = resourceId;
    UserId = userId;
    From = from;
    To = to;
  }

  public Result<Guid> CancelBooking()
  {
    if (IsCompleted) return new Result<Guid>.Conflict();

    IsCanceled = true;
    return new Result<Guid>.Ok(Id);
  }
}

