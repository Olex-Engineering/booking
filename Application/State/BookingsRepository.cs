using Booking.Application.Bookings;
using Booking.Domain.Bookings;
using Booking.Domain.Common;

namespace Booking.Application.State;

public sealed class BookingsRepository(IStateContext stateContext, TimeProvider timeProvider) : IBookingsRepository
{
  public IEnumerable<BookingEntity> GetBookings(BookingFilters filters)
  {
    IEnumerable<BookingEntity> allBookings = stateContext.GetAllBookings();
    var now = timeProvider.GetUtcNow();

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
        isMatching = isMatching && b.TimeInterval.To >= filters.From;
      } 

      if (filters.To is not null)
      {
        isMatching = isMatching && b.TimeInterval.From <= filters.To;
      }

      if (filters.InStateType is { Length: > 0 })
      {
        isMatching = isMatching && filters.InStateType.Contains(b.GetState(now));
      }

      return isMatching;
    });
  }

  public Task<Result<Guid>> RescheduleBooking(Guid bookingId, TimeInterval newTimeInterval) =>
    stateContext.ExecuteLocked(() =>
    {
      var now = timeProvider.GetUtcNow();
      var booking = GetBooking(bookingId);

      if (booking is null) return Result<Guid>.NotFound("Booking not found.");

      var resource = stateContext.GetResource(booking.ResourceId);

      if (resource is null) return Result<Guid>.NotFound("Booking resource not found.");

      if (HasConflict(booking.ResourceId, newTimeInterval, now, booking.Id)) return Result<Guid>.Conflict("Booking period conflict (from, to) with other bookings.");

      try
      {
        booking.Reschedule(newTimeInterval, now, resource.RescheduleWindowInHours);

        return Result<Guid>.Ok(booking.Id);
      } catch (InvalidOperationException ex)
      {
        return Result<Guid>.Conflict(ex.Message);
      }
    });

  public Task<Result<Guid>> ConfirmBooking(Guid bookingId) =>
    stateContext.ExecuteLocked(() =>
    {
      var now = timeProvider.GetUtcNow();
      var booking = GetBooking(bookingId);

      if (booking is null) return Result<Guid>.NotFound("Booking not found.");

      if (!booking.IsPending(now)) return Result<Guid>.Conflict("Only a pending booking can be confirmed.");

      booking.Confirm(now);

      return Result<Guid>.Ok(booking.Id);
    });

  public Task<Result<Guid>> CancelBooking(Guid bookingId) =>
    stateContext.ExecuteLocked(() =>
    {
      var now = timeProvider.GetUtcNow();
      var booking = GetBooking(bookingId);

      if (booking is null) return Result<Guid>.NotFound("Booking not found.");

      var resource = stateContext.GetResource(booking.ResourceId);

      if (resource is null) return Result<Guid>.NotFound("Booking resource not found.");

      try
      {
        booking.Cancel(now, resource.CancellationWindowInHours);
        return Result<Guid>.Ok(booking.Id);
      }
      catch (InvalidOperationException ex)
      {
        return Result<Guid>.Conflict(ex.Message);
      }
    });

  public BookingEntity? GetBooking(Guid id) =>
    stateContext.GetBooking(id);

  public Task<Result<Guid>> SaveBooking(BookingEntity booking) =>
    stateContext.ExecuteLocked(() =>
    {
      if (HasConflict(booking.ResourceId, booking.TimeInterval, timeProvider.GetUtcNow()))
      {
        return Result<Guid>.Conflict("Booking period conflict (from, to) with other bookings.");
      }

      stateContext.SaveBooking(booking);

      return Result<Guid>.Ok(booking.Id);
    });

  private bool HasConflict(Guid resourceId, TimeInterval timeInterval, DateTimeOffset now, Guid? excludeBookingId = null) =>
    stateContext.GetAllBookings().Any(b =>
      b.ResourceId == resourceId
      && b.Id != excludeBookingId
      && b.IsActive(now)
      && b.TimeInterval.IsConflictedWith(timeInterval));
}
