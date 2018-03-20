using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Services
{
  public interface IServiceStatusElement
  {
    ServiceStatuses Status { get; }

    Result Initialize();

    Result Start();

    Result Stop();

    Result Pause();

    Result Restart();

    event EventHandler<ServiceStatusTransition> StatusChanged;

    string ServiceName { get; }

    bool IsRunning { get; }

    bool IsStopped { get; }

    bool IsFailed { get; }

    bool IsPaused { get; }
  }
}
