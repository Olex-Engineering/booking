namespace Booking.Domain.Resources;

public sealed class Resource
{
  public const int MaxWindowInHours = 24 * 365;

  public Guid Id { get; } = Guid.CreateVersion7();
  public ResourceType Type { get; }
  public string Title { get; }
  public string Description { get; }
  public Guid UserId { get; }
  public int CancellationWindowInHours { get; }
  public int RescheduleWindowInHours { get; }

  private Resource(ResourceType type, string title, string description, Guid userId, int cancellationWindowInHours, int rescheduleWindowInHours)
  {
    Type = type;
    Title = title;
    Description = description;
    UserId = userId;
    CancellationWindowInHours = cancellationWindowInHours;
    RescheduleWindowInHours = rescheduleWindowInHours;
  }

  public static Resource Create(ResourceType type, string title, string description, Guid userId, int? cancellationWindowInHours, int? rescheduleWindowInHours)
  {
    if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown resource type.");
    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(description);
    if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(userId));

    var cancellationWindow = cancellationWindowInHours ?? DefaultWindowInHours(type);
    var rescheduleWindow = rescheduleWindowInHours ?? DefaultWindowInHours(type);

    ArgumentOutOfRangeException.ThrowIfNegative(cancellationWindow, nameof(cancellationWindowInHours));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(cancellationWindow, MaxWindowInHours, nameof(cancellationWindowInHours));
    ArgumentOutOfRangeException.ThrowIfNegative(rescheduleWindow, nameof(rescheduleWindowInHours));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(rescheduleWindow, MaxWindowInHours, nameof(rescheduleWindowInHours));

    return new Resource(type, title, description, userId, cancellationWindow, rescheduleWindow);
  }

  public bool IsCancellationWindowValid(DateTimeOffset bookingFrom, DateTimeOffset now) =>
    IsOutsideWindow(bookingFrom, now, CancellationWindowInHours);

  public bool IsRescheduleWindowValid(DateTimeOffset bookingFrom, DateTimeOffset now) =>
    IsOutsideWindow(bookingFrom, now, RescheduleWindowInHours);

  private static bool IsOutsideWindow(DateTimeOffset bookingFrom, DateTimeOffset now, int windowInHours) =>
    bookingFrom >= now.AddHours(windowInHours);

  private static int DefaultWindowInHours(ResourceType type) => type switch
  {
    ResourceType.Master => 24,
    ResourceType.Property => 48,
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
}
