using System;
using System.Xml.Serialization;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlType(TypeName = "ResponsePackage")]
  public class SocketResponsePackage<TResponse, TResponseTypes> : SocketPayloadBase
    where TResponseTypes : struct, IConvertible
    where TResponse : SocketResponse<TResponseTypes>
  {
    [XmlElement(ElementName = "Response")]
    public TResponse Response { get; set; }

    [XmlElement(ElementName = "Result")]
    public NetworkResult Result { get; set; }

    public SocketResponsePackage()
    {
    }

    public SocketResponsePackage(TResponse response, Result result)
    {
      Response = response;
      Result = NetworkResult.FromResult(result);
    }
  }
}
