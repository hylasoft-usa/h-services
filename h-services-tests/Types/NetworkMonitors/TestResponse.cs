using System;
using System.Xml.Serialization;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Tests.Types.NetworkMonitors
{
  [Serializable]
  [XmlType(TypeName = "Response")]
  public class TestResponse : SocketResponse<ResponseTypes>
  {
    [XmlElement(ElementName = "Int")]
    public int ResponseInt { get; set; }
  }
}
