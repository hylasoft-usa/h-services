using System.Collections.ObjectModel;

namespace Hylasoft.Services.Interfaces
{
  public interface IHServicesProvider
  {
    string ServiceName { get; set; }

    ILogger Logger { get; set; }

    Collection<IHService> Services { get; }
  }
}
