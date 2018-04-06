using System.Collections.ObjectModel;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Services.Base;

namespace Hylasoft.Services.Services
{
  public abstract class MonitorSetService<TMonitor, TItem> : HMonitorService<TMonitor>, IMonitorSetService<TItem>
    where TItem : class
    where TMonitor : class, ISetMonitor<TItem>
  {
    protected MonitorSetService(TMonitor monitor, IServiceValidator serviceValidator) : base(monitor, serviceValidator)
    {
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
    #endregion

    #region IMonitorSetService Implementation
    public Result GetCurrentSet(out Collection<TItem> monitoredItems)
    {
      return Monitor.GetCurrentSet(out monitoredItems);
    }
    #endregion

    #region Abstract Methods
    protected abstract void OnItemChange(object sender, TItem changedItem);

    protected abstract void OnItemsAdded(object sender, Collection<TItem> newItems);

    protected abstract void OnItemsRemoved(object send, Collection<TItem> removedItems);

    #endregion
  }
}
