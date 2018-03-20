using System;
using System.Xml.Serialization;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlRoot(ElementName = "SocketRequest")]
  public class SocketRequest<TRequest, TRequestTypes>
    where TRequest : class, new()
    where TRequestTypes : struct, IConvertible
  {
    [XmlElement(ElementName = "Request")]
    TRequest Request { get; set; }

    [XmlElement(ElementName = "Type")]
    TRequestTypes Type { get; set; }
  }
}
