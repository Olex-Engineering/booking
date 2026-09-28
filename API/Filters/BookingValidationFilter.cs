using Booking.API.DTO.Booking;
using Booking.Application.Bookings;
using Booking.Application.State;

namespace Booking.API.Filters;

public class BookingCreateValidationFilter(IGlobalStateHandler globalStateHandler) : IEndpointFilter
{
  private static SemaphoreSlim _lock = new(1, 1);

  public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
  {
    await _lock.WaitAsync();
    try
    {
      CreateBookingRequest request = context.GetArgument<CreateBookingRequest>(0);
      BookingFilters filters = new(ResourceId: request.ResourceId);

      var otherBookings = globalStateHandler.GetBookings(filters);

      var bookingTimeError = otherBookings.Any(b =>
      {
        var isValid = request.To <= b.From || request.From >= b.To || b.IsCanceled;
        return !isValid;
      });

      if (bookingTimeError) return TypedResults.Conflict();

      return await next(context);
    }
    finally
    {
        _lock.Release();
    }

    
  }
}
