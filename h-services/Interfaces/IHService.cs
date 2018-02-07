using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
  public interface IHService : IServiceStatusElement
  {
    Result Initialize();

    Result Start();

    Result Stop();

    Result Pause();

    Result Restart();

    event EventHandler<ServiceStatusTransition> StatusChanged;

    event EventHandler<Result> ErrorOccured;

    string ServiceName { get; }
  }
}
