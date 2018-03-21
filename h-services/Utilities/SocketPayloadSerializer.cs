using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Utilities
{
  public class SocketPayloadSerializer
  {
    public Result Serialize<TPayload, TPayloadTypes>(TPayload payload, out string data)
      where TPayloadTypes : struct, IConvertible
      where TPayload : SocketPayload<TPayloadTypes>, new()
    {
      data = null;
      try
      {
        var serializer = BuildSerializer<TPayload, TPayloadTypes>();
        var builder = new StringBuilder();

        using (var stringWriter = new StringWriter(builder))
          using (var writer = BuildXmlWriter(stringWriter))
            serializer.Serialize(writer, payload);

        data = builder.ToString();
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    public Result Deserialize<TPayload, TPayloadTypes>(string data, out TPayload payload)
      where TPayloadTypes : struct, IConvertible
      where TPayload : SocketPayload<TPayloadTypes>, new()
    {
      payload = null;

      try
      {
        var serializer = BuildSerializer<TPayload, TPayloadTypes>();
        using (var stringReader = new StringReader(data))
          using (var reader = XmlReader.Create(stringReader))
            payload = serializer.Deserialize(reader) as TPayload;

        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected XmlSerializer BuildSerializer<TPayload, TPayloadTypes>()
      where TPayloadTypes : struct, IConvertible
      where TPayload : SocketPayload<TPayloadTypes>, new()
    {
      return new XmlSerializer(typeof (TPayload));
    }

    protected XmlWriter BuildXmlWriter(TextWriter textWriter)
    {
      return XmlWriter.Create(textWriter, BuildXmlSettings());
    }

    protected XmlWriterSettings BuildXmlSettings()
    {
      return new XmlWriterSettings
      {
        Async = false,
        Indent = false,
        Encoding = Encoding.ASCII
      };
    }
  }
}
