using Booking.Domain.Bookings;

namespace Booking.Application.Bookings;

public static class BookingQueryExtension
{
  public static IQueryable<BookingEntity> OnlyActive(this IQueryable<BookingEntity> query)
  {
    return query.Where(b => BookingEntity.ActiveStates.Contains(b.State));
  }
}
