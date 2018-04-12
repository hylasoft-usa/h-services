using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Services.Base
{
  public interface IServiceStatusElement
  {
    ServiceStatuses Status { get; }

    Result Initialize(Result reason = null);

    Result Start(Result reason = null);

    Result Stop(Result reason = null);

    Result Pause(Result reason = null);

    Result Restart(Result reason = null);

    event EventHandler<ServiceStatusTransition> StatusChanged;

    event EventHandler<Result> ErrorOccured;

    string ServiceName { get; }

    bool IsRunning { get; }

    bool IsStopped { get; }

    bool IsFailed { get; }

    bool IsPaused { get; }

    Result LastTransitionReason { get; }
  }
}
