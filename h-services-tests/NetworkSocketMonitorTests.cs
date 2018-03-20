using Hylasoft.Services.Tests.Types.NetworkMonitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class NetworkSocketMonitorTests : TestBase
  {
    [TestMethod]
    public void TestNetworkMonitorInitialize()
    {
      var monitor = new TestNeworkMonitor();
      Assert.IsTrue(monitor.Initialize());
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestNetworkMonitorStart()
    {
      var monitor = new TestNeworkMonitor();
      Assert.IsTrue(monitor.Start());
      Assert.IsTrue(monitor.Stop());
    }
  }
}
