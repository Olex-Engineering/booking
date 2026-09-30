using Booking.Application.Bookings;
using Booking.Domain.BookingEntity;
using Booking.Domain.Common;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public sealed class GlobalStateHandler : IGlobalStateHandler
{
  private readonly SemaphoreSlim _lock = new(1, 1);

  private readonly GlobalState GlobalState = new();

  public IEnumerable<BookingEntity> GetBookings(BookingFilters filters)
  {
    IEnumerable<BookingEntity> allBookings = GlobalState.Bookings.Values ?? [];

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
        isMatching = isMatching && b.TimeInterval.From >= filters.From;
      }

      if (filters.To is not null)
      {
        isMatching = isMatching && b.TimeInterval.From <= filters.To;
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

  public BookingEntity? GetBooking(Guid id) =>
    GlobalState.Bookings.TryGetValue(id, out var _booking) ? _booking : null;
  public Resource? GetResource(Guid id)
  {
    var resource = GlobalState.Resources.TryGetValue(id, out var _resource) ? _resource : null;

    if (resource is null) return null;

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
  public User? GetUser(Guid id)
  {
    Console.WriteLine(GlobalState.Users.Count);

    return GlobalState.Users.TryGetValue(id, out var user) ? user : null;
  }

  public async Task<Result<Guid>> SaveBooking(BookingEntity booking)
  {
    await _lock.WaitAsync();
    try
    {
      BookingFilters filters = new(ResourceId: booking.ResourceId);

      var otherBookings = GetBookings(filters);

      var bookingTimeError = otherBookings.Any(b =>
       booking.TimeInterval.IsConflictedWith(b.TimeInterval) && !b.IsCanceled
      );

      if (!bookingTimeError)
      {
        GlobalState.Bookings.TryAdd(booking.Id, booking);
        return Result<Guid>.Ok(booking.Id);
      } else
      {
        return Result<Guid>.Conflict();
      }
    }
    finally
    {
        _lock.Release();
    }
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
