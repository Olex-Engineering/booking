using Booking.Domain.Resource;

namespace Booking.Application.Resources;

public sealed record ResourceDto(
  Guid Id,
  ResourceType ResourceType,
  string Title,
  string Description,
  Guid UserId
) {
  public static ResourceDto FromEntity(Resource r) =>
    new(r.Id, r.ResourceType, r.Title, r.Description, r.UserId); 
}
