using System;
using System.Collections.ObjectModel;

namespace Hylasoft.Services.Interfaces.Monitoring
{
  public interface ISetMonitor<TItem> : IMonitor
    where TItem : class
  {
    event EventHandler<TItem> ItemChanged;

    event EventHandler<Collection<TItem>> ItemsAdded;

    event EventHandler<Collection<TItem>> ItemsRemoved;
  }
}
