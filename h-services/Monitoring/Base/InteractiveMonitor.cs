using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Extensions;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Services.Base;

namespace Hylasoft.Services.Monitoring.Base
{
  public abstract class InteractiveMonitor : HMonitor, IInteractiveService
  {
    protected IReadOnlyCollection<IHService> Services { get; private set; }

    public Result SetServices(IEnumerable<IHService> service)
    {
      try
      {
        if (Services == null && service != null)
          Services = service.ToCollection();

        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result GetServicesOfType<TService>(out Collection<TService> services)
      where TService : IHService
    {
      try
      {
        services = Services
          .OfType<TService>()
          .ToCollection();

        return Result.Success;
      }
      catch (Exception e)
      {
        services = null;
        return Result.Error(e);
      }
    }
  }
}
