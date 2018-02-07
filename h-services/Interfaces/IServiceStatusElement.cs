using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
  public interface IServiceStatusElement
  {
    ServiceStatuses Status { get; }

    bool IsRunning { get; }

    bool IsStopped { get; }

    bool IsFailed { get; }

    bool IsPaused { get; }
  }
}
