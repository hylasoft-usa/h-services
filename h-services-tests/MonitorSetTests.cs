﻿using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Tests.Base;
using Hylasoft.Services.Tests.Types.MonitorSets;
using Hylasoft.Services.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class MonitorSetTests : SetMonitorTestBase
  {
    private readonly TestSetMonitor _testMonitor;

    protected TestSetMonitor TestMonitor { get { return _testMonitor; } }

    protected ServiceStatusTransition LastTransition { get; private set; }

    protected TestMonitorItem LastChange { get; private set; }

    protected Collection<TestMonitorItem> LastAdded { get; private set; }

    protected Collection<TestMonitorItem> LastRemoved { get; private set; } 

    protected int TransitionCount { get; private set; }

    protected int ChangeCount { get; private set; }

    protected int AddCount { get; private set; }

    protected int RemovedCount { get; private set; }

    public MonitorSetTests()
    {
      _testMonitor = BuildMonitor<TestSetMonitor>();
    }

    [TestInitialize]
    public void Initialize()
    {
      InitializeSet(TestMonitor);

      TransitionCount = 0;
      LastTransition = null;
      
      ChangeCount = 0;
      LastChange = null;

      AddCount = 0;
      LastAdded = null;

      RemovedCount = 0;
      LastRemoved = null;
    }

    [TestMethod]
    public void TestSetMonitorStartAndStop()
    {
      var monitor = TestMonitor;

      Assert.AreEqual(TransitionCount, 0);

      AssertStartup(monitor);
      Assert.IsTrue(monitor.Stop());

      // Starting, Started, Stopping, Stopped.
      Assert.AreEqual(TransitionCount, 4);
    }

    [TestMethod]
    public void TestSetMonitorValueChange()
    {
      const int key = 2;
      const string initValue = "Changed02";
      const string changeValue = "ChangedAgain02";

      var monitor = TestMonitor;
      var set = monitor.InnerSet;

      Assert.IsNull(LastChange);
      set[key] = initValue;
      Assert.IsNull(LastChange);

      AssertStartup(monitor);
      Assert.AreEqual(ChangeCount, 0);
      AssertValueChange(monitor, key, changeValue);
      AssertValueChange(monitor, key, initValue);
      Assert.AreEqual(ChangeCount, 2);
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestSetMonitorProtectionFromChange()
    {
      const int valueIndex = 2;
      var initialValue = InitialValues[valueIndex];
      var key = initialValue.Key;
      var value = initialValue.Value;

      const string changedValue = "ChangedValue";

      AssertStartup(TestMonitor);
      Assert.AreEqual(ChangeCount, 0);
      
      AssertValueChange(TestMonitor, key, changedValue);
      Assert.AreEqual(ChangeCount, 1);

      AssertValueChange(TestMonitor, key, value);
      Assert.AreEqual(ChangeCount, 2);

      // Setting the value to itself.
      TestMonitor.InnerSet[key] = value;
      TestMonitor.WaitOnUpdate();
      Assert.AreEqual(ChangeCount, 2);

      Assert.IsTrue(TestMonitor.Stop());
    }

    [TestMethod]
    public void TestSetMonitorFailureOnStartUp()
    {
      var failMonitor = BuildFailMonitor(true);
      AssertStartup(failMonitor, false);

      // Expected Failed
      Assert.AreEqual(TransitionCount, 1);
      AssertIsFailed(failMonitor);
    }

    [TestMethod]
    public void TestSetMonitorFailureDuringRun()
    {
      var transCount = 0;
      var failMonitor = BuildFailMonitor(false);
      AssertStartup(failMonitor);
      Assert.IsTrue(failMonitor.Stop());

      // Starting, Started, Stopping, Stopped.
      Assert.AreEqual(TransitionCount, transCount += 4);
      AssertStartup(failMonitor);

      // + Starting, Started
      Assert.AreEqual(TransitionCount, transCount += 2);
      AssertValueChange(failMonitor, 2, "Changed02");

      failMonitor.ShouldFail = true;
      failMonitor.WaitOnUpdate();

      // + Failed
      Assert.AreEqual(TransitionCount, transCount += 1);
      AssertIsFailed(failMonitor);

      AssertStartup(failMonitor, false);
      // + Starting, Started, Failed
      Assert.AreEqual(TransitionCount, transCount += 3);
      AssertIsFailed(failMonitor);

      failMonitor.ShouldFail = false;
      AssertStartup(failMonitor);
      // + Starting, Started
      Assert.AreEqual(TransitionCount, transCount += 2);
      
      AssertValueChange(failMonitor, 1, "Changed01");
      Assert.IsTrue(failMonitor.Stop());

      // + Stopping, Stopped
      Assert.AreEqual(TransitionCount, transCount + 2);
      Assert.AreEqual(failMonitor.Status, ServiceStatuses.Stopped);
      Assert.IsTrue(LastTransition.Reason);
    }

    [TestMethod]
    public void TestNewlyAddedSetMonitorValues()
    {
      var monitor = TestMonitor;

      const int newKey = 12;
      const string newInitValue = "Value12";
      const string newChangedValue = "Changed12";

      AssertStartup(monitor);
      AssertValueAdd(monitor, newKey, newInitValue);
      AssertValueChange(monitor, newKey, newChangedValue);

      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestRemovedSetMonitorValues()
    {
      var monitor = TestMonitor;

      var valueToRemove = InitialValues.FirstOrDefault();
      Assert.IsNotNull(valueToRemove);
      AssertStartup(monitor);
      AssertValueRemove(monitor, valueToRemove.Key);

      const int newKey = 5;
      const string newVal = "Value5";
      const string newValChanged = "ValueChanged5";

      AssertValueAdd(monitor, newKey, newVal);
      AssertValueChange(monitor, newKey, newValChanged);

      Assert.IsTrue(monitor.Stop());
    }

    #region Handlers
    protected void OnMonitorTransition(object sender, ServiceStatusTransition transition)
    {
      LastTransition = transition;
      ++TransitionCount;
    }

    protected void OnItemChanged(object sender, TestMonitorItem testItem)
    {
      LastChange = testItem;
      ++ChangeCount;
    }

    protected void OnItemsAdded(object sender, Collection<TestMonitorItem> newItems)
    {
      LastAdded = newItems;
      ++AddCount;
    }

    protected void OnItemsRemoved(object sender, Collection<TestMonitorItem> removedItems)
    {
      LastRemoved = removedItems;
      ++RemovedCount;
    }
    #endregion

    #region Helpers

    protected void AssertStartup(TestSetMonitor monitor, bool success = true)
    {
      Assert.IsNotNull(monitor);
      
      if (!success)
      {
        Result startup;
        if (!(startup = monitor.Start()))
          return;
        
        monitor.WaitOnUpdate();
        AssertIsFailed(monitor);
        return;
      }

      Assert.IsTrue(monitor.Start());
      monitor.WaitOnUpdate();
    }

    protected void AssertValueChange(TestSetMonitor monitor, int key, string value)
    {
      Assert.AreNotEqual(monitor.InnerSet[key], value);
      monitor.InnerSet[key] = value;
      monitor.WaitOnUpdate();

      Assert.IsNotNull(LastChange);
      Assert.AreEqual(LastChange.Key, key);
      Assert.AreEqual(LastChange.Value, value);
    }

    protected void AssertValueAdd(TestSetMonitor monitor, int key, string value)
    {
      Assert.IsNotNull(monitor);
      var innerSet = monitor.InnerSet;
      Assert.IsFalse(innerSet.ContainsKey(key));

      innerSet.Add(key, value);
      monitor.WaitOnUpdate();

      Assert.IsNotNull(LastAdded);
      Assert.AreEqual(LastAdded.Count, 1);

      TestMonitorItem added;
      Assert.IsNotNull(added = LastAdded.FirstOrDefault());
      Assert.AreEqual(added.Key, key);
      Assert.AreEqual(added.Value, value);
    }

    protected override void AssertIsFailed(IServiceStatusElement monitor)
    {
      base.AssertIsFailed(monitor);
      Assert.IsFalse(LastTransition.Reason);
    }

    protected void AssertValueRemove(TestSetMonitor monitor, int key)
    {
      var rc = RemovedCount;
      Assert.IsNotNull(monitor);
      var initialStatus = monitor.Status;

      var innerSet = monitor.InnerSet;
      Assert.IsTrue(innerSet.ContainsKey(key));

      innerSet.Remove(key);
      monitor.WaitOnUpdate();
      Assert.AreEqual(monitor.Status, initialStatus);
      Assert.IsNotNull(LastRemoved);
      Assert.AreEqual(LastRemoved.Count, 1);
      Assert.AreEqual(LastRemoved.FirstOrDefault().Key, key);
      Assert.AreEqual(RemovedCount, ++rc);
    }

    protected TMonitor BuildMonitor<TMonitor>()
      where TMonitor : TestSetMonitor, new()
    {
      var monitor = new TMonitor();

      monitor.StatusChanged += OnMonitorTransition;
      monitor.ItemChanged += OnItemChanged;
      monitor.ItemsAdded += OnItemsAdded;
      monitor.ItemsRemoved += OnItemsRemoved;

      return monitor;
    }

    protected FailedSetMonitor BuildFailMonitor(bool shouldFail)
    {
      var failMonitor = BuildMonitor<FailedSetMonitor>();
      InitializeSet(failMonitor);
      failMonitor.ShouldFail = shouldFail;

      return failMonitor;
    }
    #endregion
  }
}
