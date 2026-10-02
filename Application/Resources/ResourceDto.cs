using Booking.Domain.Resource;

namespace Booking.Application.Resources;

public sealed record ResourceDto(
  Guid Id,
  ResourceType ResourceType,
  string Title,
  string Description,
  Guid UserId,
  int? CancellationWindowInHours,
  int? RescheduleWindowInHours
) {
  public static ResourceDto FromEntity(Resource r) =>
    new(r.Id, r.Type, r.Title, r.Description, r.UserId, r.CancellationWindowInHours, r.RescheduleWindowInHours); 
}
