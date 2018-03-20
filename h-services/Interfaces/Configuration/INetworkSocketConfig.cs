namespace Hylasoft.Services.Interfaces.Configuration
{
  public interface INetworkSocketConfig
  {
    uint Port { get; }

    string Address { get; }

    string HostName { get; }

    int CloseConnectionTimeout { get; }

    int MaxConnections { get; }
  }
}
