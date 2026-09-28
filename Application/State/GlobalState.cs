using System.Collections.Concurrent;
using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;
using Booking.Domain.User;

namespace Booking.Application.State;

public sealed record GlobalState() : IGlobalState
{
  public ConcurrentDictionary<Guid, Resource> Resources { get; } = new();

  public ConcurrentDictionary<Guid, User> Users { get; } = new();

  public ConcurrentDictionary<Guid, BookingEnitity> Bookings { get; } = new();
}
