using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;
using Hylasoft.Services.Validation;

namespace Hylasoft.Services.Services.Base
{
  public abstract class HService : HServiceStatusBase, IHService
  {
    private bool _initialized;


    protected HService(IServiceValidator serviceValidator = null)
    {
      _initialized = false;
    }

    public Result Initialize()
    {
      // TODO: Add a warning.
      if (_initialized)
        return Result.SingleWarning(Warnings.ServiceAlreadyInitialized, this);

      var init = Result.Success;
      try
      {
        init += InitalizeService();
        _initialized = init;
      }
      catch (Exception e)
      {
        init += Result.Error(e);
        _initialized = false;
      }

      return init + (init
        ? Result.SingleInfo(Messages.ServiceInitialized, this)
        : Result.SingleError(Errors.ServiceInitializationFailed, this));
    }

    public Result Start()
    {
      return StateChange(ServiceStatuses.Started, OnStart, Messages.ServiceStarted, Errors.ServiceStartFailed);
    }

    public Result Stop()
    {
      return StateChange(ServiceStatuses.Stopped, OnStop, Messages.ServiceStopped, Errors.ServiceStopFailed);
    }

    public Result Pause()
    {
      return StateChange(ServiceStatuses.Paused, OnPause, Messages.ServicePaused, Errors.ServicePauseFailed);
    }

    public Result Restart()
    {
      return Result.ConcatRestricted(Stop, Start);
    }

    private Result StateChange(ServiceStatuses status, Func<Result> action, string success, string fail, Result reason = null)
    {
      reason = reason ?? UserRequestedTransition;
      var currentStatus = Status;
      if (currentStatus == status)
        return Result.Success;

      Result change;
      if (!(change = CanTransitionTo(status)))
        return change;

      try
      {
        change += action();
        TransitionStatus(status, reason);
      }
      catch (Exception e)
      {
        change += Result.Error(e);
        TransitionStatus(ServiceStatuses.Failed, change);
      }

      return change + ((change)
        ? Result.SingleInfo(success, this)
        : Result.SingleError(fail, this));
    }

    protected abstract Result InitalizeService();
    
    public event EventHandler<ServiceStatusTransition> StatusChanged;

    public event EventHandler<Result> ErrorOccured;

    protected abstract Result OnStart();

    protected abstract Result OnStop();

    protected abstract Result OnPause();
    
    public abstract string ServiceName { get; }
    
    

    protected void TriggerErrorOccured(Result error)
    {
      if (ErrorOccured != null && !error) ErrorOccured(this, error);
    }

    public override string ToString()
    {
      return string.Format("{0} Service", ServiceName);
    }
  }
}
