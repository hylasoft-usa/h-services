using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Monitoring
{
  public interface INetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequest : class, new()
    where TRequestTypes : struct, IConvertible
    where TResponse : class, new()
    where TResponseTypes : struct, IConvertible
  {
    event EventHandler<SocketRequest<TRequest, TRequestTypes>> RequestReceived;

    Result SendResponse(SocketResponse<TResponse, TResponseTypes> response);
  }
}
