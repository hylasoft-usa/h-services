using System;
using System.Xml.Serialization;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlRoot(ElementName = "SocketResponse")]
  public class SocketResponse<TResponseTypes> : SocketPayload<TResponseTypes>
    where TResponseTypes : struct, IConvertible
  {
  }
}
