using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Service
{
  public abstract class HServiceStatusBase : IServiceStatusElement
  {
    private readonly Result _userRequestedTransition;

    protected Result UserRequestedTransition { get { return _userRequestedTransition; } }

    protected HServiceStatusBase()
    {
      _userRequestedTransition = Result.SingleDebug(Debugs.UserRequestedTransition);
    }

    public abstract ServiceStatuses Status { get; }
    public bool IsRunning
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Started:
            return true;
        }

        return false;
      }
    }
    
    public bool IsStopped
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Stopping:
          case ServiceStatuses.Stopped:
          case ServiceStatuses.Paused:
          case ServiceStatuses.Failed:
            return true;
        }

        return false;
      }
    }

    public bool IsFailed
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Failed:
            return true;
        }

        return false;
      }
    }

    public bool IsPaused
    {
      get
      {
        switch (Status)
        {
          case ServiceStatuses.Paused:
            return true;
        }

        return false;
      }
    }
  }
}
