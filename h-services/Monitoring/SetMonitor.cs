using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;
using Hylasoft.Services.Utilities;

namespace Hylasoft.Services.Monitoring
{
  public abstract class SetMonitor<TItem, TItemSpec> : ISetMonitor<TItem>
    where TItem : class
  {
    private readonly Result _userRequestedTransition;
    private readonly Dictionary<TItemSpec, TItem> _set;
    private readonly object _statusLock;
    private readonly ItemSetComparer<TItemSpec> _comparer;
    private readonly IMonitoringConfig _config;
    private ServiceStatuses _status;

    protected IDictionary<TItemSpec, TItem> Set { get { return _set; } }

    protected Thread RunThread { get; private set; }

    protected ItemSetComparer<TItemSpec> Comparer { get { return _comparer; } }

    protected IMonitoringConfig Config { get { return _config; } }

    protected Result UserRequestedTransition { get { return _userRequestedTransition; } }

    protected Result LastTransitionReason { get; private set; }

    #region ISetMonitor Implementation
    protected SetMonitor(IMonitoringConfig config = null)
    {
      _statusLock = new object();
      _comparer = new ItemSetComparer<TItemSpec>(AreItemsEqual, GetItemSpecHash);
      _config = config ?? new DefaultMonitoringConfig();

      _set = new Dictionary<TItemSpec, TItem>(Comparer);
      _userRequestedTransition = Result.SingleDebug(Debugs.UserRequestedTransition);

      // TODO: Consider an unknown reason.
      LastTransitionReason = UserRequestedTransition;
    }

    public Result Start()
    {
      if (IsRunning)
        return Result.SingleWarning(Warnings.MonitorIsAlreadyRunning, MonitorName);

      try
      {
        SetStatus(ServiceStatuses.Starting, UserRequestedTransition);

        Result start;
        if (!(start = UpdateSet()))
          return FailOut(start);

        RunThread = new Thread(MonitorLoop);
        RunThread.Start();

        // TODO: Timeout.
        while(!IsRunning)
          Thread.Sleep(Config.MonitorSleepInMilliseconds);

        return start;
      }
      catch (Exception e)
      {
        RunThread.Abort();
        return FailOut(Result.Error(e));
      }
    }

    public Result Stop()
    {
      if (IsStopped)
        return Result.SingleWarning(Warnings.MonitorIsAlreadyStopped, MonitorName);

      SetStatus(ServiceStatuses.Stopping, UserRequestedTransition);

      try
      {
        RunThread.Join(Config.AbortTimeoutInSeconds * 1000);
      }
      catch (Exception e)
      {
        RunThread.Abort();
        return FailOut(Result.Error(e));
      }
      finally
      {
        Set.Clear();
      }

      return Result.Success;
    }

    public ServiceStatuses Status
    {
      get { return GetStatus(); }
    }

    public bool IsRunning
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Started:
            return true;
        }

        return false;
      }
    }

    public bool IsStopped
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Stopping:
          case ServiceStatuses.Stopped:
          case ServiceStatuses.Failed:
            return true;
        }

        return false;
      }
    }

    public bool IsFailed
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Failed:
            return true;
        }

        return false;
      }
    }

    public event EventHandler<ServiceStatusTransition> StatusChanged;
    #endregion

    #region Abstract Methods
    public event EventHandler<TItem> ItemChanged;

    protected abstract Result FetchSet(out IEnumerable<TItem> items);

    protected abstract bool HasItemChanged(TItem oldItem, TItem newItem);

    protected abstract TItemSpec GetSpecification(TItem item);

    protected abstract string MonitorName { get; }

    protected abstract bool AreItemsEqual(TItemSpec a, TItemSpec b);

    protected abstract int GetItemSpecHash(TItemSpec spec);
    #endregion

    #region Threading
    protected void MonitorLoop()
    {
      if (Status != ServiceStatuses.Starting)
        return;

      SetStatus(ServiceStatuses.Started, LastTransitionReason);
      while (IsRunning)
      {
        Result update;
        // TODO: Consider a re-try mechanism.
        if (!(update = UpdateSet()))
          FailOut(update);

        Thread.Sleep(Config.MonitorSleepInMilliseconds);
      }

      if (!IsFailed)
        SetStatus(ServiceStatuses.Stopped, LastTransitionReason);
    }

    private void SetStatus(ServiceStatuses status, Result reason)
    {
      var hasChanged = false;
      ServiceStatuses oldStatus;
      lock (_statusLock)
      {
        if ((oldStatus = _status) != status)
        {
          _status = status;
          hasChanged = true;
        }
      }

      // Drop out gracefully, if nothing has changed.
      if (!hasChanged) return;
      LastTransitionReason = reason;
      TriggerStatusChanged(oldStatus, Status, reason);
    }

    private ServiceStatuses GetStatus()
    {
      ServiceStatuses status;
      lock (_statusLock)
      {
        status = _status;
      }

      return status;
    }
    #endregion

    #region Helper Methods
    protected Result UpdateSet()
    {
      Result update;
      IEnumerable<TItem> currentItems;
      if (!(update = FetchSet(out currentItems)) || currentItems == null)
        return update;

      var currentSet = currentItems.ToArray();
      if (!(update += Result.Concat(UpdateItem, currentSet)))
        return update;

      return update + ClearMissingItems(currentSet);
    }

    protected Result UpdateItem(TItem item)
    {
      // Don't bother with a null item.
      if (item == null)
        return Result.Success;

      var specification = GetSpecification(item);

      // If the item doesn't exist yet, simply add it.
      if (!Set.ContainsKey(specification))
        return AddItem(specification, item);

      var existingItem = Set[specification];
      // Update if existing item has changed.
      return HasItemChanged(existingItem, item)
        ? UpdateItem(specification, item)
        : Result.Success;
    }

    protected Result AddItem(TItemSpec spec, TItem item)
    {
      try
      {
        Set.Add(spec, item);

        // TODO: Consider adding a trace statement.
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result UpdateItem(TItemSpec spec, TItem item)
    {
      try
      {
        Set[spec] = item;
        TriggerItemChanged(item);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result ClearMissingItems(IEnumerable<TItem> items)
    {
      try
      {
        var currentItems = items.Select(GetSpecification);
        var missingItems = Set.Keys.Except(currentItems);

        foreach (var missingItem in missingItems)
          Set.Remove(missingItem);

        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    private void TriggerStatusChanged(ServiceStatuses oldStatus, ServiceStatuses newStatus, Result reason)
    {
      if (StatusChanged != null)
        StatusChanged(this, new ServiceStatusTransition(oldStatus, newStatus, reason));
    }

    private void TriggerItemChanged(TItem item)
    {
      if (ItemChanged != null)
        ItemChanged(this, item);
    }

    protected Result FailOut(Result reason)
    {
      var newStatus = reason
        ? ServiceStatuses.Stopped
        : ServiceStatuses.Failed;

      SetStatus(newStatus, reason);
      return reason;
    }
    #endregion
  }
}
