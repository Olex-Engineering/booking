using Booking.Domain.Resources;

namespace Booking.API.DTO.Resource;

public sealed record CreateResourceRequest(ResourceType ResourceType, string Title, string Description, Guid UserId, int? CancellationWindowInHours, int? RescheduleWindowInHours);
