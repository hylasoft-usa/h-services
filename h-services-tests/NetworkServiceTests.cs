using Hylasoft.Services.Tests.Base;
using Hylasoft.Services.Tests.Types.NetworkMonitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Services.Tests
{
  [TestClass]
  public class NetworkServiceTests : NetworkMonitorTestBase
  {
    [TestMethod]
    public void TestNeworkMonitorService()
    {
      var service = BuildService(PassThrough);
      var client = BuildClient();

      var request = new TestRequest(2);
      Assert.IsTrue(service.Start());

      TestResponse response;
      Assert.IsTrue(client.Send(request, out response));
      Assert.IsTrue(service.Stop());

      Assert.IsNotNull(response);
      Assert.AreEqual(request.RequestInt, response.ResponseInt);

      var error = client.Send(request, out response);
      Assert.IsFalse(error);
      Assert.IsNull(response);
    }
  }
}
