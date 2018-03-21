using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Services
{
  public interface INetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketPayload<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketPayload<TResponseTypes>, new()
  {
  }
}
