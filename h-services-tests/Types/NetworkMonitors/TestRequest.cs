using System;
using System.Xml.Serialization;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  [Serializable]
  [XmlType(TypeName = "Request")]
  public class TestRequest : SocketRequest<RequestTypes>
  {

    [XmlElement(ElementName = "Int")]
    public int RequestInt { get; set; }

    public TestRequest()
    {
      Type = RequestTypes.Test;
    }

    public TestRequest(int requestInt)
    {
      RequestInt = requestInt;
    }
  }
}
