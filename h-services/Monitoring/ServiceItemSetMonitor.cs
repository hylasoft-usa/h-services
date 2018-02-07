using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Monitoring
{
  public abstract class ServiceItemSetMonitor<TServiceItemSet, TServiceItem, TAction, TConfig> : ServiceMonitor<TServiceItemSet, TServiceItem, TAction, TConfig>
    where TAction : class
    where TServiceItem : ServiceItem<TAction>
    where TServiceItemSet : ServiceItemSet<TServiceItem, TAction>
    where TConfig : IMonitoringConfig, new()
  {
    protected ServiceItemSetMonitor(TConfig config) : base(config)
    {
    }

    protected override void RunMonitorLoop()
    {
      // TODO: Implement this.
      throw new System.NotImplementedException();
    }
  }
}
