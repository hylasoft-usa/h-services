using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Monitoring;
using Hylasoft.Services.Utilities;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  public class TestNeworkMonitor : NetworkSocketMonitor<TestRequest, RequestTypes, TestResponse, ResponseTypes>
  {
    public TestNeworkMonitor(INetworkSocketConfig config = null) : base(config, new NetworkParser(), new SocketPayloadSerializer())
    {
    }

    public override string ServiceName
    {
      get { return "Test Network Monitor"; }
    }
  }
}
