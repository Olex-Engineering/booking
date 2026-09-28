using Booking.Application.Bookings;
using Booking.Domain.BookingEntity;
using Booking.Domain.Common;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public interface IGlobalStateHandler
{
  public Task<Result<Guid>> SaveBooking(BookingEntity booking);
  public void SaveResource(Resource resource);
  public void SaveUser(User user);
  public User? GetUser(Guid id);
  public BookingEntity? GetBooking(Guid id);
  public Resource? GetResource(Guid id);
  public IEnumerable<Resource> GetResources(Guid? userId);
  public IEnumerable<BookingEntity> GetBookings(BookingFilters filters);
}
