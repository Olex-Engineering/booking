using Booking.Domain.Common;

namespace Booking.Domain.Bookings;

public sealed class BookingEntity
{
  public static readonly BookingStateType[] ActiveStates = [BookingStateType.Pending, BookingStateType.Confirmed];

  public BookingStateType State { get; private set;} = BookingStateType.Pending;

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

  // TODO: Remove after bg service is implemented.
  public bool IsActive(DateTimeOffset now) => IsPending(now) || State == BookingStateType.Confirmed;

  public bool IsPending(DateTimeOffset now) => State == BookingStateType.Pending && TimeInterval.From > now;

  public void Cancel(DateTimeOffset now, int cancellationWindowInHours)
  {
    if (!IsActive(now)) throw new InvalidOperationException("Only a pending or confirmed booking can be canceled.");
    
    if (!IsTimeWindowValid(now, cancellationWindowInHours)) throw new InvalidOperationException("Cannot cancel a booking that is not within the cancellation window.");

    State = BookingStateType.Canceled;
  }

  public void Confirm(DateTimeOffset now)
  {
    if (!IsPending(now)) throw new InvalidOperationException("Cannot confirm a booking that is not pending.");

    State = BookingStateType.Confirmed;
  }

  public void Reschedule(TimeInterval newTimeInterval, DateTimeOffset now, int rescheduleWindowInHours)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(now, newTimeInterval.From);

    if (!IsActive(now)) throw new InvalidOperationException("Cannot reschedule a booking that is not active.");

    if (!IsTimeWindowValid(now, rescheduleWindowInHours)) throw new InvalidOperationException("Cannot reschedule a booking that is not within the reschedule window.");

    TimeInterval = newTimeInterval;
    State = BookingStateType.Pending;
  }

  public static BookingEntity Create(Guid resourceId, Guid userId, TimeInterval timeInterval, DateTimeOffset now)
  {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(now, timeInterval.From);

    return new BookingEntity(resourceId, userId, timeInterval);
  }

  private bool IsTimeWindowValid(DateTimeOffset now, int windowInHours) =>
    TimeInterval.From >= now.AddHours(windowInHours);
}
