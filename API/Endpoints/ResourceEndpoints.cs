using Booking.API.DTO.Resource;
using Booking.Application.Resourses;
using Booking.Application.State;
using Booking.Domain.Resource;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Booking.API.Endpoints;

internal static class BookingEndpoints
{
  public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/resource").WithTags("Resources");

    group.MapPost("/", CreateResource).WithName("Create resource");
    group.MapGet("/{id}", GetResource).WithName("Get resource by id");
    group.MapGet("/list", GetList).WithName("Get resource list");

    return app;
  }

  private static async Task<Created<CreateResourseResponse>>  CreateResource(CreateResourceRequest request, IGlobalStateHandler globalStateHandler)
  {
    Resource resource = new(request.ResourceType, request.Title, request.Description, request.UserId);

    globalStateHandler.SaveResource(resource);

    return await Task.FromResult(TypedResults.Created("/resouces", new CreateResourseResponse(resource.Id)));
  }

  private static async Task<Results<Ok<ResourseDto>, NotFound>> GetResource(Guid id, IGlobalStateHandler globalStateHandler)
  {
    var resource = globalStateHandler.GetResource(id);

    if (resource is null)
    {
      return TypedResults.NotFound();
    }

    return TypedResults.Ok(ResourseDto.FromEntity(resource));
  }

  private static async Task<Ok<IEnumerable<Resource>>> GetList(Guid? userId, IGlobalStateHandler globalStateHandler)
  {
    return TypedResults.Ok(globalStateHandler.GetResources(userId));
  }
}
