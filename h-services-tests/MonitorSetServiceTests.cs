using System.Linq;
using Hylasoft.Services.Tests.Base;
using Hylasoft.Services.Tests.Types.MonitorSets;
using Hylasoft.Services.Tests.Types.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class MonitorSetServiceTests : SetMonitorTestBase
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

    [TestMethod]
    public void TestSetMonitorNewlyAddedValue()
    {
      var service = BuildTestSetMonitorService<TestSetMonitor>();
      Assert.IsTrue(service.Start());

      const int newKey = 12;
      const string newInitValue = "Value12";
      const string newChangedValue = "Changed12";

      AssertAddedValue(service, newKey, newInitValue);
      AssertChangedValue(service, newKey, newChangedValue);

      Assert.IsTrue(service.Stop());
    }

    [TestMethod]
    public void TestSetMonitorRemovedValue()
    {
      var service = BuildTestSetMonitorService<TestSetMonitor>();
      Assert.IsTrue(service.Start());

      var removedItem = InitialValues.FirstOrDefault();
      Assert.IsNotNull(removedItem);

      var itemToRemove = removedItem.Key;
      AssertRemovedValue(service, itemToRemove);

      const int newKey = 15;
      const string newInitValue = "Value15";
      const string newChangedValue = "Changed15";

      AssertAddedValue(service, newKey, newInitValue);
      AssertChangedValue(service, newKey, newChangedValue);

      Assert.IsTrue(service.Stop());
    }

    [TestMethod]
    public void TestSetMonitorServiceFailedStartup()
    {
      FailedSetMonitor monitor;
      TestSetMonitorService service;
      BuildFailedTestSetMonitorService(out service, out monitor);
      monitor.ShouldFail = true;


      Assert.IsFalse(service.Start());
      AssertIsFailed(service);
      AssertIsFailed(monitor);
    }

    [TestMethod]
    public void TestSetMonitorFailureInService()
    {
      FailedSetMonitor monitor;
      TestSetMonitorService service;
      BuildFailedTestSetMonitorService(out service, out monitor);
      monitor.ShouldFail = false;

      Assert.IsTrue(service.Start());

      monitor.ShouldFail = true;
      monitor.WaitOnUpdate();
      
      AssertIsFailed(monitor);
      AssertIsFailed(service);
      Assert.IsTrue(service.Stop());

      monitor.ShouldFail = false;
      Assert.IsTrue(service.Start());
      monitor.WaitOnUpdate();

      Assert.IsTrue(service.IsRunning);
      Assert.IsTrue(service.Stop());

      monitor.WaitOnUpdate();
      Assert.IsFalse(monitor.IsRunning);
      Assert.IsFalse(service.IsRunning);
    }

    protected void BuildFailedTestSetMonitorService(out TestSetMonitorService service, out FailedSetMonitor monitor)
    {
      service = BuildTestSetMonitorService<FailedSetMonitor>();
      Assert.IsNotNull(service);
      Assert.IsTrue(service.Initialize());

      monitor = service.InternalMonitor as FailedSetMonitor;
      Assert.IsNotNull(monitor);
    }
  }
}
