using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Validation;

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
      return Monitor.Initialize();
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
  }
}
