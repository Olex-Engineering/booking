namespace Booking.API.Endpoints;

using System.Diagnostics;
using Booking.API.DTO.Booking;
using Booking.API.Filters;
using Booking.Application.Bookings;
using Booking.Application.State;
using Booking.Domain.BookingEntity;
using Microsoft.AspNetCore.Http.HttpResults;


internal static class BookingEndpoints
{
  public static IEndpointRouteBuilder MapBookingsEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/bookings").AddEndpointFilter(async (context, next) =>
    {
      var endpoint = context.HttpContext.Request.Path;
      var method = context.HttpContext.Request.Method;
      var watch = new Stopwatch();
      watch.Start();

      var result = await next(context);

      watch.Stop();
      var requestTime = watch.ElapsedMilliseconds;

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

  private static async Task<Results<Created<CreateBookingResponse>,  BadRequest, Conflict>>  CreateBooking(CreateBookingRequest request, IGlobalStateHandler globalStateHandler)
  {
    try
    {
      BookingEntity booking = new(request.ResourceId, request.UserId, request.From, request.To);

       globalStateHandler.SaveBooking(booking);

      return await Task.FromResult(TypedResults.Created("/api/bookings", new CreateBookingResponse(booking.Id)));
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

  private static async Task<Results<NoContent, NotFound>> CancelBooking(BookingCancelRequest request, IGlobalStateHandler globalStateHandler)
  {
    var booking = globalStateHandler.GetBooking(request.Id);

    if (booking is null) return TypedResults.NotFound();

    booking.CancelBooking();

    globalStateHandler.UpdateBooking(booking);

    return TypedResults.NoContent();
  }
}
