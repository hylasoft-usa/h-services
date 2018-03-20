using Hylasoft.Services.Interfaces.Configuration;

namespace Hylasoft.Services.Configuration
{
  internal class DefaultNetworkSocketingConfig : INetworkSocketConfig
  {
    public uint Port { get { return 46825; } }

    public string Address { get { return null; }}

    public string HostName { get { return "localhost"; } }

    public int CloseConnectionTimeout { get { return 30; } }
    
    public int MaxConnections { get { return 25; } }
  }
}
