using System.Collections.ObjectModel;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Services;
using Hylasoft.Services.Tests.Types.MonitorSets;

namespace Hylasoft.Services.Tests.Types.Services
{
  public class TestSetMonitorService : MonitorSetService<TestSetMonitor, TestMonitorItem>
  {
    public int ItemsChanged { get; private set; }

    public int ItemsAdded { get; private set; }

    public int ItemsRemoved { get; private set; }

    public bool HasChanged { get; private set; }

    public bool HaveBeenAdded { get; private set; }

    public bool HaveBeenRemoved { get; private set; }

    public TestMonitorItem ChangedItem { get; private set; }

    public Collection<TestMonitorItem> NewItems { get; private set; }

    public Collection<TestMonitorItem> RemovedItems { get; private set; }

    public TestSetMonitor InternalMonitor
    {
      get { return Monitor; }
    }

    public TestSetMonitorService(TestSetMonitor monitor, IServiceValidator serviceValidator = null) : base(monitor, serviceValidator)
    {
      ItemsChanged = 0;
      ItemsAdded = 0;
    }

    public void ChangeItem(int key, string value)
    {
      HasChanged = false;
      Monitor.InnerSet[key] = value;
    }

    public void AddItem(int key, string value)
    {
      HaveBeenAdded = false;
      Monitor.InnerSet.Add(key, value);
    }

    public void RemoveItem(int key)
    {
      HaveBeenRemoved = false;
      Monitor.InnerSet.Remove(key);
    }

    public void WaitOnUpdate()
    {
      Monitor.WaitOnUpdate();
    }

    
    protected override void OnItemChange(object sender, TestMonitorItem changedItem)
    {
      ChangedItem = changedItem;
      ++ItemsChanged;
      HasChanged = true;
    }

    protected override void OnItemsAdded(object sender, Collection<TestMonitorItem> newItems)
    {
      NewItems = newItems;
      ++ItemsAdded;
      HaveBeenAdded = true;
    }

    protected override void OnItemsRemoved(object send, Collection<TestMonitorItem> removedItems)
    {
      RemovedItems = removedItems;
      ++ItemsRemoved;
      HaveBeenRemoved = true;
    }
  }
}
