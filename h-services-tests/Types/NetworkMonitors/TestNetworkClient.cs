using Hylasoft.Services.Clients;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Utilities;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  public class TestNetworkClient : SocketMonitorClient<TestRequest, RequestTypes, TestResponse, ResponseTypes>
  {
    public TestNetworkClient(INetworkSocketConfig config = null)
      : base(config, new NetworkParser(), new SocketPayloadSerializer())
    {
    }
  }
}
