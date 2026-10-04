using Booking.Domain.Bookings;
using Booking.Domain.Resources;
using Booking.Domain.Users;

namespace Booking.Application.State;

public interface IStateContext
{
  IEnumerable<BookingEntity> GetAllBookings();
  BookingEntity? GetBooking(Guid id);
  Resource? GetResource(Guid id);
  IEnumerable<Resource> GetResources();
  User? GetUser(Guid id);
  void SaveBooking(BookingEntity booking);
  void SaveResource(Resource resource);
  void SaveUser(User user);
  Task<T> ExecuteLocked<T>(Func<T> action);
}
