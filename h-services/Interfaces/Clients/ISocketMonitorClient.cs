using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Clients
{
  public interface ISocketMonitorClient<in TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketRequest<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>, new()
  {
    Result Send(TRequest request, out TResponse response);
  }
}
