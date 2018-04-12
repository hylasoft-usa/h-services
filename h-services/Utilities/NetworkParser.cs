using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Utilities;
using Hylasoft.Services.Resources;

namespace Hylasoft.Services.Utilities
{
  public class NetworkParser : INetworkParser
  {
    public Result BuildNetworkBindings(INetworkSocketConfig config, out Socket socket, out EndPoint endpoint)
    {
      endpoint = null;

      Result build;
      if (!(build = BuildSocket(out socket)))
        return build;

      return build + BuildEndpoint(config, out endpoint);
    }

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

    protected Result BuildEndpoint(INetworkSocketConfig config, out EndPoint endpoint)
    {
      endpoint = null;

      try
      {
        var hostEntry = Dns.GetHostEntry(config.HostName);

        IPAddress address;
        if ((address = hostEntry.AddressList.FirstOrDefault(IsAddressMatch)) == null)
          return Result.SingleError(Errors.NetworkSocketAddressNotFound, config.Address, config.HostName);

        endpoint = new IPEndPoint(address, (int)config.Port);
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

  }
}
