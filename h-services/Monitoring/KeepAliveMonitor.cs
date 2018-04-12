using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hylasoft.Extensions;
using Hylasoft.Resolution;
using Hylasoft.Services.Constants;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Monitoring.Base;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;

using StatusTypes = Hylasoft.Services.Types.ServiceStatuses;

namespace Hylasoft.Services.Monitoring
{
  public class KeepAliveMonitor : InteractiveMonitor, IKeepAliveMonitor
  {
    private readonly IDictionary<string, ServiceStatusInformation> _serviceStatuses;

    protected IDictionary<string, ServiceStatusInformation> ServiceStatuses { get { return _serviceStatuses; } }

    public KeepAliveMonitor()
    {
      _serviceStatuses = new Dictionary<string, ServiceStatusInformation>();
    }

    #region IMonitor Implementation
    protected override Result OnInitialize()
    {
      return UpdateServiceStatuses();
    }

    protected override Result PerformServiceLoop()
    {
      return Result.ConcatRestricted(UpdateServiceStatuses, CheckServices);
    }

    protected override Result CleanupOnShutdown()
    {
      try
      {
        ServiceStatuses.Clear();
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    public override string ServiceName
    {
      get { return ServiceNames.KeepAlive; }
    }
    #endregion

    #region IKeepAliveMonitor Implementation
    public Result GetServiceStatusInformation(out Collection<ServiceStatusInformation> statusInformation)
    {
      try
      {
        statusInformation = ServiceStatuses.Values.ToCollection();
        return Result.Success;
      }
      catch (Exception e)
      {
        statusInformation = null;
        return Result.Error(e);
      }
    }

    public event EventHandler<IHService> RevivingService;

    public event EventHandler<IHService> ServiceRevived;

    public event EventHandler<IHService> ReviveFailed;

    Result TriggerRevivingService(IHService service)
    {
      try
      {
        if (RevivingService != null) RevivingService(this, service);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    Result TriggerServiceRevived(IHService service)
    {
      try
      {
        if (ServiceRevived != null) ServiceRevived(this, service);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    Result TriggerReviveFailed(IHService service)
    {
      try
      {
        if (ReviveFailed != null) ReviveFailed(this, service);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }
    #endregion

    #region Domain Methods
    protected Result UpdateServiceStatuses()
    {
      return Result.Concat(UpdateServiceStatus, Services);
    }

    protected Result UpdateServiceStatus(IHService service)
    {
      if (service == null)
        return Result.SingleFatal(Fatals.KeepAliveServicePassedNullService, ServiceName);

      // Add any services that are missing.
      var serviceId = service.ServiceName;
      if (!(ServiceStatuses.ContainsKey(serviceId)))
        ServiceStatuses.Add(serviceId, new ServiceStatusInformation(serviceId));

      // Get reference to monitored service state.
      var statusInfo = ServiceStatuses[serviceId];

      StatusTypes currentStatus;
      var priorStatus = statusInfo.CurrentStatus;

      // If the service is not currently failed, clear failed attempts.
      if ((currentStatus = service.Status) != StatusTypes.Failed)
      {
        statusInfo.FailedAttempts = 0;
      }

      // If the current status is not the prior status, then it was updated externally.
      if (currentStatus != priorStatus)
      {
        statusInfo.LastResult = service.LastTransitionReason;
      }

      statusInfo.CurrentStatus = currentStatus;
      statusInfo.PriorStatus = priorStatus;
      return Result.Success;
    }

    protected Result CheckServices()
    {
      return Result.Concat(CheckService, Services);
    }

    protected Result CheckService(IHService service)
    {
      if (service == null)
        return Result.SingleFatal(Fatals.KeepAliveServicePassedNullService, ServiceName);

      var serviceId = service.ServiceName;
      ServiceStatusInformation serviceInfo;
      if (!(ServiceStatuses.ContainsKey(serviceId)) || (serviceInfo = ServiceStatuses[serviceId]) == null)
        return Result.SingleFatal(Fatals.KeepAliveServiceMissingService, service, ServiceName);

      StatusTypes currentStatus;
      // Nothing to do, if the service isn't failed.
      if ((currentStatus = serviceInfo.CurrentStatus) != StatusTypes.Failed)
        return Result.Success;

      // Status is failed.
      Result revive;
      bool shouldRevive;
      TimeSpan? secondsToWait;
      if (!(revive = CheckIfShouldRevive(serviceInfo, out shouldRevive, out secondsToWait)) || !shouldRevive)
        return revive;

      // Status is failed and should be revived.
      if (!(revive += TriggerRevivingService(service)))
        return revive;

      // Setting reason information about start.
      var reason = Result.SingleInfo(ServiceReasons.KeepAliveStartup, Debugs.StartedByKeepAliveMonitor, serviceInfo.FailedAttempts + 1);
      if (!(Result)(serviceInfo.LastResult = service.Start(reason)))
      {
        // Failed to revive.
        ++serviceInfo.FailedAttempts;
        serviceInfo.LastAttempt = DateTime.Now;
        serviceInfo.CurrentWaitTicks = secondsToWait == null ? (long?) null : secondsToWait.Value.Ticks;
        return revive + TriggerReviveFailed(service);
      }

      // Service revived.
      serviceInfo.CurrentStatus = service.Status;
      serviceInfo.PriorStatus = currentStatus;
      serviceInfo.FailedAttempts = 0;
      serviceInfo.LastAttempt = null;

      ++serviceInfo.Revivals;
      return TriggerServiceRevived(service);
    }

    protected Result CheckIfShouldRevive(ServiceStatusInformation serviceInfo, out bool shouldRevive, out TimeSpan? secondsToWait)
    {
      shouldRevive = false;
      secondsToWait = null;

      // This is not likely to ever happen, without significant rewrites.
      if (serviceInfo == null)
        return Result.SingleFatal(Fatals.KeepAliveServicePassedNullService);

      // Try, if it has never been tried before.
      DateTime? lastAttempt;
      if ((lastAttempt = serviceInfo.LastAttempt) == null)
      {
        shouldRevive = true;
        return Result.Success;
      }

      Result check;
      if (!(check = GetRevivalTimespan(serviceInfo, out secondsToWait)))
        return check;

      var timePassed = DateTime.Now - lastAttempt.Value;
      shouldRevive = timePassed >= secondsToWait;

      return check;
    }

    protected Result GetRevivalTimespan(ServiceStatusInformation serviceInfo, out TimeSpan? secondsToWait)
    {
      secondsToWait = TimeSpan.FromTicks(long.MaxValue);

      // This is not likely to ever happen, without significant rewrites.
      if (serviceInfo == null)
        return Result.SingleFatal(Fatals.KeepAliveServicePassedNullService);

      const int baseSeconds = 5;
      const double growthRate = 1.2;
      var attempts = serviceInfo.FailedAttempts;

      var multiplier = Math.Pow(growthRate, attempts);
      secondsToWait = TimeSpan.FromSeconds(baseSeconds * multiplier);

      return Result.Success;
    }
    #endregion
  }
}
