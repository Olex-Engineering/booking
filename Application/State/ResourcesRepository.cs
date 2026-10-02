using Booking.Domain.Resource;

namespace Booking.Application.State;

public sealed class ResourcesRepository(IStateContext stateContext): IResourcesRepository
{
  public Resource? GetResource(Guid id) => stateContext.GetResource(id);

  public IEnumerable<Resource> GetResources(Guid? userId)
  {
    IEnumerable<Resource> resources = stateContext.GetResources();

    if (userId is not null)
    {
      resources = resources.Where(r => r.UserId == userId);
    }

    return resources;
  }

  public void SaveResource(Resource resource)
  {
    stateContext.SaveResource(resource);
  }
}
