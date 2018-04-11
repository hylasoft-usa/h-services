using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Services
{
  public class KeepAliveService : HMonitorService<IKeepAliveMonitor>, IKeepAliveService
  {
    public KeepAliveService(IKeepAliveMonitor monitor, IServiceValidator validator) : base(monitor, validator)
    {
      Monitor.RevivingService += OnRevivingService;
      Monitor.ServiceRevived += OnServiceRevived;
      Monitor.ReviveFailed += OnReviveFailed;
    }

    public Result SetServices(IEnumerable<IHService> services)
    {
      return Monitor.SetServices(services);
    }

    public Result GetServiceStatusInformation(out Collection<ServiceStatusInformation> statusInformation)
    {
      return Monitor.GetServiceStatusInformation(out statusInformation);
    }

    public event EventHandler<IHService> RevivingService;

    public event EventHandler<IHService> ServiceRevived;

    public event EventHandler<IHService> ReviveFailed;

    protected virtual void OnRevivingService(object sender, IHService service)
    {
      if (RevivingService != null) RevivingService(sender, service);
    }

    protected virtual void OnServiceRevived(object sender, IHService service)
    {
      if (ServiceRevived != null) ServiceRevived(sender, service);
    }

    protected virtual void OnReviveFailed(object sender, IHService service)
    {
      if (ReviveFailed != null) ReviveFailed(sender, service);
    }
  }
}
