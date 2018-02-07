using System.Collections.Generic;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Tests.Types.MonitorSets
{
  public class FailedSetMonitor : TestSetMonitor
  {
    public bool ShouldFail { get; set; }

    protected override Result FetchSet(out IEnumerable<TestMonitorItem> items)
    {
      var fetch = base.FetchSet(out items);
      return ShouldFail
        ? fetch + Result.SingleError("This failed")
        : fetch;
    }
  }
}
