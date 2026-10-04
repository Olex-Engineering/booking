namespace Booking.Domain.Users;

public sealed class User
{
  public Guid Id { get; } = Guid.CreateVersion7();
  public string Name { get; }

  private User(string name)
  {
    Name = name;
  }

  public static User Create(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);

    return new User(name);
  }
}
