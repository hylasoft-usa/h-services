using System;
using System.Collections.ObjectModel;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Monitoring
{
  public interface IKeepAliveMonitor : IMonitor, IInteractiveService
  {
    event EventHandler<IHService> RevivingService;

    event EventHandler<IHService> ServiceRevived;

    event EventHandler<IHService> ReviveFailed;

    Result GetServiceStatusInformation(out Collection<ServiceStatusInformation> statusInformation);
  }
}
