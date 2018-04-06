using Hylasoft.Resolution;
using System.Collections.ObjectModel;
using Hylasoft.Services.Interfaces.Services.Base;

namespace Hylasoft.Services.Interfaces.Services
{
  public interface IMonitorSetService<TItem> : IHService
    where TItem : class
  {
    Result GetCurrentSet(out Collection<TItem> monitoredItems);
  }
}
