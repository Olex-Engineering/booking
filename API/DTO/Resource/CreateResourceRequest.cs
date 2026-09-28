using Booking.Domain.Resource;

namespace Booking.API.DTO.Resource;

public sealed record CreateResourceRequest(ResourceType ResourceType, string Title, string Description, Guid UserId);
