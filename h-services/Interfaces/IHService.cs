using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
  public interface IHService
  {
    Result Initialize();

    Result Start();

    Result Stop();

    Result Pause();

    Result Restart();

    ServiceStatuses Status { get; }

    bool IsRunning { get; }

    bool IsStopped { get; }

    bool IsFailed { get; }

    bool IsPaused { get; }

    event EventHandler<ServiceStatusTransition> StatusChanged;

    event EventHandler<Result> ErrorOccured;
  }
}
