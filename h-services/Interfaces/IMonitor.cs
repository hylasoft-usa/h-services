using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
  public interface IMonitor
  {
    Result Start();

    Result Stop();

    ServiceStatuses Status { get; }

    bool IsRunning { get; }

    bool IsStopped { get; }

    event EventHandler<ServiceStatusTransition> StatusChanged;
  }
}
