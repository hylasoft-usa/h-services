using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;
using Hylasoft.Services.Validation;

namespace Hylasoft.Services.Services.Base
{
  public abstract class HServiceStatusBase : IServiceStatusElement
  {
    private readonly Result _userRequestedTransition;
    private readonly IServiceValidator _serviceValidator;
    private readonly object _statusTransitionLock = new object();
    private readonly object _initializationLock = new object();
    private readonly object _statusLock = new object();

    private ServiceStatuses _status;

    protected Result UserRequestedTransition { get { return _userRequestedTransition; } }

    protected bool IsInitialized { get; private set; }

    protected IServiceValidator ServiceValidator { get { return _serviceValidator; } }

    protected HServiceStatusBase(IServiceValidator serviceValidator = null)
    {
      _status = ServiceStatuses.Stopped;
      _serviceValidator = serviceValidator ?? new ServiceValidator();
      _userRequestedTransition = Result.SingleDebug(Debugs.UserRequestedTransition);
      IsInitialized = false;
      LastTransitionReason = UserRequestedTransition;
    }

    #region IServiceStatusElement Implementation

    public ServiceStatuses Status
    {
      get
      {
        lock (_statusLock)
          return _status;
      }

      set
      {
        lock (_statusLock)
          _status = value;
      }
    }

    public Result Initialize(Result reason = null)
    {
      try
      {
        lock (_initializationLock)
          return LockedInitialize();
      }
      catch (Exception e)
      {
        return ErrorOut(e);
      }
    }

    public Result Start(Result reason = null)
    {
      try
      {
        lock (_statusTransitionLock)
          return LockedStart(reason);
      }
      catch (Exception e)
      {
        return ErrorOut(e);
      }
    }

    public Result Stop(Result reason = null)
    {
      try
      {
        lock (_statusTransitionLock)
          return LockedStop(reason);
      }
      catch (Exception e)
      {
        return ErrorOut(e);
      }
    }

    public Result Pause(Result reason = null)
    {
      try
      {
        lock (_statusTransitionLock)
          return LockedPause(reason);
      }
      catch (Exception e)
      {
        return ErrorOut(e);
      }
    }

    public Result Restart(Result reason = null)
    {
      try
      {
        lock (_statusTransitionLock)
        {
          Result restart;
          if (!(restart = LockedStop(reason)))
            return ErrorOut(restart);

          return restart + LockedStart(reason);
        }
      }
      catch (Exception e)
      {
        return ErrorOut(e);
      }
    }

    public event EventHandler<ServiceStatusTransition> StatusChanged;

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
          case ServiceStatuses.Paused:
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

    public bool IsPaused
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Paused:
            return true;
        }

        return false;
      }
    }

    public Result LastTransitionReason { get; private set; }
    #endregion

    #region Domain Methods
    private Result LockedStart(Result reason)
    {
      var start = Result.Success;
      if (!IsInitialized && !(start += Initialize()))
        return start;

      return start + LockedMajorTransition(ServiceStatuses.Starting, ServiceStatuses.Started, StartService,
        IsRunning, Warnings.MonitorIsAlreadyRunning, reason);
    }

    private Result LockedStop(Result reason)
    {
      return LockedMajorTransition(ServiceStatuses.Stopping, ServiceStatuses.Stopped, StopService,
        IsStopped, Warnings.MonitorIsAlreadyStopped, reason);
    }

    private Result LockedPause(Result reason)
    {
      if (!IsRunning)
        return Result.SingleWarning(Warnings.MonitorIsAlreadyStopped, ServiceName);

      Result pause;
      if (!(pause = PauseService()))
        return ErrorOut(pause);
      
      if (Status != ServiceStatuses.Paused)
        pause += TransitionStatus(ServiceStatuses.Paused, reason);

      return pause;
    }

    private Result LockedInitialize()
    {
      if (IsInitialized)
        return Result.SingleWarning(Warnings.ServiceAlreadyInitialized, ServiceName);

      Result init;
      if (!(init = InitializeService()))
        return ErrorOut(init);

      IsInitialized = true;
      return init;
    }

    private Result LockedMajorTransition(ServiceStatuses initialTransition, ServiceStatuses finalTransition,
      Func<Result> serviceCall, bool statusCheck, string statusWarning, Result reason = null)
    {
      if (statusCheck)
        return Result.SingleWarning(statusWarning, ServiceName);

      Result transition;
      if (!(transition = TransitionStatus(initialTransition, reason)))
        return ErrorOut(transition);

      if (!(transition += serviceCall()))
        return ErrorOut(transition);

      if (Status != finalTransition)
        transition += TransitionStatus(finalTransition, reason);

      return transition;
    }
    #endregion

    #region Abstract Members
    protected abstract Result InitializeService();

    protected abstract Result StartService();

    protected abstract Result StopService();

    protected abstract Result PauseService();

    public abstract string ServiceName { get; }
    #endregion

    #region Status Methods
    protected internal Result SetRunning(Result reason = null)
    {
      return TransitionStatus(ServiceStatuses.Started, reason);
    }

    protected Result SetStopped(Result reason = null)
    {
      return TransitionStatus(ServiceStatuses.Stopped, reason);
    }

    protected Result SetPaused(Result reason = null)
    {
      return TransitionStatus(ServiceStatuses.Paused, reason);
    }

    private Result TransitionStatus(ServiceStatuses status, Result reason = null)
    {
      Result transition;
      reason = reason ?? UserRequestedTransition;
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
      if ((oldStatus = Status) == status)
        return Result.Success;

      try
      {
        _status = status;
        TriggerStatusChanged(oldStatus, Status, reason);
        LastTransitionReason = reason;
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
    #endregion

    #region Helper Methods
    protected Result ErrorOut(Exception e)
    {
      return ErrorOut(Result.Error(e));
    }

    protected Result ErrorOut(Result error)
    {
      try
      {
        var status = error
          ? ServiceStatuses.Stopped
          : ServiceStatuses.Failed;

        error += TransitionStatus(status, error);
      }
      catch (Exception transitionFailed)
      {
        error += Result.Error(transitionFailed);
      }

      return error;
    }
    #endregion
  }
}
