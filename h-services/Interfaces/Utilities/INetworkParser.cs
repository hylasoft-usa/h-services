using System.Net;
using System.Net.Sockets;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Configuration;

namespace Hylasoft.Services.Interfaces.Utilities
{
  public interface INetworkParser
  {
    Result BuildNetworkBindings(INetworkSocketConfig config, out Socket socket, out EndPoint endpoint);
  }
}
