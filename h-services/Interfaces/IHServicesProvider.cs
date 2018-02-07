using System.Collections.ObjectModel;

namespace Hylasoft.Services.Interfaces
{
  public interface IHServicesProvider
  {
    string ServiceName { get; }

    ILogger Logger { get; }

    Collection<IHService> Services { get; }
  }
}
