using Booking.Domain.Common;

namespace Booking.Domain.BookingEntity;

public sealed class BookingEntity
{
  private BookingStateType _state = BookingStateType.Pending;

  public Guid Id { get; } = Guid.CreateVersion7();
  public Guid ResourceId { get; }
  public Guid UserId { get; }
  public TimeInterval TimeInterval { get; private set; }

  private BookingEntity(Guid resourceId, Guid userId, TimeInterval timeInterval)
  {
    ResourceId = resourceId;
    UserId = userId;
    TimeInterval = timeInterval;
  }

  public BookingStateType GetState(DateTimeOffset now) => _state switch
  {
    BookingStateType.Confirmed when now > TimeInterval.To => BookingStateType.Completed,
    BookingStateType.Pending when now > TimeInterval.To => BookingStateType.Expired,
    _ => _state
  };

  public bool IsActive(DateTimeOffset now) =>
    GetState(now) is BookingStateType.Pending or BookingStateType.Confirmed;

  public void Cancel(DateTimeOffset now)
  {
    if (!IsActive(now)) throw new InvalidOperationException("Cannot cancel a booking that is not active.");

    _state = BookingStateType.Canceled;
  }

  public void Confirm(DateTimeOffset now)
  {
    if (GetState(now) != BookingStateType.Pending) throw new InvalidOperationException("Cannot confirm a booking that is not pending.");

    _state = BookingStateType.Confirmed;
  }

  public void Reschedule(TimeInterval newTimeInterval, DateTimeOffset now)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(now, newTimeInterval.From);

    if (!IsActive(now)) throw new InvalidOperationException("Cannot reschedule a booking that is not active.");

    TimeInterval = newTimeInterval;
  }

  public static BookingEntity Create(Guid resourceId, Guid userId, TimeInterval timeInterval, DateTimeOffset now)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(now, timeInterval.From);

    return new BookingEntity(resourceId, userId, timeInterval);
  }
}
