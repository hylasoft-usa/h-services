using Hylasoft.Resolution;

namespace Hylasoft.Services.Types
{
  public class ServiceStatusTransition
  {
    private readonly ServiceStatuses _priorStatus;
    private readonly ServiceStatuses _currentStatus;
    private readonly Result _reason;

    public ServiceStatuses PriorStatus { get { return _priorStatus; } }

    public ServiceStatuses CurrentStatus { get { return _currentStatus; } }

    public Result Reason { get { return _reason; } }

    public ServiceStatusTransition(ServiceStatuses priorStatus, ServiceStatuses currentStatus, Result reason)
    {
      _priorStatus = priorStatus;
      _currentStatus = currentStatus;
      _reason = reason;
    }
  }
}
