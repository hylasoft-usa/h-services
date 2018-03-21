using System;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlRoot(ElementName = "SocketResponse")]
  public class SocketResponse<TResponse, TResponseTypes>
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketPayload<TResponseTypes>, new()
  {
    [XmlElement(ElementName = "Response")]
    public TResponse Response { get; set; }

    [XmlElement(ElementName = "Result")]
    public NetworkResult Result { get; set; }

    internal Socket RequestHandler { get; set; }
  }
}
