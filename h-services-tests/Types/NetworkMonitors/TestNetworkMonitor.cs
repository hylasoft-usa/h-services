using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Monitoring;
using Hylasoft.Services.Utilities;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  public class TestNetworkMonitor : NetworkSocketMonitor<TestRequest, RequestTypes, TestResponse, ResponseTypes>
  {
    public TestNetworkMonitor(INetworkSocketConfig config = null) : base(config, new NetworkParser(), new SocketPayloadSerializer())
    {
    }

    public override string ServiceName
    {
      get { return "Test Network Monitor"; }
    }
  }
}
