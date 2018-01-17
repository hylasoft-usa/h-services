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

    event EventHandler<ServiceStatuses> StatusChanged;

    event EventHandler<Result> ErrorOccured;
  }
}
