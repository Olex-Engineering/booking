using Booking.API.DTO.Resource;
using Booking.Application.Resources;
using Booking.Application.State;
using Booking.Domain.Resource;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Booking.API.Endpoints;

internal static class ResourceEndpoints
{
  public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/resources").WithTags("Resources");

    group.MapPost("/", CreateResource).WithName("Create resource");
    group.MapGet("/{id}", GetResource).WithName("Get resource by id");
    group.MapGet("/list", GetList).WithName("Get resource list");

    return app;
  }

  private static async Task<Created<CreateResourceResponse>>  CreateResource(CreateResourceRequest request, IGlobalStateHandler globalStateHandler)
  {
    Resource resource = new(request.ResourceType, request.Title, request.Description, request.UserId);

    globalStateHandler.SaveResource(resource);

    return await Task.FromResult(TypedResults.Created($"/api/resources/{resource.Id}", new CreateResourceResponse(resource.Id)));
  }

  private static async Task<Results<Ok<ResourceDto>, NotFound>> GetResource(Guid id, IGlobalStateHandler globalStateHandler)
  {
    var resource = globalStateHandler.GetResource(id);

    if (resource is null)
    {
      return TypedResults.NotFound();
    }

    return TypedResults.Ok(ResourceDto.FromEntity(resource));
  }

  private static async Task<Ok<IEnumerable<ResourceDto>>> GetList(Guid? userId, IGlobalStateHandler globalStateHandler)
  {
    var resouces = globalStateHandler.GetResources(userId).Select(ResourceDto.FromEntity);

    return TypedResults.Ok(resouces);
  }
}
