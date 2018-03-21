using System;
using System.Xml.Serialization;
using Hylasoft.Extensions;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlType(TypeName = "Message")]
  public class SocketPayload<TPayloadTypes>
    where TPayloadTypes : struct, IConvertible
  {
    [XmlElement(ElementName = "Type")]
    public TPayloadTypes Type { get; set; }

    public SocketPayload()
    {
      Type = TypeExtensions.DefaultValue<TPayloadTypes>();
    }
  }
}
