namespace Booking.API.Endpoints;

using System.Diagnostics;
using Booking.API.DTO.Booking;
using Booking.Application.Bookings;
using Booking.Application.State;
using Booking.Domain.Bookings;
using Booking.Domain.Common;
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
    group.MapPatch("/reschedule", RescheduleBooking).WithName("Reschedule booking");
    group.MapPatch("/confirm", ConfirmBooking).WithName("Confirm booking");
    group.MapGet("/{id:guid}", GetBooking).WithName("Get booking by id");
    group.MapGet("/list", GetList).WithName("Get booking list");

    return app;
  }

  private static async Task<Results<Created<CreateBookingResponse>, BadRequest<string>, Conflict<string>>> CreateBooking(CreateBookingRequest request, IBookingsRepository bookingsRepository, TimeProvider timeProvider)
  {
    var (ResourceId, UserId, From, To) = request;

    if (ResourceId == Guid.Empty || UserId == Guid.Empty)
    {
      return TypedResults.BadRequest("Guid cannot be empty.");
    }

    try
    {
      TimeInterval timeInterval = new(From, To);
      BookingEntity booking = BookingEntity.Create(ResourceId, UserId, timeInterval, timeProvider.GetUtcNow());

      var result = await bookingsRepository.SaveBooking(booking);

      return result.Type switch
      {
        ResultType.Ok => TypedResults.Created($"/api/bookings/{result.Value}", new CreateBookingResponse(result.Value)),
        ResultType.Conflict => TypedResults.Conflict(result.Message),
        _ => throw new UnreachableException($"Unhandled result type: {result.Type}"),
      };

    } catch (ArgumentOutOfRangeException)
    {
      return TypedResults.BadRequest("From and to properties are wrong.");
    }
  }

  private static async Task<Results<Ok<BookingDto>, NotFound>> GetBooking(Guid id, IBookingsRepository bookingsRepository, TimeProvider timeProvider)
  {
    var booking = bookingsRepository.GetBooking(id);

    if (booking is null)
    {
      return TypedResults.NotFound();
    }

    return TypedResults.Ok(BookingDto.FromEntity(booking, timeProvider.GetUtcNow()));
  }

  private static async Task<Ok<IEnumerable<BookingDto>>> GetList([AsParameters] BookingFilters filters, IBookingsRepository bookingsRepository, TimeProvider timeProvider)
  {
    var now = timeProvider.GetUtcNow();
    var bookings = bookingsRepository.GetBookings(filters);
    var bookingsDto = bookings.Select(b => BookingDto.FromEntity(b, now));

    return TypedResults.Ok(bookingsDto);
  }

  private static async Task<Results<NoContent, NotFound<string>, Conflict<string>>> CancelBooking(BookingCancelRequest request, IBookingsRepository bookingsRepository)
  {
    var result = await bookingsRepository.CancelBooking(request.Id);

    return ToActionResult(result);
  }

  private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>, Conflict<string>>> RescheduleBooking(BookingRescheduleRequest request, IBookingsRepository bookingsRepository)
  {
    try
    {
      TimeInterval newTimeInterval = new(request.From, request.To);

      var result = await bookingsRepository.RescheduleBooking(request.Id, newTimeInterval);

      return result.Type switch
      {
        ResultType.Ok => TypedResults.NoContent(),
        ResultType.NotFound => TypedResults.NotFound(result.Message),
        ResultType.Conflict => TypedResults.Conflict(result.Message),
        _ => throw new UnreachableException($"Unhandled result type: {result.Type}"),
      };
    } catch (ArgumentOutOfRangeException)
    {
      return TypedResults.BadRequest("From and to properties are wrong.");
    }
  }

  private static async Task<Results<NoContent, NotFound<string>, Conflict<string>>> ConfirmBooking(BookingConfirmRequest request, IBookingsRepository bookingsRepository)
  {
    var result = await bookingsRepository.ConfirmBooking(request.Id);

    return ToActionResult(result);
  }

  private static Results<NoContent, NotFound<string>, Conflict<string>> ToActionResult(Result<Guid> result) =>
    result.Type switch
    {
      ResultType.Ok => TypedResults.NoContent(),
      ResultType.NotFound => TypedResults.NotFound(result.Message),
      ResultType.Conflict => TypedResults.Conflict(result.Message),
      _ => throw new UnreachableException($"Unhandled result type: {result.Type}"),
    };
}
