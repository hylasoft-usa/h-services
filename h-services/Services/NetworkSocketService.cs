using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Services
{
  public abstract class NetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
    : HService, INetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequest : class, new()
    where TRequestTypes : struct, IConvertible
    where TResponse : class, new()
    where TResponseTypes : struct, IConvertible
  {
    #region HService Implementation
    protected override Result OnInitialize()
    {
      throw new NotImplementedException();
    }

    protected override Result OnStart()
    {
      throw new NotImplementedException();
    }

    protected override Result OnStop()
    {
      throw new NotImplementedException();
    }

    protected override Result OnPause()
    {
      throw new NotImplementedException();
    }
    #endregion

    #region INetworkSocketService Implementation
    public event EventHandler<SocketRequest<TRequest, TRequestTypes>> RequestReceived;

    public Result SendResponse(SocketResponse<TResponse, TResponseTypes> response)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
