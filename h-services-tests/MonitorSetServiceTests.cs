using System.Threading;
using Hylasoft.Services.Tests.Types.MonitorSets;
using Hylasoft.Services.Tests.Types.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  class MonitorSetServiceTests : TestBase
  {
    [TestMethod]
    public void TestSetMonitorServiceStartup()
    {
      var service = BuildTestService<TestSetMonitor>();

      Assert.IsTrue(service.Start());
      Assert.IsTrue(service.IsRunning);

      Assert.IsTrue(service.Stop());
      Assert.IsTrue(service.IsStopped);
      Assert.IsFalse(service.IsFailed);

      Assert.AreEqual(service.ItemsChanged, 0);
      Assert.IsNull(service.ChangedItem);
    }

    [TestMethod]
    public void TestSetMonitorChangedValue()
    {
      var service = BuildTestService<TestSetMonitor>();

      Assert.IsTrue(service.Start());
      AssertChangedValue(service, 1, "Changed01");
      Assert.IsTrue(service.Stop());
    }

    protected TestSetMonitorService BuildTestService<TMonitor>()
      where TMonitor : TestSetMonitor, new()
    {
      return new TestSetMonitorService(BuildTestMonitor<TMonitor>());
    }

    protected TMonitor BuildTestMonitor<TMonitor>()
      where TMonitor : TestSetMonitor, new()
    {
      var monitor = new TMonitor();
      InitializeSet(monitor);

      return monitor;
    }

    protected void AssertChangedValue(TestSetMonitorService service, int key, string value)
    {
      service.ChangeItem(key, value);
      while (!service.HasChanged)
        Thread.Sleep(200);

      TestMonitorItem item;
      Assert.IsNotNull(item = service.ChangedItem);
      Assert.AreEqual(item.Key, key);
      Assert.AreEqual(item.Value, value);
    }
  }
}
