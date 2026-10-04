using Booking.Domain.Common;

namespace Booking.Domain.Bookings;

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
    BookingStateType.Pending when now > TimeInterval.From => BookingStateType.Expired,
    _ => _state
  };

  public bool IsActive(DateTimeOffset now) =>
    GetState(now) is BookingStateType.Pending or BookingStateType.Confirmed;

  public bool IsPending(DateTimeOffset now) => GetState(now) == BookingStateType.Pending;

  public void Cancel(DateTimeOffset now, int cancellationWindowInHours)
  {
    if (!IsTimeWindowValid(now, cancellationWindowInHours)) throw new InvalidOperationException("Cannot cancel a booking that is not within the cancellation window.");

    if (!IsActive(now)) throw new InvalidOperationException("Only a pending or confirmed booking can be canceled.");

    _state = BookingStateType.Canceled;
  }

  public void Confirm(DateTimeOffset now)
  {
    if (!IsPending(now)) throw new InvalidOperationException("Cannot confirm a booking that is not pending.");

    _state = BookingStateType.Confirmed;
  }

  public void Reschedule(TimeInterval newTimeInterval, DateTimeOffset now, int rescheduleWindowInHours)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(now, newTimeInterval.From);

    if (!IsTimeWindowValid(now, rescheduleWindowInHours)) throw new InvalidOperationException("Cannot reschedule a booking that is not within the reschedule window.");

    if (!IsActive(now)) throw new InvalidOperationException("Cannot reschedule a booking that is not active.");

    TimeInterval = newTimeInterval;
    _state = BookingStateType.Pending;
  }

  public static BookingEntity Create(Guid resourceId, Guid userId, TimeInterval timeInterval, DateTimeOffset now)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(now, timeInterval.From);

    return new BookingEntity(resourceId, userId, timeInterval);
  }

  private bool IsTimeWindowValid(DateTimeOffset now, int windowInHours) =>
    TimeInterval.From >= now.AddHours(windowInHours);
}
