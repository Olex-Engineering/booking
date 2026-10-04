using Booking.Domain.Resources;

namespace Booking.Application.State;

public interface IResourcesRepository
{
  public void SaveResource(Resource resource);
  public Resource? GetResource(Guid id);
  public IEnumerable<Resource> GetResources(Guid? userId);

}
