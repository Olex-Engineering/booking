namespace Booking.Domain.Resource;

public sealed class Resource(ResourceType resourceType, string title, string description, Guid userId)
{
  public Guid Id { get; } = Guid.CreateVersion7();
  public ResourceType ResourceType { get; } = resourceType;
  public string Title { get; } = title;
  public string Description { get; } = description;
  public Guid UserId { get; } = userId;
}
 