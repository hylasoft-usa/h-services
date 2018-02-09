using System.Collections.Generic;
using Hylasoft.Services.Services;
using Hylasoft.Services.Tests.Types.MonitorSets;

namespace Hylasoft.Services.Tests.Types.Services
{
  public class TestSetMonitorService : MonitorSetService<TestSetMonitor, TestMonitorItem>
  {
    public int ItemsChanged { get; private set; }

    public bool HasChanged { get; private set; }

    public TestMonitorItem ChangedItem { get; private set; }

    public IDictionary<int, string> InnerSet { get { return Monitor.InnerSet; } } 

    public TestSetMonitorService(TestSetMonitor monitor) : base(monitor)
    {
      ItemsChanged = 0;
    }

    public void ChangeItem(int key, string value)
    {
      HasChanged = false;
      Monitor.InnerSet[key] = value;
    }
    
    protected override void OnItemChange(object sender, TestMonitorItem changedItem)
    {
      ChangedItem = changedItem;
      ++ItemsChanged;
      HasChanged = true;
    }
  }
}
