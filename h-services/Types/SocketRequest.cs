using System;
using System.Xml.Serialization;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlRoot(ElementName = "SocketRequest")]
  public class SocketRequest<TRequestTypes> : SocketPayload<TRequestTypes>
    where TRequestTypes : struct, IConvertible
  {
  }
}
