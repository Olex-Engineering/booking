using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public interface IStateContext
{
  ICollection<BookingEntity> GetAllBookings();
  BookingEntity? GetBooking(Guid id);
  Resource? GetResource(Guid id);
  IEnumerable<Resource> GetResources();
  User? GetUser(Guid id);
  void SaveBooking(BookingEntity booking);
  void SaveResource(Resource resource);
  void SaveUser(User user);
  Task<T> ExecuteLocked<T>(Func<T> action);
}
