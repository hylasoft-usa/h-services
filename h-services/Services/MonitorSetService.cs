using System.Collections.ObjectModel;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Services.Base;

namespace Hylasoft.Services.Services
{
  public abstract class MonitorSetService<TMonitor, TItem> : HService, IMonitorSetService
    where TItem : class
    where TMonitor : class, ISetMonitor<TItem>
  {
    private readonly TMonitor _monitor;

    protected TMonitor Monitor { get { return _monitor; } }

    protected MonitorSetService(TMonitor monitor, IServiceValidator serviceValidator) : base(serviceValidator)
    {
      _monitor = monitor;
    }

    #region HService Implementation
    protected override Result OnInitialize()
    {
      Result init;
      if (!(init = Monitor.Initialize()))
        return init;

      Monitor.ItemChanged += OnItemChange;
      Monitor.ItemsAdded += OnItemsAdded;
      Monitor.ItemsRemoved += OnItemsRemoved;
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

    protected abstract void OnItemsAdded(object sender, Collection<TItem> newItems);

    protected abstract void OnItemsRemoved(object send, Collection<TItem> removedItems);

    #endregion
  }
}
