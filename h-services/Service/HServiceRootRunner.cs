﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Extensions;
using Hylasoft.Logging;
using Hylasoft.Resolution;
using Hylasoft.Services.Constants;
using Hylasoft.Services.Interfaces.Providers;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Service
{
  public class HServiceRootRunner : IHServiceRootRunner
  {
    private readonly Result _initializedByStartup;
    private readonly Result _setByParentService;
    private readonly IHServicesProvider _provider;
    private readonly Collection<IHService> _services;
    private readonly IResultLogger _logger;
    
    protected Collection<IHService> Services { get { return _services; } }

    protected IResultLogger Logger { get { return _logger; } }

    protected IHServicesProvider ServicesProvider { get { return _provider; } }

    protected bool IsInitialized { get; private set; }

    protected Result InitializedByStartup { get { return _initializedByStartup; } }

    protected Result SetByParentService { get { return _setByParentService; } }

    public HServiceRootRunner(IHServicesProvider provider)
    {
      _provider = provider;
      _services = ServicesProvider.Services;
      _logger = ServicesProvider.Logger;
      _initializedByStartup = Result.SingleInfo(ServiceReasons.InitializedByStartup, Debugs.InitializedByStartup);
      _setByParentService = Result.SingleInfo(ServiceReasons.SetByParentService, Debugs.SetByParentService);

      IsInitialized = false;
    }

    public void OnStart(string[] args)
    {
      bool wasInitialized;
      if (!(wasInitialized = IsInitialized))
      {
        InitializeService();
        IsInitialized = true;
      }

      // Don't try to start a service that failed initialization.
      var condition = wasInitialized
        ? Always
        : (Func<IHService, bool>) (s => s != null && !s.IsFailed);

      ServiceAction(service => service.Start, InitializedByStartup, condition);
    }

    public void OnStop()
    {
      ServiceAction(service => service.Stop, SetByParentService);
    }

    public void OnPause()
    {
      ServiceAction(service => service.Pause, SetByParentService);
    }

    public void OnContinue()
    {
      ServiceAction(service => service.Start, SetByParentService);
    }

    public string ServiceName { get { return ServicesProvider.ServiceName; } }

    #region Root Service Implementation
    protected virtual Result InitializeService()
    {
      var init = ServiceAction(service => service.Initialize);

      BindServices();
      return init;
    }

    protected Result OnStart()
    {
      return ServiceAction(service => service.Start);
    }

    protected Result BindServices()
    {
      try
      {
        return Services.Aggregate(Result.Success, (r, s) => r + BindService(s));
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Collection<TIService> ServicesOfType<TIService>()
      where TIService : IHService
    {
      return Services.OfType<TIService>().ToCollection();
    }

    protected Result ServiceAction<TIService>(Func<TIService, Func<Result,Result>> getAction, TIService serviceType, Result reason = null)
      where TIService : IHService
    {
      var services = ServicesOfType<TIService>();
      var result = services.Aggregate(Result.Success, (current, service) => current + PerformServiceAction(getAction(service), reason));
      return HandleServiceAction(result);
    }

    protected Result ServiceAction(Func<IHService, Func<Result, Result>> getAction, Result reason, Func<IHService, bool> condition = null)
    {
      return ServiceAction(getAction, null, reason, condition);
    }

    protected Result ServiceAction(Func<IHService, Func<Result, Result>> getAction, IEnumerable<IHService> services = null, Result reason = null, Func<IHService, bool> condition = null)
    {
      services = services ?? Services;
      condition = condition ?? Always;

      var result = services.Aggregate(Result.Success, (current, service) => condition(service) ? current + PerformServiceAction(getAction(service), reason) : current);
      return HandleServiceAction(result);
    }

    protected Result PerformServiceAction(Func<Result,Result> action, Result reason)
    {
      try
      {
        return action(reason);
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result HandleServiceAction(Result result)
    {
      Logger.LogSynchronous(result);
      return result;
    }

    protected void HandleError(object o, Result result)
    {
      Logger.LogSynchronous(result);
    }

    protected void HandleStatusChange(object o, ServiceStatusTransition transition)
    {
      var service = o as HService;
      if (service == null) return;

      Logger.LogSynchronous(Result.SingleInfo(Messages.ServiceStatusChanged, service, transition.CurrentStatus));
    }

    protected virtual Result BindService(IHService service)
    {
      service.ErrorOccured += HandleError;
      service.StatusChanged += HandleStatusChange;

      IInteractiveService interactive;
      if ((interactive = service as IInteractiveService) != null)
        interactive.SetServices(Services);

      return Result.Success;
    }
    #endregion

    private bool Always(IHService service)
    {
      return service != null;
    }
  }
}
