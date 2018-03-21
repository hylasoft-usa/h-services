using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hylasoft.Resolution;
using Hylasoft.Services.Constants;
using Hylasoft.Services.Interfaces.Clients;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Utilities;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Clients
{
  public class SocketMonitorClient<TRequest, TRequestTypes, TResponse, TResponseTypes> : ISocketMonitorClient<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketRequest<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>, new()
  {
    private readonly INetworkSocketConfig _config;
    private readonly INetworkParser _netParser;
    private readonly ISocketPayloadSerializer _payloadSerializer;

    protected INetworkSocketConfig Config { get { return _config; } }

    protected INetworkParser NetParser { get { return _netParser; } }

    protected ISocketPayloadSerializer PayloadSerializer { get { return _payloadSerializer; } }

    public SocketMonitorClient(INetworkSocketConfig config, INetworkParser netParser, ISocketPayloadSerializer payloadSerializer)
    {
      _config = config;
      _netParser = netParser;
      _payloadSerializer = payloadSerializer;
    }

    public Result Send(TRequest request, out TResponse response)
    {
      try
      {
        Result send;
        EndPoint endpoint;
        Socket clientSocket;
        if (!(send = NetParser.BuildNetworkBindings(Config, out clientSocket, out endpoint)))
          return ErrorOut(send, out response);
        
        byte[] data;
        if (!(send = Package(request, out data)))
          return ErrorOut(send, out response);

        clientSocket.Connect(endpoint);
        clientSocket.Send(data);
        clientSocket.Receive(data);

        return Unpack(data, out response);
      }
      catch (Exception e)
      {
        return ErrorOut(Result.Error(e), out response);
      }
    }

    protected Result ErrorOut(Result error, out TResponse response)
    {
      response = new TResponse
      {
        Result = NetworkResult.FromResult(error)
      };

      return error;
    }

    protected Result Package(TRequest request, out byte[] data)
    {
      data = null;

      string dataStr;
      Result package;
      if (!(package = Package(request, out dataStr)))
        return package;

      data = Encoding.ASCII.GetBytes(dataStr);
      return package;
    }

    protected Result Package(TRequest request, out string data)
    {
      // TODO: Create a class for this.
      data = null;

      Result package;
      string serialized;
      if (!(package = PayloadSerializer.Serialize<TRequest, TRequestTypes>(request, out serialized)))
        return package;

      data = string.Format("{0}{1}", serialized, ServiceValues.EoF);
      return package;
    }

    protected Result Unpack(byte[] data, out TResponse response)
    {
      try
      {
        var dataStr = Encoding.ASCII.GetString(data);
        var dataCleaned = dataStr.Substring(0, dataStr.IndexOf(ServiceValues.EoF, StringComparison.Ordinal));
        
        return PayloadSerializer.Deserialize<TResponse, TResponseTypes>(dataCleaned, out response);
      }
      catch (Exception e)
      {
        return ErrorOut(Result.Error(e), out response);
      }
    }
  }
}
