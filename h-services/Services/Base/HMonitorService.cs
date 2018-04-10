using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Services.Base
{
  public abstract class HMonitorService<TMonitor> : HService
    where TMonitor : class, IMonitor
  {
    private readonly TMonitor _monitor;

    protected TMonitor Monitor { get { return _monitor; } }

    protected HMonitorService(TMonitor monitor, IServiceValidator validator) : base(validator)
    {
      _monitor = monitor;
    }

    protected override Result OnInitialize()
    {
      var init = Result.Success;
      try
      {
        init += Monitor.Initialize();
        Monitor.StatusChanged += OnMonitorTransition;
      }
      catch (Exception e)
      {
        init += Result.Error(e);
      }

      return init;
    }

    protected override Result OnStart()
    {
      return Monitor.Start();
    }

    protected override Result OnStop()
    {
      return Monitor.Stop();
    }

    protected override Result OnPause()
    {
      return Monitor.Pause();
    }

    public override string ServiceName
    {
      get { return Monitor.ServiceName; }
    }

    private void OnMonitorTransition(object sender, ServiceStatusTransition transition)
    {
      if (transition == null) return;

      var reason = transition.Reason;
      // If the monitor fails, also have the service fail.
      if (transition.CurrentStatus == ServiceStatuses.Failed)
        ErrorOut(reason);
    }
  }
}
