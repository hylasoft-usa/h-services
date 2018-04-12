using Hylasoft.Resolution;
using Hylasoft.Services.Monitoring.Types;
using Hylasoft.Services.Tests.Types.NetworkMonitors;
using Hylasoft.Services.Tests.Types.Services;

namespace Hylasoft.Services.Tests.Base
{
  public abstract class NetworkMonitorTestBase : TestBase
  {
    protected TestNetworkMonitor BuildMonitor(NetworkSocketHandler<TestRequest, RequestTypes, TestResponse, ResponseTypes> handler = null)
    {
      var monitor = new TestNetworkMonitor();
      if (handler != null) monitor.Handler = handler;

      return monitor;
    }

    protected TestNetworkClient BuildClient()
    {
      return new TestNetworkClient();
    }

    protected TestNetworkSocketService BuildService(NetworkSocketHandler<TestRequest, RequestTypes, TestResponse, ResponseTypes> handler = null)
    {
      var monitor = BuildMonitor(handler);
      return new TestNetworkSocketService(monitor);
    }

    protected Result PassThrough(TestRequest request, out TestResponse response)
    {
      response = null;
      if (request == null)
        return Result.SingleFatal("No request.");

      response = new TestResponse
      {
        ResponseInt = request.RequestInt,
        Type = ResponseTypes.Test
      };

      return Result.Success;
    }
  }
}
