using Booking.Domain.BookingEntity;
using Booking.Domain.Resource;

namespace Booking.Application.Resourses;

public sealed record ResourseDto(
  Guid Id,
  ResourceType ResourseType,
  string Title,
  string Description,
  Guid UserId,
  List<BookingEnitity> Bookings
) {
  public static ResourseDto FromEntity(Resource r) =>
    new(r.Id, r.ResourceType, r.Title, r.Description, r.UserId, r.Bookings); 
}
