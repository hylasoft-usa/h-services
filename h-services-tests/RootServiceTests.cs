using Hylasoft.Services.Tests.Base;
using Hylasoft.Services.Tests.Types.MonitorSets;
using Hylasoft.Services.Tests.Types.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class RootServiceTests : SetMonitorTestBase
  {
    [TestMethod]
    public void TestServiceRootOnStart()
    {
      TestSetMonitorService testService;
      var root = BuildRootRunner<TestSetMonitor>(out testService);

      var args = new string[0];
      
      root.OnStart(args);
      Assert.IsTrue(testService .IsRunning);
      root.OnStop();
      Assert.IsTrue(testService.IsStopped);
      Assert.IsFalse(testService.IsFailed);
    }

    [TestMethod]
    public void TestServiceRootValueChange()
    {
      TestSetMonitorService testService;
      var root = BuildRootRunner<TestSetMonitor>(out testService);

      var args = new string[0];
      root.OnStart(args);
      AssertChangedValue(testService, 1, "FromServiceRoot01");
      root.OnStop();
    }

    [TestMethod]
    public void TestServiceRootValueAdded()
    {
      TestSetMonitorService testService;
      var root = BuildRootRunner<TestSetMonitor>(out testService);

      root.OnStart(new string[0]);
      AssertAddedValue(testService, 5, "NewFromServiceRoot");
      root.OnStop();
    }
  }
}
