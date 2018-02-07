using Hylasoft.Services.Tests.Types.MonitorSets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  class MonitorSetServiceTests : TestBase
  {
    [TestMethod]
    public void TestSetMonitorServiceStartup()
    {
      var service = BuildTestSetMonitorService<TestSetMonitor>();

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
      var service = BuildTestSetMonitorService<TestSetMonitor>();

      Assert.IsTrue(service.Start());
      AssertChangedValue(service, 1, "Changed01");
      Assert.IsTrue(service.Stop());
    }
  }
}
