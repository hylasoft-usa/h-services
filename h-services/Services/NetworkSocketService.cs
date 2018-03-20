using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Resources;
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
    private readonly INetworkSocketConfig _config;
    private Socket _socket;
    private EndPoint _endpoint;

    protected INetworkSocketConfig Config { get { return _config; } }

    protected Socket ConnectionSocket { get { return _socket; } }

    protected EndPoint ConnectionEndpoint { get { return _endpoint; } }

    protected NetworkSocketService(INetworkSocketConfig config)
    {
      _config = config ?? new DefaultNetworkSocketingConfig();
    }

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
