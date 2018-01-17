using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using Hylasoft.Extensions;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Services;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Service
{
  public class HServiceRoot : ServiceBase
  {
    private readonly IHServicesProvider _provider;
    private readonly Collection<IHService> _services;
    private readonly ILogger _logger;

    protected Collection<IHService> Services { get { return _services; } }

    protected ILogger Logger { get { return _logger; } }

    protected IHServicesProvider ServicesProvider { get { return _provider; } }

    public HServiceRoot(IHServicesProvider provider)
    {
      _provider = provider;
      _services = ServicesProvider.Services;
      _logger = ServicesProvider.Logger;

      ServiceName = ServicesProvider.ServiceName;
    }

    #region ServiceBase Implementation
    protected override void OnStart(string[] args)
    {
      ServiceAction(service => service.Start);
    }

    protected override void OnStop()
    {
      ServiceAction(service => service.Stop);
    }

    protected override void OnPause()
    {
      ServiceAction(service => service.Pause);
    }

    protected override void OnContinue()
    {
      OnStart();
    }
    #endregion

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

    protected void HandleStatusChange(object o, ServiceStatuses status)
    {
      var service = o as HService;
      if (service == null) return;

      Logger.Log(Result.SingleInfo(Messages.ServiceStatusChanged, service, status));
    }
    #endregion
  }
}
