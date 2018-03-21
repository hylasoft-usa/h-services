using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Monitoring.Types
{
  public delegate Result NetworkSocketHandler<in TRequest, TRequestTypes, TResponse, TResponseTypes>(TRequest request, out TResponse response)
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketRequest<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>, new();
}
