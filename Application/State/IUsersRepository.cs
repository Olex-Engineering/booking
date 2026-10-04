using Booking.Domain.Users;

namespace Booking.Application.State;

public interface IUsersRepository
{
  public void SaveUser(User user);
  public User? GetUser(Guid id);
}
