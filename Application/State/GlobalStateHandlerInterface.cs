using Booking.Application.Bookings;
using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public interface IGlobalStateHandler
{
  public void SaveBooking(BookingEnitity booking);
  public void SaveResource(Resource resource);
  public void SaveUser(User user);
  public void UpdateBooking(BookingEnitity booking);

  public User? GetUser(Guid id);
  public BookingEnitity? GetBooking(Guid id);
  public Resource? GetResource(Guid id);
  public IEnumerable<Resource> GetResources(Guid? userId);
  public IEnumerable<BookingEnitity> GetBookings(BookingFilters filters);
}
