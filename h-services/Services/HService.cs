using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Services
{
  public abstract class HService : IHService
  {
    private bool _initialized;
    private ServiceStatuses _status;
    private readonly IServiceValidator _serviceValidator;

    protected IServiceValidator ServiceValidator
    { get { return _serviceValidator; } }

    protected HService(IServiceValidator serviceValidator)
    {
      _serviceValidator = serviceValidator;
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

    public ServiceStatuses Status { get { return _status; } }

    public event EventHandler<ServiceStatuses> StatusChanged;

    public event EventHandler<Result> ErrorOccured;

    protected abstract Result OnStart();

    protected abstract Result OnStop();

    protected abstract Result OnPause();

    protected abstract string ServiceName { get; }

    private Result StateChange(ServiceStatuses status, Func<Result> action, string success, string fail)
    {
      var currentStatus = Status;
      if (currentStatus == status)
        return Result.Success;

      Result change;
      if (!(change = CanTransitionTo(status)))
        return change;

      try
      {
        change += action();
      }
      catch (Exception e)
      {
        change += Result.Error(e);
      }

      return change + ((change)
        ? Result.SingleInfo(success, this)
        : Result.SingleError(fail, this));
    }

    protected Result SetRunning()
    {
      return TransitionStatus(ServiceStatuses.Started);
    }

    protected Result SetStopped()
    {
      return TransitionStatus(ServiceStatuses.Stopped);
    }

    protected Result SetPaused()
    {
      return TransitionStatus(ServiceStatuses.Paused);
    }

    private Result TransitionStatus(ServiceStatuses status)
    {
      Result transition;
      if (!(transition = CanTransitionTo(status)))
        return transition;

      return transition + SetStatus(status);
    }

    private Result CanTransitionTo(ServiceStatuses status)
    {
      var currentStatus = Status;
      var acceptedTransitions = ServiceValidator.GetStatusTransitions(status);

      return acceptedTransitions.Contains(currentStatus)
        ? Result.Success
        : Result.SingleError(Warnings.ServiceStatusTransitionNotAllowed, currentStatus, status);
    }

    private Result SetStatus(ServiceStatuses status)
    {
      if (_status == status)
        return Result.Success;

      try
      {
        _status = status;
        TriggerStatusChanged(status);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    private void TriggerStatusChanged(ServiceStatuses status)
    {
      if (StatusChanged != null)
        StatusChanged(this, status);
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
