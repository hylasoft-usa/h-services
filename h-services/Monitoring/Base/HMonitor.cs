using System;
using System.Threading;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Monitoring.Base
{
  public abstract class HMonitor : HServiceStatusBase, IMonitor
  {
    private readonly IMonitoringConfig _config;

    protected IMonitoringConfig Config { get { return _config; } }


    protected Thread RunThread { get; private set; }

    protected HMonitor(IMonitoringConfig config = null, IServiceValidator serviceValidator = null) : base(serviceValidator)
    {
      _config = config ?? new DefaultMonitoringConfig();
    }

    protected override Result InitializeService()
    {
      return OnInitialize();
    }

    protected override Result StartService()
    {
      try
      {
        RunThread = new Thread(MonitorLoop);
        RunThread.Start();
      }
      catch (Exception e)
      {
        RunThread.Abort();
        return Result.Error(e);        
      }

      var tick = 0;
      var timeout = (Config.AbortTimeoutInSeconds * 1000) / Config.MonitorSleepInMilliseconds;
      while (!IsRunning && !IsFailed && tick++ <= timeout)
        Thread.Sleep(Config.MonitorSleepInMilliseconds);

      return (tick >= timeout)
        ? Result.SingleError(Errors.TimedOutWaitingOnStart, ServiceName)
        : Result.Success;
    }

    protected override Result StopService()
    {
      try
      {
        SetStopped();
        RunThread.Join(Config.AbortTimeoutInSeconds * 1000);
      }
      catch (Exception e)
      {
        RunThread.Abort();
        return CleanupOnShutdown() + ErrorOut(Result.Error(e));
      }

      return CleanupOnShutdown();
    }

    protected override Result PauseService()
    {
      return Result.Success;
    }

    #region IService Implementation
    public event EventHandler<Result> ErrorOccured;

    protected void RaiseError(Result error)
    {
      if (!error) TriggerErrorOccured(error);
    }

    protected Result ExecServiceLoop()
    {
      try
      {
        return PerformServiceLoop();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }
    #endregion

    #region Abstract Members
    protected abstract Result OnInitialize();

    protected abstract Result PerformServiceLoop();

    protected abstract Result CleanupOnShutdown();
    #endregion

    #region Threading
    protected void MonitorLoop()
    {
      if (Status != ServiceStatuses.Starting)
        return;

      var loop = Result.Success;
      SetRunning(LastTransitionReason);
      while (IsRunning || IsPaused)
      {
        // TODO: Consider a re-try mechanism.
        if (IsRunning && !(loop = ExecServiceLoop()))
          ErrorOut(loop);

        Thread.Sleep(Config.MonitorSleepInMilliseconds);
      }

      if (!IsFailed)
        SetStopped(loop);
    }
    #endregion

    #region Helper Methods
    private void TriggerErrorOccured(Result error)
    {
      if (ErrorOccured != null)
        ErrorOccured(this, error);
    }
    #endregion
  }
}
