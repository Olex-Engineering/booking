using Booking.Application.Bookings;
using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public sealed class GlobalStateHandler : IGlobalStateHandler
{
  private readonly GlobalState GlobalState = new();

  public IEnumerable<BookingEnitity> GetBookings(BookingFilters filters)
  {
    IEnumerable<BookingEnitity> allBookings = GlobalState.Bookings.Values ?? [];

    return allBookings.Where(b =>
    {
      bool isMatching = true;

      if (filters.ResourceId is not null)
      {
        isMatching = isMatching && b.ResourceId == filters.ResourceId;
      }

      if (filters.UserId is not null)
      {
        isMatching = isMatching && b.UserId == filters.UserId;
      }

      if (filters.From is not null)
      {
        isMatching = isMatching && b.From > filters.From;
      }

      if (filters.To is not null)
      {
        isMatching = isMatching && b.To < filters.To;
      }

      if (filters.IsCanceled is not null)
      {
        isMatching = isMatching && b.IsCanceled == filters.IsCanceled;
      }

      if (filters.IsCompleted is not null)
      {
        isMatching = isMatching && b.IsCompleted == filters.IsCompleted;
      }

      return isMatching;
    });
  }

  public BookingEnitity? GetBooking(Guid id) =>
    GlobalState.Bookings.TryGetValue(id, out var _booking) ? _booking : null;
  public Resource? GetResource(Guid id)
  {
    var resource = GlobalState.Resources.TryGetValue(id, out var _resource) ? _resource : null;

    if (resource is null) return null;

    var allBookings = GlobalState.Bookings.Values;

    if (allBookings is null) return resource;

    resource.Bookings = [.. allBookings.Where(booking => booking.ResourceId == resource.Id)];

    return resource;
  }

  public IEnumerable<Resource> GetResources(Guid? userId)
  {
    IEnumerable<Resource> resources = GlobalState.Resources.Values ?? [];

    if (userId is not null)
    {
      resources = resources.Where(r => r.UserId == userId);
    }

    return resources;
  }

  public void UpdateBooking(BookingEnitity entity)
  {
    GlobalState.Bookings.AddOrUpdate(entity.Id, entity, (key, old) => entity);
  }

  public User? GetUser(Guid id)
  {
    Console.WriteLine(GlobalState.Users.Count);

    return GlobalState.Users.TryGetValue(id, out var user) ? user : null;
  }

  public void SaveBooking(BookingEnitity booking)
  {
    GlobalState.Bookings.TryAdd(booking.Id, booking);
  }

  public void SaveResource(Resource resource)
  {
    GlobalState.Resources.TryAdd(resource.Id, resource);
  }

  public void SaveUser(User user)
  {

    var isSuccess = GlobalState.Users.TryAdd(user.Id, user);

    Console.WriteLine($"Save user, id: {user.Id}, name: {user.Name}, isSuccess: {isSuccess}");
  }

}
