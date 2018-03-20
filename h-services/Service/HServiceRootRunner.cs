using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Extensions;
using Hylasoft.Logging;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Providers;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Service
{
  public class HServiceRootRunner : IHServiceRootRunner
  {
    private readonly IHServicesProvider _provider;
    private readonly Collection<IHService> _services;
    private readonly IResultLogger _logger;
    
    protected Collection<IHService> Services { get { return _services; } }

    protected IResultLogger Logger { get { return _logger; } }

    protected IHServicesProvider ServicesProvider { get { return _provider; } }

    public HServiceRootRunner(IHServicesProvider provider)
    {
      _provider = provider;
      _services = ServicesProvider.Services;
      _logger = ServicesProvider.Logger;

      InitalizeService();
    }

    public void OnStart(string[] args)
    {
      ServiceAction(service => service.Start);
    }

    public void OnStop()
    {
      ServiceAction(service => service.Stop);
    }

    public void OnPause()
    {
      ServiceAction(service => service.Pause);
    }

    public void OnContinue()
    {
      OnStart();
    }

    public string ServiceName { get { return ServicesProvider.ServiceName; } }

    #region Root Service Implementation
    protected Result InitalizeService()
    {
      var init = ServiceAction(service => service.Initialize);

      BindServices();
      return init;
    }

    protected Result OnStart()
    {
      return ServiceAction(service => service.Start);
    }

    protected void BindServices()
    {
      foreach (var service in Services)
      {
        service.ErrorOccured += HandleError;
        service.StatusChanged += HandleStatusChange;
      }
    }

    protected Collection<TIService> ServicesOfType<TIService>()
      where TIService : IHService
    {
      return Services.OfType<TIService>().ToCollection();
    }

    protected Result ServiceAction<TIService>(Func<TIService, Func<Result>> getAction, TIService serviceType)
      where TIService : IHService
    {
      var services = ServicesOfType<TIService>();
      var result = services.Aggregate(Result.Success, (current, service) => current + PerformServiceAction(getAction(service)));
      return HandleServiceAction(result);
    }

    protected Result ServiceAction(Func<IHService, Func<Result>> getAction, IEnumerable<IHService> services = null)
    {
      services = services ?? Services;
      var result = services.Aggregate(Result.Success, (current, service) => current + PerformServiceAction(getAction(service)));
      return HandleServiceAction(result);
    }

    protected Result ServiceAction(Func<IHService, Result> action, IEnumerable<IHService> services = null)
    {
      services = services ?? Services;
      return HandleServiceAction(Result.Concat(action, services));
    }

    protected Result PerformServiceAction(Func<Result> action)
    {
      try
      {
        return action();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result HandleServiceAction(Result result)
    {
      Logger.Log(result);
      return result;
    }

    protected void HandleError(object o, Result result)
    {
      Logger.Log(result);
    }

    protected void HandleStatusChange(object o, ServiceStatusTransition transition)
    {
      var service = o as HService;
      if (service == null) return;

      Logger.Log(Result.SingleInfo(Messages.ServiceStatusChanged, service, transition.CurrentStatus));
    }
    #endregion
  }
}
