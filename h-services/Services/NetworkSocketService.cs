using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Services;
using Hylasoft.Services.Services.Base;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Services
{
  public abstract class NetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
    : HService, INetworkSocketService<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketPayload<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketPayload<TResponseTypes>, new()
  {
    private readonly INetworkSocketConfig _config;

    protected INetworkSocketConfig Config { get { return _config; } }

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
  }
}
