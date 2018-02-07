﻿using System.Collections.Generic;
using System.Threading;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Tests.Types;
using Hylasoft.Services.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class ServiceTests : TestBase
  {
    private readonly TestSetMonitor _testMonitor;

    private readonly KeyValuePair<int, string>[] _initialValues =
    {
      new KeyValuePair<int, string>(1, "Test01"),
      new KeyValuePair<int, string>(2, "Test02"),
      new KeyValuePair<int, string>(3, "Test03") 
    };

    protected TestSetMonitor TestMonitor { get { return _testMonitor; } }

    protected KeyValuePair<int, string>[] InitialValues { get { return _initialValues; } }

    protected ServiceStatusTransition LastTransition { get; private set; }

    protected TestMonitorItem LastChange { get; private set; }

    protected bool WaitingOnChange { get; private set; }

    protected int TransitionCount { get; private set; }

    protected int ChangeCount { get; private set; }

    public ServiceTests()
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
    }

    [TestMethod]
    public void TestSetMonitorStartAndStop()
    {
      var monitor = TestMonitor;

      Assert.AreEqual(TransitionCount, 0);

      Assert.IsTrue(monitor.Start());
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

      Assert.IsTrue(monitor.Start());
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

      Assert.IsTrue(TestMonitor.Start());
      Assert.AreEqual(ChangeCount, 0);
      
      AssertValueChange(TestMonitor, key, changedValue);
      Assert.AreEqual(ChangeCount, 1);

      AssertValueChange(TestMonitor, key, value);
      Assert.AreEqual(ChangeCount, 2);

      // Setting the value to itself.
      TestMonitor.InnerSet[key] = value;
      Thread.Sleep(1000);
      Assert.AreEqual(ChangeCount, 2);

      Assert.IsTrue(TestMonitor.Stop());
    }

    [TestMethod]
    public void TestSetMonitorFailureOnStartUp()
    {
      var failMonitor = BuildFailMonitor(true);
      Assert.IsFalse(failMonitor.Start());

      // Expected Starting, Failed
      Assert.AreEqual(TransitionCount, 2);
      AssertIsFailed(failMonitor);
    }

    [TestMethod]
    public void TestSetMonitorFailureDuringRun()
    {
      var transCount = 0;
      var failMonitor = BuildFailMonitor(false);
      Assert.IsTrue(failMonitor.Start());
      Assert.IsTrue(failMonitor.Stop());

      // Starting, Started, Stopping, Stopped.
      Assert.AreEqual(TransitionCount, transCount += 4);
      Assert.IsTrue(failMonitor.Start());

      // + Starting, Started
      Assert.AreEqual(TransitionCount, transCount += 2);
      AssertValueChange(failMonitor, 2, "Changed02");

      failMonitor.ShouldFail = true;
      Thread.Sleep(1000);

      // + Failed
      Assert.AreEqual(TransitionCount, transCount += 1);
      AssertIsFailed(failMonitor);

      Assert.IsFalse(failMonitor.Start());
      // + Starting, Failed
      Assert.AreEqual(TransitionCount, transCount += 2);
      AssertIsFailed(failMonitor);

      failMonitor.ShouldFail = false;
      Assert.IsTrue(failMonitor.Start());
      // + Starting, Started
      Assert.AreEqual(TransitionCount, transCount += 2);
      
      AssertValueChange(failMonitor, 1, "Changed01");
      Assert.IsTrue(failMonitor.Stop());

      // + Stopping, Stopped
      Assert.AreEqual(TransitionCount, transCount + 2);
      Assert.AreEqual(failMonitor.Status, ServiceStatuses.Stopped);
      Assert.IsTrue(LastTransition.Reason);
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
      WaitingOnChange = false;
    }
    #endregion

    #region Helpers
    protected void AssertValueChange(TestSetMonitor monitor, int key, string value)
    {
      Assert.AreNotEqual(monitor.InnerSet[key], value);

      WaitingOnChange = true;
      monitor.InnerSet[key] = value;

      while(WaitingOnChange)
        Thread.Sleep(500);

      Assert.IsNotNull(LastChange);
      Assert.AreEqual(LastChange.Key, key);
      Assert.AreEqual(LastChange.Value, value);
    }

    protected void AssertIsFailed(IMonitor monitor)
    {
      Assert.IsTrue(monitor.IsFailed);
      Assert.AreEqual(monitor.Status, ServiceStatuses.Failed);
      Assert.IsFalse(LastTransition.Reason);
    }

    protected void InitializeSet(TestSetMonitor monitor)
    {
      monitor.InnerSet.Clear();
      foreach (var value in InitialValues)
        monitor.InnerSet.Add(value.Key, value.Value);
    }

    protected TMonitor BuildMonitor<TMonitor>()
      where TMonitor : TestSetMonitor, new()
    {
      var monitor = new TMonitor();

      monitor.StatusChanged += OnMonitorTransition;
      monitor.ItemChanged += OnItemChanged;

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