using System;

namespace Hylasoft.Services.Interfaces
{
  public interface ISetMonitor<TItem> : IMonitor
    where TItem : class
  {
    event EventHandler<TItem> ItemChanged;
  }
}
