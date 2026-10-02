using System.Collections.Concurrent;
using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public sealed class GlobalState() : IStateContext
{
  private ConcurrentDictionary<Guid, Resource> Resources { get; } = new();

  private ConcurrentDictionary<Guid, User> Users { get; } = new();

  private ConcurrentDictionary<Guid, BookingEntity> Bookings { get; } = new();

  private readonly SemaphoreSlim _lock = new(1, 1);

  public async Task<T> ExecuteLocked<T>(Func<T> action)
  {
    await _lock.WaitAsync();
    try
    {
      return action();
    }
    finally
    {
      _lock.Release();
    }
  }

  public ICollection<BookingEntity> GetAllBookings()
  {
    return Bookings.Values;
  }

  public BookingEntity? GetBooking(Guid id) =>
    Bookings.TryGetValue(id, out var _booking) ? _booking : null;

  public Resource? GetResource(Guid id)
    => Resources.TryGetValue(id, out var _resource) ? _resource : null;

  public IEnumerable<Resource> GetResources() => Resources.Values;

  public User? GetUser(Guid id) => Users.TryGetValue(id, out var user) ? user : null;

  public void SaveBooking(BookingEntity booking)
  {
    Bookings.TryAdd(booking.Id, booking);
  }

  public void SaveResource(Resource resource)
  {
    Resources.TryAdd(resource.Id, resource);
  }

  public void SaveUser(User user)
  {
    Users.TryAdd(user.Id, user);
  }
}
