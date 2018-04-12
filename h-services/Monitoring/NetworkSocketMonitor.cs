using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Hylasoft.Resolution;
using Hylasoft.Services.Configuration;
using Hylasoft.Services.Constants;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Utilities;
using Hylasoft.Services.Monitoring.Base;
using Hylasoft.Services.Monitoring.Types;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Types;


namespace Hylasoft.Services.Monitoring
{
  public abstract class NetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes> : HMonitor, INetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketRequest<TRequestTypes>, new()
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>, new()
  {
    private readonly INetworkSocketConfig _config;
    private readonly INetworkParser _netParser;
    private Socket _socket;
    private EndPoint _endpoint;
    private readonly ManualResetEvent _accepted;
    private readonly ISocketPayloadSerializer _payloadSerializer;

    protected INetworkSocketConfig NetworkConfig { get { return _config; } }

    protected Socket ConnectionSocket { get { return _socket; } }

    protected EndPoint ConnectionEndpoint { get { return _endpoint; } }

    protected ManualResetEvent Accepted { get { return _accepted; } }

    protected ISocketPayloadSerializer PayloadSerializer { get { return _payloadSerializer; } }

    protected INetworkParser NetParser { get { return _netParser; } }

    protected NetworkSocketMonitor(INetworkSocketConfig config, INetworkParser netParser, ISocketPayloadSerializer payloadSerializer)
    {
      _netParser = netParser;
      _payloadSerializer = payloadSerializer;
      _config = config ?? new DefaultNetworkSocketingConfig();
      _accepted = new ManualResetEvent(false);

      Handler = NoHandler;
    }

    #region HMonitor Implementation
    protected override Result OnInitialize()
    {
      Result init;
      // Close any existing connections, if they exist somehow.
      if (!(init = UnbindSocket()))
        return init;

      return init + NetParser.BuildNetworkBindings(NetworkConfig, out _socket, out _endpoint);
    }

    protected override Result StartService()
    {
      try
      {
        Result init;
        if (!(init = OnInitialize()))
          return init;

        if (ConnectionSocket == null)
          return init + Result.SingleFatal(Fatals.NetworkSocketServiceStartedWithoutSocket, ServiceName);

        if (ConnectionEndpoint == null)
          return init + Result.SingleFatal(Fatals.NetworkSocketServiceStartedWithoutEndpoint, ServiceName);

        ConnectionSocket.Bind(ConnectionEndpoint);
        ConnectionSocket.Listen(NetworkConfig.MaxConnections);
        
        return base.StartService();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected override Result StopService()
    {
      try
      {
        UnbindSocket();
        return base.StopService();
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected override Result PerformServiceLoop()
    {
      Accepted.Reset();
      ConnectionSocket.BeginAccept(AcceptConnection, ConnectionSocket);

      Accepted.WaitOne();
      return Result.Success;
    }

    protected override Result CleanupOnShutdown()
    {
      return UnbindSocket();
    }
    #endregion

    #region INetworkSocketMonitor Implementation
    public NetworkSocketHandler<TRequest, TRequestTypes, TResponse, TResponseTypes> Handler { set; protected get; }

    protected Result NoHandler(TRequest request, out TResponse response)
    {
      response = new TResponse();
      return Result.SingleError(Errors.NoHandlerDefinedForSocketMonitor);
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

        Result unbind;
        if (!(unbind = CloseConnections()))
          return unbind;

        ConnectionSocket.Close(NetworkConfig.CloseConnectionTimeout);
        return unbind;
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

        ConnectionSocket.Shutdown(SocketShutdown.Both);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }
    #endregion

    #region DataObjects
    protected class TransmissionState
    {
      private readonly int _bufferSize;
      private readonly byte[] _buffer;
      private readonly NetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes> _socketMonitor;
      private readonly Socket _handler;

      private readonly StringBuilder _receivingMessage;
      private readonly string _eofTerminator;

      public NetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes> SocketMonitor
      { get { return _socketMonitor; } }

      public int BufferSize { get { return _bufferSize; } }

      public byte[] Buffer { get { return _buffer; } }

      public Socket Handler { get { return _handler; } }

      protected StringBuilder InboundMessage { get { return _receivingMessage; } }

      protected string EofTerminator { get { return _eofTerminator; } }

      protected byte[] OutboundMessage { get; private set; }

      public int BytesToSend { get; private set; }

      protected int TransmissionItterator { get; private set; }

      public TransmissionState(NetworkSocketMonitor<TRequest, TRequestTypes, TResponse, TResponseTypes> socketMonitor, int bufferSize, Socket handler, string eofTerminator)
      {
        _socketMonitor = socketMonitor;
        _bufferSize = bufferSize;
        _handler = handler;
        _eofTerminator = eofTerminator;

        _receivingMessage = new StringBuilder();
        _buffer = new byte[BufferSize];
        BytesToSend = 0;
      }

      public void Receive()
      {
        Handler.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, SocketMonitor.Receive, this);
      }

      public Result Read(IAsyncResult result, out bool done)
      {
        try
        {
          done = false;

          int bytesRead;
          if ((bytesRead = Handler.EndReceive(result)) <= 0)
          {
            done = true;
            return Result.Success;
          }

          // Append new bytes to message.
          InboundMessage.Append(Encoding.ASCII.GetString(Buffer, 0, bytesRead));

          // If EOF is not found, then the message is not over.
          if (!InboundMessage.ToString().EndsWith(EofTerminator))
            return Result.Success;

          // EOF was found.
          done = true;
          return Result.Success;
        }
        catch (Exception e)
        {
          done = true;
          return Result.Error(e);
        }
      }

      public string Parse()
      {
        var message = InboundMessage.ToString();
        return message.Substring(0, message.IndexOf(EofTerminator, StringComparison.Ordinal));
      }

      public Result Send(string message)
      {
        try
        {
          OutboundMessage = Encoding.ASCII.GetBytes(string.Format("{0}{1}", message, EofTerminator));
          BytesToSend = OutboundMessage.Length;
          TransmissionItterator = 0;

          return Send(0);
        }
        catch (Exception e)
        {
          return Result.Error(e);
        }
      }

      public Result Send(int bytesSent)
      {
        try
        {
          BytesToSend -= bytesSent;
          TransmissionItterator += bytesSent;

          // Still more to send.
          if (BytesToSend > 0)
          {
            OutboundMessage
              .Skip(TransmissionItterator)
              .Take(Buffer.Length)
              .ToArray()
              .CopyTo(Buffer, 0);

            Handler.BeginSend(Buffer, 0, Buffer.Length, SocketFlags.None, SocketMonitor.Send, this);
            return Result.Success;
          }

          Handler.Shutdown(SocketShutdown.Both);
          Handler.Close();

          // Data sent.
          return Result.Success;
        }
        catch (Exception e)
        {
          Handler.Shutdown(SocketShutdown.Both);
          Handler.Close();

          return Result.Error(e);
        }
      }
    }
    #endregion

    #region Listening
    protected void AcceptConnection(IAsyncResult result)
    {
      try
      {
        Accepted.Set();

        Socket listener, handler;
        if (result == null
            || (listener = result.AsyncState as Socket) == null
            || (handler = listener.EndAccept(result)) == null)
          return;

        // TODO: Consider making this configurable.
        const int bufferSize = 1024;
       
        // Begin receiving.
        var state = new TransmissionState(this, bufferSize, handler, ServiceValues.EoF);
        state.Receive();
      }
      catch (ObjectDisposedException)
      {
        // Exit gracefully in the event of a shutdown.
      }
      catch (Exception e)
      {
        ErrorOut(e);
      }
    }

    protected void Receive(IAsyncResult result)
    {
      try
      {
        TransmissionState state;
        if ((state = result.AsyncState as TransmissionState) == null)
          return;

        bool done;
        Result receive;
        // Try to read more of the message.
        if (!(receive = state.Read(result, out done)))
        {
          Send(state, receive);
          return;
        }

        // If not finished reading, continue.
        if (!done)
        {
          state.Receive();
          return;
        }

        TRequest request;
        if (!(receive += BuildRequest(state.Parse(), out request)))
        {
          Send(state, receive);
          return;
        }

        TResponse response;
        receive += Handler(request, out response);
        Send(state, receive, response);
      }
      catch (Exception e)
      {
        ErrorOut(e);
      }
    }

    protected void Send(TransmissionState state, Result result, TResponse baseResponse = null)
    {
      result = result ?? Result.Success;

      try
      {
        string data;
        var serialize = Result.Success;
        SocketResponsePackage<TResponse, TResponseTypes> response;
        // TODO: Consider other options.
        if (result.Any(i => i.Level >= ResultIssueLevels.Fatal))
          ErrorOut(result);
        
        if (IsFailed
            || (response = PackResponse(result, baseResponse)) == null
            || !(serialize += PayloadSerializer.Serialize(response, out data))
            || !state.Send(data))
          Send(state, result + serialize);
      }
      catch (Exception e)
      {
        ErrorOut(result + Result.Error(e));
      }
    }

    protected void Send(IAsyncResult result)
    {
      TransmissionState state;
      if ((state = result.AsyncState as TransmissionState) == null)
        return;

      state.Send(state.Handler.EndSend(result));
    }

    protected SocketResponsePackage<TResponse, TResponseTypes> PackResponse(Result result, TResponse baseResponse = null)
    {
      var response = baseResponse ?? new TResponse();
      return new SocketResponsePackage<TResponse, TResponseTypes>(response, result);
    }
    #endregion

    #region Abstract Methods
    protected virtual Result BuildRequest(string message, out TRequest request)
    {
      return PayloadSerializer.Deserialize(message, out request);
    }
    #endregion
  }
}