using System;
using System.Xml.Serialization;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  [Serializable]
  [XmlType(TypeName = "Request")]
  public class TestRequest : SocketPayload<RequestTypes>
  {

    [XmlElement(ElementName = "Int")]
    public int TestInt { get; set; }
  }
}
