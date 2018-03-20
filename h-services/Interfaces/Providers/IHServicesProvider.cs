using System.Collections.ObjectModel;
using Hylasoft.Logging;
using Hylasoft.Services.Interfaces.Services;

namespace Hylasoft.Services.Interfaces.Providers
{
  public interface IHServicesProvider
  {
    string ServiceName { get; }

    IResultLogger Logger { get; }

    Collection<IHService> Services { get; }
  }
}
