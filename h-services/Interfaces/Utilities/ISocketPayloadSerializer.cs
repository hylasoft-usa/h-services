using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces.Utilities
{
  public interface ISocketPayloadSerializer
  {
    Result Serialize<TPayload>(TPayload payload, out string data)
      where TPayload : SocketPayloadBase, new();

    Result Deserialize<TPayload>(string data, out TPayload payload)
      where TPayload :SocketPayloadBase, new();
  }
}
