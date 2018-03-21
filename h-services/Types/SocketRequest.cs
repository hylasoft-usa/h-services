using System;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlRoot(ElementName = "SocketRequest")]
  public class SocketRequest<TRequest, TRequestTypes>
    where TRequestTypes : struct, IConvertible
    where TRequest : SocketPayload<TRequestTypes>, new()
  {
    [XmlElement(ElementName = "Request")]
    public TRequest Request { get; set; }

    [XmlIgnore]
    internal Socket RequestHandler { get; set; }

    public SocketRequest()
    {
    }

    public SocketRequest(TRequest request)
    {
      Request = request;
    }
  }
}
