using Booking.Domain.Users;

namespace Booking.Application.Users;

public sealed record UserDto(
  Guid Id,
  string Name
)
{
  public static UserDto FromEntity(User user)
  {
    return new(user.Id, user.Name);
  }
}
