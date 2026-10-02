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

  private static async Task<Results<Created<CreateResourceResponse>, BadRequest<string>>>  CreateResource(CreateResourceRequest request, IResourcesRepository resourcesRepository)
  {
    var (ResourceType, Title, Description, UserId, CancellationWindowInHours, RescheduleWindowInHours) = request;

    try
    {
      Resource resource = Resource.Create(ResourceType, Title, Description, UserId, CancellationWindowInHours, RescheduleWindowInHours);
      resourcesRepository.SaveResource(resource);

      return  TypedResults.Created($"/api/resources/{resource.Id}", new CreateResourceResponse(resource.Id));
    }
    catch (ArgumentException ex)
    {
      return TypedResults.BadRequest(ex.Message);
    }
  }

  private static async Task<Results<Ok<ResourceDto>, NotFound>> GetResource(Guid id, IResourcesRepository resourcesRepository)
  {
    var resource = resourcesRepository.GetResource(id);

    if (resource is null)
    {
      return TypedResults.NotFound();
    }

    return TypedResults.Ok(ResourceDto.FromEntity(resource));
  }

  private static async Task<Ok<IEnumerable<ResourceDto>>> GetList(Guid? userId, IResourcesRepository resourcesRepository)
  {
    var resouces = resourcesRepository.GetResources(userId).Select(ResourceDto.FromEntity);

    return TypedResults.Ok(resouces);
  }
}
