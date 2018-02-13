using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Services;
using Hylasoft.Services.Tests.Types.MonitorSets;

namespace Hylasoft.Services.Tests.Types.Services
{
  public class TestSetMonitorService : MonitorSetService<TestSetMonitor, TestMonitorItem>
  {
    public int ItemsChanged { get; private set; }

    public int ItemsAdded { get; private set; }

    public bool HasChanged { get; private set; }

    public bool HaveBeenAdded { get; private set; }

    public TestMonitorItem ChangedItem { get; private set; }

    public Collection<TestMonitorItem> NewItems { get; private set; }

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
      Monitor.InnerSet.Add(key, value);
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
  }
}
