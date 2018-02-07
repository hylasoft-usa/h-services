using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;
using Hylasoft.Services.Validation;

namespace Hylasoft.Services.Service
{
  public abstract class HService : HServiceStatusBase, IHService
  {
    private bool _initialized;
    private ServiceStatuses _status;
    private readonly IServiceValidator _serviceValidator;

    protected IServiceValidator ServiceValidator { get { return _serviceValidator; } }

    protected HService(IServiceValidator serviceValidator = null)
    {
      _serviceValidator = serviceValidator ?? new ServiceValidator();
      _status = ServiceStatuses.Stopped;
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

    protected abstract Result InitalizeService();
    
    public event EventHandler<ServiceStatusTransition> StatusChanged;

    public event EventHandler<Result> ErrorOccured;

    protected abstract Result OnStart();

    protected abstract Result OnStop();

    protected abstract Result OnPause();

    protected abstract string ServiceName { get; }

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

    protected Result SetRunning(Result reason)
    {
      return TransitionStatus(ServiceStatuses.Started, reason);
    }

    protected Result SetStopped(Result reason)
    {
      return TransitionStatus(ServiceStatuses.Stopped, reason);
    }

    protected Result SetPaused(Result reason)
    {
      return TransitionStatus(ServiceStatuses.Paused, reason);
    }

    private Result TransitionStatus(ServiceStatuses status, Result reason)
    {
      Result transition;
      if (!(transition = CanTransitionTo(status)))
        return transition;

      return transition + SetStatus(status, reason);
    }

    private Result CanTransitionTo(ServiceStatuses status)
    {
      var currentStatus = Status;
      var acceptedTransitions = ServiceValidator.GetStatusTransitions(status);

      return acceptedTransitions.Contains(currentStatus)
        ? Result.Success
        : Result.SingleError(Warnings.ServiceStatusTransitionNotAllowed, currentStatus, status);
    }

    private Result SetStatus(ServiceStatuses status, Result reason)
    {
      ServiceStatuses oldStatus;
      if ((oldStatus = _status) == status)
        return Result.Success;

      try
      {
        _status = status;
        TriggerStatusChanged(oldStatus, status, reason);
        return Result.Success;
      }
      catch (Exception e)
      {
        return reason + Result.Error(e);
      }
    }

    private void TriggerStatusChanged(ServiceStatuses oldStatus, ServiceStatuses newStatus, Result reason)
    {
      if (StatusChanged != null)
        StatusChanged(this, new ServiceStatusTransition(oldStatus, newStatus, reason));
    }

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
