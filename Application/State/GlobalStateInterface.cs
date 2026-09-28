using System.Collections.Concurrent;
using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public interface IGlobalState
{
  public ConcurrentDictionary<Guid, Resource> Resources { get; }
  public ConcurrentDictionary<Guid, User> Users { get; }
  public ConcurrentDictionary<Guid, BookingEnitity> Bookings { get; }

}
