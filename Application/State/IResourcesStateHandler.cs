using Booking.Domain.Resource;

namespace Booking.Application.State;

public interface IResourcesStateHandler
{
  public void SaveResource(Resource resource);
  public Resource? GetResource(Guid id);
  public IEnumerable<Resource> GetResources(Guid? userId);

}
