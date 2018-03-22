using Hylasoft.Resolution;
using Hylasoft.Services.Monitoring.Types;
using Hylasoft.Services.Tests.Types.NetworkMonitors;
using Hylasoft.Services.Types;
using Hylasoft.Services.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class NetworkSocketMonitorTests : TestBase
  {
    [TestMethod]
    public void TestNetworkMonitorInitialize()
    {
      var monitor = BuildMonitor();
      Assert.IsTrue(monitor.Initialize());
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestNetworkMonitorStart()
    {
      var monitor = BuildMonitor();
      Assert.IsTrue(monitor.Start());
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestNetworkMonitorRestart()
    {
      var monitor = BuildMonitor();
      Assert.IsTrue(monitor.Start());
      Assert.IsTrue(monitor.Stop());
      Assert.IsTrue(monitor.Start());
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestSocketPayloadSerialization()
    {
      var serializer = new SocketPayloadSerializer();

      const RequestTypes type = RequestTypes.Test;
      const int intVal = 5;

      var payload = new TestRequest
      {
        Type = type,
        RequestInt = intVal
      };

      string data;
      Assert.IsTrue(serializer.Serialize<TestRequest, RequestTypes>(payload, out data));
      Assert.IsNotNull(data);

      TestRequest deserialized;
      Assert.IsTrue(serializer.Deserialize<TestRequest, RequestTypes>(data, out deserialized));
      Assert.IsNotNull(deserialized);
      Assert.AreEqual(payload.Type, deserialized.Type);
      Assert.AreEqual(payload.RequestInt, deserialized.RequestInt);
    }

    [TestMethod]
    public void TestNetworkClient()
    {
      const int testInt = 12;
      var monitor = BuildMonitor(PassThrough);
      var client = BuildClient();

      var request = new TestRequest(testInt);

      TestResponse response;
      Assert.IsTrue(monitor.Start());
      Assert.IsTrue(client.Send(request, out response));
      Assert.IsTrue(monitor.Stop());
    }
    
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

    protected Result PassThrough(TestRequest request, out TestResponse response)
    {
      response = null;
      if (request == null)
        return Result.SingleFatal("No request,");

      response = new TestResponse
      {
        ResponseInt = request.RequestInt,
        Type = ResponseTypes.Test,
        Result = NetworkResult.FromResult(Result.Success)
      };

      return Result.Success;
    }
  }
}
