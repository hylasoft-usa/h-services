﻿using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Resources;

namespace Hylasoft.Services.Services.Base
{
  public abstract class HService : HServiceStatusBase, IHService
  {
    protected HService(IServiceValidator serviceValidator = null) : base(serviceValidator)
    {
    }
    
    protected override Result InitializeService()
    {
      return TransitionService(OnInitialize, Messages.ServiceInitialized, Errors.ServiceInitializationFailed);
    }

    protected override Result StartService()
    {
      return TransitionService(OnStart, Messages.ServiceStarted, Errors.ServiceStartFailed);
    }

    protected override Result StopService()
    {
      return TransitionService(OnStop, Messages.ServiceStopped, Errors.ServiceStopFailed);
    }

    protected override Result PauseService()
    {
      return TransitionService(OnPause, Messages.ServicePaused, Errors.ServicePauseFailed);
    }

    private Result TransitionService(Func<Result> serviceCall, string onSuccess, string onFailure)
    {
      var transition = Result.Success;
      try
      {
        transition += serviceCall();
      }
      catch (Exception e)
      {
        transition += Result.Error(e);
      }

      return transition + (transition
        ? Result.SingleInfo(onSuccess, this)
        : Result.SingleError(onFailure, this));
    }

    protected abstract Result OnInitialize();

    protected abstract Result OnStart();

    protected abstract Result OnStop();

    protected abstract Result OnPause();

    public override string ToString()
    {
      return string.Format("{0} Service", ServiceName);
    }
  }
}
