using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
  public interface IServiceItemSetMonitor<in TServiceItemSet, TServiceItem, TAction> : IServiceMonitor<TServiceItemSet, TServiceItem, TAction>
    where TAction : class
    where TServiceItem : ServiceItem<TAction>
    where TServiceItemSet : ServiceItemSet<TServiceItem, TAction>
  {
    Result Start();

    Result UpdateItems();
  }
}
