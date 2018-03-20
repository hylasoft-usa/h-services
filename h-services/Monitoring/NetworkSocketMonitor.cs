using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Monitoring.Base;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Monitoring
{
  public abstract class NetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes> : HMonitor, INetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequest : class, new()
    where TRequestTypes : struct, IConvertible
    where TResponse : class, new()
    where TResponseTypes : struct, IConvertible
  {
    private readonly INetworkSocketConfig _config;
    private Socket _socket;
    private EndPoint _endpoint;

    protected INetworkSocketConfig NetworkConfig { get { return _config; } }

    protected Socket ConnectionSocket { get { return _socket; } }

    protected EndPoint ConnectionEndpoint { get { return _endpoint; } }

    protected NetworkSocketMonitor(INetworkSocketConfig config)
    {
      _config = config ?? new DefaultNetworkSocketingConfig();
    }

    #region HMonitor Implementation
    protected override Result OnInitialize()
    {
      Result init;
      // Close any existing connections, if they exist somehow.
      if (!(init = UnbindSocket()))
        return init;

      // Build both socket and endpoint.
      if (!(init += BuildSocket(out _socket)))
        return init;

      return init + BuildEndpoint(out _endpoint);
    }

    protected override Result StartService()
    {
      try
      {
        if (ConnectionSocket == null)
          return Result.SingleFatal(Fatals.NetworkSocketServiceStartedWithoutSocket, ServiceName);

        if (ConnectionEndpoint == null)
          return Result.SingleFatal(Fatals.NetworkSocketServiceStartedWithoutEndpoint, ServiceName);

        ConnectionSocket.Bind(ConnectionEndpoint);
        ConnectionSocket.Listen(NetworkConfig.MaxConnections);
        
        return base.StartService();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected override Result PerformServiceLoop()
    {
      throw new NotImplementedException();
    }

    protected override Result CleanupOnShutdown()
    {
      return UnbindSocket();
    }
    #endregion

    #region INetworkSocketMonitor Implementation
    public event EventHandler<SocketRequest<TRequest, TRequestTypes>> RequestReceived;
    
    public Result SendResponse(SocketResponse<TResponse, TResponseTypes> response)
    {
      throw new NotImplementedException();
    }
    #endregion

    #region Connection Building
    protected Result BuildSocket(out Socket socket)
    {
      socket = null;

      try
      {
        var addressFamily = GetAddressFamily();
        var socketType = GetSocketType();
        var protocolType = GetProtocolType();

        socket = new Socket(addressFamily, socketType, protocolType);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result BuildEndpoint(out EndPoint endpoint)
    {
      endpoint = null;

      try
      {
        var hostEntry = Dns.GetHostEntry(NetworkConfig.HostName);

        IPAddress address;
        if ((address = hostEntry.AddressList.FirstOrDefault(IsAddressMatch)) == null)
          return Result.SingleError(Errors.NetworkSocketAddressNotFound, NetworkConfig.Address, NetworkConfig.HostName);

        endpoint = new IPEndPoint(address, (int)NetworkConfig.Port);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected AddressFamily GetAddressFamily()
    {
      return AddressFamily.InterNetwork;
    }

    protected SocketType GetSocketType()
    {
      return SocketType.Stream;
    }

    protected ProtocolType GetProtocolType()
    {
      return ProtocolType.Tcp;
    }

    protected bool IsAddressMatch(IPAddress address)
    {
      return address != null
             && address.AddressFamily == GetAddressFamily();
    }
    #endregion

    #region Connection Handling
    protected bool IsBound
    {
      get { return ConnectionSocket != null && ConnectionSocket.IsBound; }
    }

    protected bool IsConnected
    {
      get { return IsBound && ConnectionSocket.Connected; }
    }

    protected Result UnbindSocket()
    {
      try
      {
        if (!IsBound)
          return Result.Success;

        ConnectionSocket.Shutdown(SocketShutdown.Both);
        ConnectionSocket.Disconnect(false);
        ConnectionSocket.Dispose();

        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result CloseConnections()
    {
      try
      {
        if (!IsConnected)
          return Result.Success;

        ConnectionSocket.Close(NetworkConfig.CloseConnectionTimeout);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }
    #endregion
  }
}
