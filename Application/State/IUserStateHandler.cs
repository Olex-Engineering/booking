using Booking.Domain.User;

namespace Booking.Application.State;

public interface IUserStateHandler
{
  public void SaveUser(User user);
  public User? GetUser(Guid id);
}
