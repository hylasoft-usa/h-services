using System;
using System.Net.Sockets;
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
    public TRequest Request { get; set; }

    [XmlElement(ElementName = "Type")]
    public TRequestTypes Type { get; set; }

    [XmlIgnore]
    internal Socket RequestHandler { get; set; }

    public SocketRequest()
    {
    }

    public SocketRequest(TRequest request, TRequestTypes type)
    {
      Request = request;
      Type = type;
    }
  }
}
