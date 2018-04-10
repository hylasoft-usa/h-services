using System;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Monitoring.Types;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Services
{
  public abstract class NetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
      : HMonitorService<INetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes>>,
        INetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketRequest<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>, new()
  {
    protected NetworkSocketService(INetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes> monitor, IServiceValidator validator) : base(monitor, validator)
    {
    }

    public NetworkSocketHandler<TRequest, TRequestTypes, TResponse, TResponseTypes> Handler
    {
      set { Monitor.Handler = value; }
    }
  }
}
