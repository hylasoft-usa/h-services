using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Utilities
{
  public interface ISocketPayloadSerializer
  {
    Result Serialize<TPayload, TPayloadTypes>(TPayload payload, out string data)
      where TPayloadTypes : struct, IConvertible
      where TPayload : SocketPayload<TPayloadTypes>, new();

    Result Deserialize<TPayload, TPayloadTypes>(string data, out TPayload payload)
      where TPayloadTypes : struct, IConvertible
      where TPayload : SocketPayload<TPayloadTypes>, new();
  }
}
