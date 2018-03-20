using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Monitoring;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  public class TestNeworkMonitor : NetworkSocketMonitor<TestRequest, RequestTypes, TestResponse, ResponseTypes>
  {
    public TestNeworkMonitor(INetworkSocketConfig config = null) : base(config)
    {
    }

    public override string ServiceName
    {
      get { return "Test Network Monitor"; }
    }
  }
}
