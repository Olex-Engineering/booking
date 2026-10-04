using Booking.Application.Bookings;
using Booking.Domain.Bookings;
using Booking.Domain.Common;

namespace Booking.Application.State;

public interface IBookingsRepository
{
  public Task<Result<Guid>> SaveBooking(BookingEntity booking);
  public BookingEntity? GetBooking(Guid id);
  public IEnumerable<BookingEntity> GetBookings(BookingFilters filters);
  public Task<Result<Guid>> RescheduleBooking(Guid bookingId, TimeInterval newTimeInterval);
  public  Task<Result<Guid>> ConfirmBooking(Guid bookingId);
  public Task<Result<Guid>> CancelBooking(Guid bookingId);
}
