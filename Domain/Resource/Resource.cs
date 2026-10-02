namespace Booking.Domain.Resource;

public sealed class Resource
{
  public Guid Id { get; } = Guid.CreateVersion7();
  public ResourceType Type { get; }
  public string Title { get; }
  public string Description { get; }
  public Guid UserId { get; }
  public int CancellationWindowInHours { get; }
  public int RescheduleWindowInHours { get; }

  private Resource(ResourceType type, string title, string description, Guid userId, int? cancellationWindowInHours, int? rescheduleWindowInHours)
  {
    Type = type;
    Title = title;
    Description = description;
    UserId = userId;
    CancellationWindowInHours = cancellationWindowInHours ?? type switch
  {
    ResourceType.Master => 24,
    ResourceType.Property => 48,
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
    RescheduleWindowInHours = rescheduleWindowInHours ?? type switch
  {
    ResourceType.Master => 24,
    ResourceType.Property => 48,
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
  }

  public static Resource Create(ResourceType type, string title, string description, Guid userId, int? cancellationWindowInHours, int? rescheduleWindowInHours)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(description);
    ArgumentOutOfRangeException.ThrowIfNegative(cancellationWindowInHours ?? 0);
    ArgumentOutOfRangeException.ThrowIfNegative(rescheduleWindowInHours ?? 0);
    if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(userId));

    return new Resource(type, title, description, userId, cancellationWindowInHours, rescheduleWindowInHours);
  }

  public bool ValidateBookingCancellationWindow(DateTimeOffset bookingFrom, DateTimeOffset now)
  {
    var cancellationWindow = now.AddHours(CancellationWindowInHours);

    return bookingFrom >= cancellationWindow;
  }

  public bool ValidateBookingRescheduleWindow(DateTimeOffset bookingFrom, DateTimeOffset now)
  {
    var rescheduleWindow = now.AddHours(RescheduleWindowInHours);

    return bookingFrom >= rescheduleWindow;
  }
}
