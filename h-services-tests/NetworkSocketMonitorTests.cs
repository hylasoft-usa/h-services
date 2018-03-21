using Hylasoft.Services.Tests.Types.NetworkMonitors;
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
      var monitor = new TestNeworkMonitor();
      Assert.IsTrue(monitor.Initialize());
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestNetworkMonitorStart()
    {
      var monitor = new TestNeworkMonitor();
      Assert.IsTrue(monitor.Start());
      Assert.IsTrue(monitor.Stop());
    }

    [TestMethod]
    public void TestNetworkMonitorRestart()
    {
      var monitor = new TestNeworkMonitor();
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

      var payload = new TestRequest()
      {
        Type = type,
        TestInt = intVal
      };

      string data;
      Assert.IsTrue(serializer.Serialize<TestRequest, RequestTypes>(payload, out data));
      Assert.IsNotNull(data);

      TestRequest deserialized;
      Assert.IsTrue(serializer.Deserialize<TestRequest, RequestTypes>(data, out deserialized));
      Assert.IsNotNull(deserialized);
      Assert.AreEqual(payload.Type, deserialized.Type);
      Assert.AreEqual(payload.TestInt, deserialized.TestInt);
    }
  }
}
