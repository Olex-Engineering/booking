using Booking.Domain.Bookings;

namespace Booking.Application.Bookings;

public static class BookingActiveExpression
{
  public static IQueryable<BookingEntity> OnlyActive(this IQueryable<BookingEntity> query)
  {
    return query.Where(b => b.state == BookingStateType.Pending || b.state == BookingStateType.Confirmed);
  }
}
