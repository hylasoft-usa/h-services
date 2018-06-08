using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Logging;
using Hylasoft.Services.Interfaces.Providers;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Providers;
using Hylasoft.Services.Service;
using Hylasoft.Services.Tests.Types.Loggers;
using Hylasoft.Services.Tests.Types.MonitorSets;
using Hylasoft.Services.Tests.Types.Services;
using Hylasoft.Services.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests.Base
{
  public abstract class SetMonitorTestBase : TestBase
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

    protected TestSetMonitorService BuildTestSetMonitorService<TMonitor>()
      where TMonitor : TestSetMonitor, new()
    {
      var service = new TestSetMonitorService(BuildTestMonitor<TMonitor>());
      service.Initialize();

      return service;
    }
    
    protected TMonitor BuildTestMonitor<TMonitor>()
      where TMonitor : TestSetMonitor, new()
    {
      var monitor = new TMonitor();
      InitializeSet(monitor);

      return monitor;
    }

    protected IHServiceRootRunner BuildRootRunner<TMonitor>(out TestSetMonitorService monitorService)
      where TMonitor : TestSetMonitor, new()
    {
      monitorService = BuildTestSetMonitorService<TMonitor>();

      var provider = BuildServicesProvider(monitorService);
      return new HServiceRootRunner(provider);
    }

    protected IHServicesProvider BuildServicesProvider(params IHService[] services)
    {
      var logger = BuildLogger();
      const string serviceName = "TestService";

      return new HServiceProvider(serviceName, logger, services);
    }

    protected IResultLogger BuildLogger()
    {
      return new NullLogger("TestLogger");
    }

    protected void AssertChangedValue(TestSetMonitorService service, int key, string value)
    {
      service.ChangeItem(key, value);
      service.WaitOnUpdate();

      TestMonitorItem item;
      Assert.IsNotNull(item = service.ChangedItem);
      Assert.AreEqual(item.Key, key);
      Assert.AreEqual(item.Value, value);
    }

    protected virtual void AssertIsFailed(IServiceStatusElement monitor)
    {
      Assert.IsTrue(monitor.IsFailed);
      Assert.AreEqual(monitor.Status, ServiceStatuses.Failed);
    }

    protected void AssertAddedValue(TestSetMonitorService service, int key, string value)
    {
      service.AddItem(key, value);
      service.WaitOnUpdate();

      Collection<TestMonitorItem> newItems;
      Assert.IsNotNull(newItems = service.NewItems);
      Assert.AreEqual(newItems.Count, 1);

      TestMonitorItem added;
      Assert.IsNotNull(added = newItems.FirstOrDefault());
      Assert.AreEqual(added.Key, key);
      Assert.AreEqual(added.Value, value);
    }

    protected void AssertRemovedValue(TestSetMonitorService service, int key)
    {
      var initialStatus = service.Status;
      service.RemoveItem(key);
      service.WaitOnUpdate();

      Collection<TestMonitorItem> removedItems;
      Assert.AreEqual(service.Status, initialStatus);
      Assert.IsNotNull(removedItems = service.RemovedItems);
      Assert.AreEqual(removedItems.Count, 1);

      TestMonitorItem removed;
      Assert.IsNotNull(removed = removedItems.FirstOrDefault());
      Assert.AreEqual(removed.Key, key);
    }
  }
}
