using System.Collections.Generic;
using Hylasoft.Services.Tests.Types.MonitorSets;

namespace Hylasoft.Services.Tests
{
  public abstract class TestBase
  {
    private readonly KeyValuePair<int, string>[] _initialValues =
    {
      new KeyValuePair<int, string>(1, "Test01"),
      new KeyValuePair<int, string>(2, "Test02"),
      new KeyValuePair<int, string>(3, "Test03") 
    };

    protected KeyValuePair<int, string>[] InitialValues { get { return _initialValues; } }

    protected void InitializeSet(TestSetMonitor monitor)
    {
      monitor.InnerSet.Clear();
      foreach (var value in InitialValues)
        monitor.InnerSet.Add(value.Key, value.Value);
    }
  }
}
