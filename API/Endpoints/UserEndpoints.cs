using Booking.API.DTO.User;
using Booking.Application.State;
using Booking.Application.Users;
using Booking.Domain.User;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Booking.API.Endpoints;

internal static class UserEndpoints
{
  public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/users").WithTags("Users");

    group.MapPost("/", CreateUser).WithName("Create user");
    group.MapGet("/{id}", GetUser).WithName("User By Id");

    return app;
  }

  private static async Task<Results<Created<CreateUserResponse>, BadRequest<string>>> CreateUser(CreateUserRequest request, IUsersRepository userRepository)
  {
    try
    {
      User user = User.Create(request.Name);

      userRepository.SaveUser(user);

      return TypedResults.Created($"/api/users/{user.Id}", new CreateUserResponse(user.Id));
    }
    catch (ArgumentException ex)
    {
      return TypedResults.BadRequest(ex.Message);
    }
  }

  private static async Task<Results<Ok<UserDto>, NotFound>> GetUser(Guid id, IUsersRepository userRepository)
  {
    var user = userRepository.GetUser(id);

    if (user is null)
    {
      return TypedResults.NotFound();
    }

    return TypedResults.Ok(UserDto.FromEntity(user));
  }
}
