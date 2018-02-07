using System;
using System.Threading;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Service;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Monitoring
{
  public abstract class HMonitor : HServiceStatusBase, IMonitor
  {
    private ServiceStatuses _status;
    private readonly object _statusLock;
    private readonly IMonitoringConfig _config;

    protected IMonitoringConfig Config { get { return _config; } }

    protected Result LastTransitionReason { get; private set; }

    protected Thread RunThread { get; private set; }

    protected HMonitor(IMonitoringConfig config = null)
    {
      _config = config;
      _statusLock = new object();
      _config = config ?? new DefaultMonitoringConfig();

      // TODO: Consider an unknown reason.
      LastTransitionReason = UserRequestedTransition;
    }

    public Result Initialize()
    {
      try
      {
        return InitializeService();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    public Result Start()
    {
      if (IsRunning)
        return Result.SingleWarning(Warnings.MonitorIsAlreadyRunning, ServiceName);

      try
      {
        SetStatus(ServiceStatuses.Starting, UserRequestedTransition);

        Result start;
        if (!(start = InitializeOnStartup()))
          return FailOut(start);

        RunThread = new Thread(MonitorLoop);
        RunThread.Start();

        // TODO: Timeout.
        while (!IsRunning)
          Thread.Sleep(Config.MonitorSleepInMilliseconds);

        return start;
      }
      catch (Exception e)
      {
        RunThread.Abort();
        return FailOut(Result.Error(e));
      }
    }

    #region IService Implementation
    public Result Stop()
    {
      if (IsStopped)
        return Result.SingleWarning(Warnings.MonitorIsAlreadyStopped, ServiceName);

      SetStatus(ServiceStatuses.Stopping, UserRequestedTransition);

      try
      {
        RunThread.Join(Config.AbortTimeoutInSeconds * 1000);
      }
      catch (Exception e)
      {
        RunThread.Abort();
        return CleanupOnShutdown() + FailOut(Result.Error(e));
      }

      return CleanupOnShutdown();
    }

    public Result Pause()
    {
      return SetStatus(ServiceStatuses.Paused, UserRequestedTransition);
    }

    public Result Restart()
    {
      return Result.ConcatRestricted(Stop, Start);
    }

    public override ServiceStatuses Status
    {
      get { return GetStatus(); }
    }

    public event EventHandler<ServiceStatusTransition> StatusChanged;
    public event EventHandler<Result> ErrorOccured;

    #endregion

    #region Abstract Members
    protected abstract string ServiceName { get; }

    protected abstract Result InitializeService();

    protected abstract Result PerformServiceLoop();

    protected abstract Result InitializeOnStartup();

    protected abstract Result CleanupOnShutdown();
    #endregion

    #region Threading
    protected void MonitorLoop()
    {
      if (Status != ServiceStatuses.Starting)
        return;

      SetStatus(ServiceStatuses.Started, LastTransitionReason);
      while (IsRunning || IsPaused)
      {
        Result loop;
        // TODO: Consider a re-try mechanism.
        if (IsRunning && !(loop = PerformServiceLoop()))
          FailOut(loop);

        Thread.Sleep(Config.MonitorSleepInMilliseconds);
      }

      if (!IsFailed)
        SetStatus(ServiceStatuses.Stopped, LastTransitionReason);
    }

    private Result SetStatus(ServiceStatuses status, Result reason)
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

      if (hasChanged)
      {
        LastTransitionReason = reason;
        TriggerStatusChanged(oldStatus, Status, reason);
      }

      return reason;
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
    private void TriggerStatusChanged(ServiceStatuses oldStatus, ServiceStatuses newStatus, Result reason)
    {
      if (StatusChanged != null)
        StatusChanged(this, new ServiceStatusTransition(oldStatus, newStatus, reason));
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
