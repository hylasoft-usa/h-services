using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
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
      _config = config ?? new DefaultNetworkSocketingConfig();
      _netParser = netParser;
      _payloadSerializer = payloadSerializer;
    }

    public Result Send(TRequest request, out TResponse response)
    {
      response = null;

      try
      {
        Result send;
        EndPoint endpoint;
        Socket clientSocket;
        if (!(send = NetParser.BuildNetworkBindings(Config, out clientSocket, out endpoint)))
          return send;
        
        byte[] data;
        if (!(send = Package(request, out data)))
          return send;

        clientSocket.Connect(endpoint);
        clientSocket.Send(data);

        return ReceiveResponse(clientSocket, out response);
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result ReceiveResponse(Socket clientSocket, out TResponse response)
    {
      response = null;

      try
      {
        Result receive;
        string message;
        if (!(receive = ReceiveMessage(clientSocket, out message)))
          return receive;

        NetworkResult responseResult;
        SocketResponsePackage<TResponse, TResponseTypes> package;
        if (!(receive += PayloadSerializer.Deserialize(message, out package))
            || package == null
            || (response = package.Response) == null
            || (responseResult = package.Result) == null)
          return receive;

        return receive + responseResult.ToResult();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result ReceiveMessage(Socket clientSocket, out string message)
    {
      message = null;

      try
      {
        const int bufferSize = 1024;
        var buffer = new byte[bufferSize];
        var builder = new StringBuilder();

        var data = string.Empty;
        while (clientSocket.Receive(buffer, bufferSize, SocketFlags.None) > 0)
        {
          builder.Append(Encoding.ASCII.GetString(buffer));
          if ((data = builder.ToString()).EndsWith(ServiceValues.EoF))
            break;
        }

        message = data.Substring(0, data.IndexOf(ServiceValues.EoF, StringComparison.Ordinal));
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
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
      if (!(package = PayloadSerializer.Serialize(request, out serialized)))
        return package;

      data = string.Format("{0}{1}", serialized, ServiceValues.EoF);
      return package;
    }
  }
}
