using System.Collections.ObjectModel;
using Hylasoft.Logging;

namespace Hylasoft.Services.Interfaces
{
  public interface IHServicesProvider
  {
    string ServiceName { get; }

    IResultLogger Logger { get; }

    Collection<IHService> Services { get; }
  }
}
