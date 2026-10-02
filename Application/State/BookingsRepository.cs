using Booking.Application.Bookings;
using Booking.Domain.BookingEntity;
using Booking.Domain.Common;

namespace Booking.Application.State;

public sealed class BookingsRepository(IStateContext stateContext, TimeProvider timeProvider) : IBookingsStateHandler
{
  public IEnumerable<BookingEntity> GetBookings(BookingFilters filters)
  {
    ICollection<BookingEntity> allBookings = stateContext.GetAllBookings();
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
        isMatching = isMatching && b.TimeInterval.From >= filters.From;
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

      if (booking is null) return Result<Guid>.Error();

      var resource = stateContext.GetResource(booking.ResourceId);

      if (resource is null) return Result<Guid>.Error();

      if (!resource.ValidateBookingRescheduleWindow(booking.TimeInterval.From, now)) return Result<Guid>.Conflict();

      if (HasConflict(booking.ResourceId, newTimeInterval, now, booking.Id)) return Result<Guid>.Conflict();

      try
      {
        booking.Reschedule(newTimeInterval, now);
        stateContext.SaveBooking(booking);

        return Result<Guid>.Ok(booking.Id);
      } catch (InvalidOperationException)
      {
        return Result<Guid>.Conflict();
      }
    });

  public Task<Result<Guid>> ConfirmBooking(Guid bookingId) =>
    stateContext.ExecuteLocked(() =>
    {
      var booking = GetBooking(bookingId);

      if (booking is null) return Result<Guid>.Error();
      try
      {
        booking.Confirm(timeProvider.GetUtcNow());
        stateContext.SaveBooking(booking);

        return Result<Guid>.Ok(booking.Id);
      }
      catch (InvalidOperationException)
      {
        return Result<Guid>.Conflict();
      }
    });

  public Task<Result<Guid>> CancelBooking(Guid bookingId) =>
    stateContext.ExecuteLocked(() =>
    {
      var booking = GetBooking(bookingId);

      if (booking is null) return Result<Guid>.Error();

      var resource = stateContext.GetResource(booking.ResourceId);

      if (resource is null) return Result<Guid>.Error();

      var now = timeProvider.GetUtcNow();

      if (!resource.ValidateBookingCancellationWindow(booking.TimeInterval.From, now)) return Result<Guid>.Conflict();

      try
      {
        booking.Cancel(now);
        stateContext.SaveBooking(booking);

        return Result<Guid>.Ok(booking.Id);
      }
      catch
      {
        return Result<Guid>.Conflict();
      }
    });

  public BookingEntity? GetBooking(Guid id) =>
    stateContext.GetBooking(id);

  public Task<Result<Guid>> SaveBooking(BookingEntity booking) =>
    stateContext.ExecuteLocked(() =>
    {
      var bookingTimeError = HasConflict(booking.ResourceId, booking.TimeInterval, timeProvider.GetUtcNow());

      if (!bookingTimeError)
      {
        stateContext.SaveBooking(booking);
        return Result<Guid>.Ok(booking.Id);
      } else
      {
        return Result<Guid>.Conflict();
      }
    });

  private bool HasConflict(Guid resourceId, TimeInterval timeInterval, DateTimeOffset now, Guid? excludeBookingId = null) =>
    stateContext.GetAllBookings().Any(b =>
      b.ResourceId == resourceId
      && b.Id != excludeBookingId
      && b.IsActive(now)
      && b.TimeInterval.IsConflictedWith(timeInterval));
}
