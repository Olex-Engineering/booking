namespace Booking.Domain.User;

public sealed class User(string name)
{
  public Guid Id { get; } = Guid.CreateVersion7();
  public string Name { get; } = name;
}
