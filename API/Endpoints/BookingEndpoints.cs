namespace Booking.API.Endpoints;

using System.Diagnostics;
using Booking.API.DTO.Booking;
using Booking.Application.Bookings;
using Booking.Application.State;
using Booking.Domain.BookingEntity;
using Booking.Domain.Common;
using Booking.Domain.User;
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

    group.MapPost("/", CreateBooking).WithName("Create booking");
    group.MapPatch("/cancel", CancelBooking).WithName("Cancel booking");
    group.MapGet("/{id}", GetBooking).WithName("Get booking by id");
    group.MapGet("/list", GetList).WithName("Get booking list");

    return app;
  }

  private static async Task<Results<Created<CreateBookingResponse>,  BadRequest<string>, Conflict<string>>>  CreateBooking(CreateBookingRequest request, IGlobalStateHandler globalStateHandler)
  {
    var (ResourceId, UserId, From, To) = request;

    if (ResourceId == Guid.Empty || UserId == Guid.Empty)
    {
      return TypedResults.BadRequest("Guid cannot be empty.");
    }

    try
    {
      TimeInterval timeInterval = new(From, To);
      BookingEntity booking = new(ResourceId, UserId, timeInterval);

      var result = await globalStateHandler.SaveBooking(booking);

      return result.Type switch
      {
        ResultType.Ok => TypedResults.Created($"/api/bookings/{result.Value}", new CreateBookingResponse(result.Value)),
        ResultType.Conflict => TypedResults.Conflict("Booking period conflict (from, to) with other bookings."),
        _ => throw new UnreachableException($"Unhandled result type: {result.Type}"),
      };

    } catch (ArgumentOutOfRangeException)
    {
      return TypedResults.BadRequest("From, to properties are wrong.");
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

  private static async Task<Results<NoContent, NotFound, Conflict>> CancelBooking(BookingCancelRequest request, IGlobalStateHandler globalStateHandler)
  {
    var booking = globalStateHandler.GetBooking(request.Id);

    if (booking is null) return TypedResults.NotFound();

    var result = booking.CancelBooking();

    return result.Type switch
    {
      ResultType.Ok => TypedResults.NoContent(),
      ResultType.Conflict => TypedResults.Conflict(),
      _ => throw new UnreachableException($"Unhandled result type: {result.Type}"),
    };
  }
}
