using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Services;
using Hylasoft.Services.Tests.Types.NetworkMonitors;

namespace Hylasoft.Services.Tests.Types.Services
{
  public class TestNetworkSocketService : NetworkSocketService<TestRequest, RequestTypes, TestResponse, ResponseTypes>
  {
    public TestNetworkSocketService(INetworkSocketMonitor<TestRequest, RequestTypes, TestResponse, ResponseTypes> monitor,
      IServiceValidator validator = null) : base(monitor, validator)
    {
    }
  }
}
