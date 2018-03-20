using System;
using System.Xml.Serialization;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlRoot(ElementName = "SocketResponse")]
  public class SocketResponse<TResponse, TResponseTypes>
    where TResponse : class, new()
    where TResponseTypes : struct, IConvertible
  {
    [XmlElement(ElementName = "Response")]
    public TResponse Response { get; set; }

    [XmlElement(ElementName = "Type")]
    public TResponseTypes Type { get; set; }

    [XmlElement(ElementName = "Result")]
    public NetworkResult Result { get; set; }
  }
}
