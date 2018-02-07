using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Services.Base;

namespace Hylasoft.Services.Services
{
  public abstract class MonitorSetService<TMonitor, TItem> : HService
    where TItem : class
    where TMonitor : class, ISetMonitor<TItem>
  {
    private readonly TMonitor _monitor;

    protected TMonitor Monitor { get { return _monitor; } }

    protected MonitorSetService(TMonitor monitor)
    {
      _monitor = monitor;
    }

    #region HService Implementation
    protected override Result InitalizeService()
    {
      Result init;
      if (!(init = Monitor.Initialize()))
        return init;

      Monitor.ItemChanged += OnItemChange;
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
    #endregion

    #region Abstract Methods
    protected abstract void OnItemChange(object sender, TItem changedItem);
    #endregion
  }
}
