using System;
using Hylasoft.Services.Interfaces.Services.Base;
using Hylasoft.Services.Monitoring.Types;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Services
{
  public interface INetworkSocketService<out TRequest, TRequestTypes, TResponse, TResponseTypes> : IHService
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketRequest<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>, new()
  {
    NetworkSocketHandler<TRequest, TRequestTypes, TResponse, TResponseTypes> Handler { set; }
  }
}
