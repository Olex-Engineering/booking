using Booking.Domain.Users;

namespace Booking.Application.State;

public sealed class UsersRepository(IStateContext stateContext) : IUsersRepository
{
  public User? GetUser(Guid id)
  {

    return stateContext.GetUser(id);
  }

  public void SaveUser(User user)
  {
    stateContext.SaveUser(user);
  }
}
