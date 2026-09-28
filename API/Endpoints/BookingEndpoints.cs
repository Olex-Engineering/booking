namespace Booking.API.Endpoints;

using Booking.API.DTO.Booking;
using Booking.API.Filters;
using Booking.Application.Bookings;
using Booking.Application.State;
using Booking.Domain.BookingEntity;
using Microsoft.AspNetCore.Http.HttpResults;


internal static class ResourceEndpoints
{
  public static IEndpointRouteBuilder MapBookingsEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/booking").AddEndpointFilter(async (context, next) =>
    {
      var endpoint = context.HttpContext.Request.Path;
      var method = context.HttpContext.Request.Method;
      var startTime = DateTime.Now;

      var result = await next(context);

      var endTime = DateTime.Now;
      var requestTime = (endTime - startTime).Milliseconds;

      Console.WriteLine($"Request data: method {method}, endpoint: {endpoint}, time: {requestTime}");
      return result;
    }).WithTags("Bookings");

    group.MapPost("/", CreateBooking)
      .AddEndpointFilter<BookingCreateValidationFilter>()
      .WithName("Create booking");
    group.MapPatch("/cancel", CancelBooking).WithName("Cancel booking");
    group.MapGet("/{id}", GetBooking).WithName("Get booking by id");
    group.MapGet("/list", GetList).WithName("Get booking list");

    return app;
  }

  private static async Task<Results<Created<CreateBookingResponse>,  BadRequest>>  CreateBooking(CreateBookingRequest request, IGlobalStateHandler globalStateHandler)
  {
    try
    {
      BookingEnitity booking = new(request.ResourceId, request.UserId, request.From, request.To);

       globalStateHandler.SaveBooking(booking);

      return await Task.FromResult(TypedResults.Created("/bookings", new CreateBookingResponse(booking.Id)));
    } catch (ArgumentOutOfRangeException)
    {
      return TypedResults.BadRequest();
    }
  }

  private static async Task<Results<Ok<BookingDto>, NotFound>> GetBooking(Guid id, IGlobalStateHandler globalStateHandler)
  {
    var booking = globalStateHandler.GetBooking(id);

    if (booking is null)
    {
      return TypedResults.NotFound();
    }

    return TypedResults.Ok(BookingDto.FromEntity(booking));
  }

  private static async Task<Ok<IEnumerable<BookingDto>>> GetList([AsParameters] BookingFilters filters, IGlobalStateHandler globalStateHandler)
  {
    var bookings = globalStateHandler.GetBookings(filters);
    var bookingsDto = bookings.Select(BookingDto.FromEntity);

    return TypedResults.Ok(bookingsDto);
  }

  private static async Task<Results<NoContent, NotFound>> CancelBooking(BookingCancelRequeset request, IGlobalStateHandler globalStateHandler)
  {
    var booking = globalStateHandler.GetBooking(request.Id);

    if (booking is null) return TypedResults.NotFound();

    booking.CancelBooking();

    globalStateHandler.UpdateBooking(booking);

    return TypedResults.NoContent();
  }
}
