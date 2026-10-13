using Booking.Domain.Bookings;

namespace Booking.Application.Bookings;

// Expired and Completed are not stored, so the rules of BookingEntity.GetState are repeated here
// over stored columns only: entity methods cannot be translated to a db query.
public static class BookingQueryExtensions
{
  public static IQueryable<BookingEntity> OnlyActive(this IQueryable<BookingEntity> query, DateTimeOffset now)
  {
    return query.Where(b =>
      (b.State == BookingStateType.Pending && b.TimeInterval.From >= now)
      || (b.State == BookingStateType.Confirmed && b.TimeInterval.To >= now));
  }

  public static IQueryable<BookingEntity> InStates(this IQueryable<BookingEntity> query, BookingStateType[] states, DateTimeOffset now)
  {
    var pending = states.Contains(BookingStateType.Pending);
    var confirmed = states.Contains(BookingStateType.Confirmed);
    var canceled = states.Contains(BookingStateType.Canceled);
    var expired = states.Contains(BookingStateType.Expired);
    var completed = states.Contains(BookingStateType.Completed);

    return query.Where(b =>
      (pending && b.State == BookingStateType.Pending && b.TimeInterval.From >= now)
      || (confirmed && b.State == BookingStateType.Confirmed && b.TimeInterval.To >= now)
      || (canceled && b.State == BookingStateType.Canceled)
      || (expired && (b.State == BookingStateType.Expired || (b.State == BookingStateType.Pending && b.TimeInterval.From < now)))
      || (completed && (b.State == BookingStateType.Completed || (b.State == BookingStateType.Confirmed && b.TimeInterval.To < now))));
  }
}
