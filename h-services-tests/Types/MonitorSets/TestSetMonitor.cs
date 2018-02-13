using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hylasoft.Resolution;
using Hylasoft.Services.Monitoring;

namespace Hylasoft.Services.Tests.Types.MonitorSets
{
  public class TestSetMonitor : SetMonitor<TestMonitorItem, int>
  {
    private readonly object _updateLock = new object();
    private readonly Dictionary<int, string> _innerSet;
    private bool _updateOccured;

    protected bool UpdateOccured
    {
      get
      {
        lock (_updateLock)
          return _updateOccured;
      }

      private set
      {
        lock (_updateLock)
          _updateOccured = value;
      }
    }

    public TestSetMonitor()
    {
      _innerSet = new Dictionary<int, string>();
    }

    public void WaitOnUpdate()
    {
      UpdateOccured = false;
      while(IsRunning && !UpdateOccured)
        Thread.Sleep(100);
    }

    public IDictionary<int, string> InnerSet { get { return _innerSet; } }

    protected sealed override Result PerformServiceLoop()
    {
      var cleanSlate = !UpdateOccured;
      var perform = base.PerformServiceLoop();
      if (cleanSlate)
        UpdateOccured = true;

      return perform;
    }

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
