using System.Collections.Generic;
using System.Linq;
using Hylasoft.Resolution;
using Hylasoft.Services.Monitoring;

namespace Hylasoft.Services.Tests.Types.MonitorSets
{
  public class TestSetMonitor : SetMonitor<TestMonitorItem, int>
  {
    private readonly Dictionary<int, string> _innerSet;

    public TestSetMonitor()
    {
      _innerSet = new Dictionary<int, string>();
    }

    public IDictionary<int, string> InnerSet { get { return _innerSet; } }

    protected override Result FetchSet(out IEnumerable<TestMonitorItem> items)
    {
      items = InnerSet.Select(kvp => new TestMonitorItem(kvp.Key, kvp.Value));
      return Result.Success;
    }

    protected override bool HasItemChanged(TestMonitorItem oldItem, TestMonitorItem newItem)
    {
      return oldItem.Value != newItem.Value;
    }

    protected override int GetSpecification(TestMonitorItem item)
    {
      return item == null
        ? -1
        : item.Key;
    }

    protected override string MonitorName
    {
      get { return "Test Set Monitor"; }
    }

    protected override bool AreItemsEqual(int a, int b)
    {
      return a == b;
    }

    protected override int GetItemSpecHash(int spec)
    {
      return spec;
    }
  }
}
